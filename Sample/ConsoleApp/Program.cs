using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace ConsoleApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8; // 設定控制台輸出編碼為 UTF-8

            Console.WriteLine("食品營養成分資訊 - JSON 反序列化並寫入資料庫");

            // 使用 AppDomain 獲取應用程式基礎目錄
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string jsonFilePath = Path.Combine(baseDirectory, "App_Data", "20_5.json");

            Console.WriteLine($"嘗試讀取檔案: {jsonFilePath}");

            if (!File.Exists(jsonFilePath))
            {
                Console.WriteLine($"錯誤：找不到檔案 {jsonFilePath}");
                Console.WriteLine("\n按任意鍵結束...");
                Console.ReadKey();
                return;
            }

            try
            {
                // 從檔案讀取 JSON 資料
                Console.WriteLine("正在讀取 JSON 檔案...");
                string jsonString = File.ReadAllText(jsonFilePath);
                Console.WriteLine($"檔案大小: {new FileInfo(jsonFilePath).Length / 1024 / 1024:F2} MB");

                // 反序列化 JSON 資料
                Console.WriteLine("正在進行反序列化...");
                var foodInfoList = JsonSerializer.Deserialize<List<FoodInfo>>(jsonString);

                if (foodInfoList != null && foodInfoList.Count > 0)
                {
                    Console.WriteLine($"成功反序列化 {foodInfoList.Count} 筆食品資料\n");

                    // 顯示前 10 筆資料以避免輸出過多
                    int displayCount = Math.Min(10, foodInfoList.Count);
                    Console.WriteLine($"顯示前 {displayCount} 筆資料：\n");

                    for (int i = 0; i < displayCount; i++)
                    {
                        var foodInfo = foodInfoList[i];
                        Console.WriteLine($"第 {i + 1} 筆資料:");
                        Console.WriteLine($"整合編號: {foodInfo.IntegratedNumber}");
                        Console.WriteLine($"樣品名稱: {foodInfo.SampleName} ({foodInfo.SampleEnglishName})");
                        Console.WriteLine($"分析項分類: {foodInfo.AnalysisCategory}");
                        Console.WriteLine($"分析項: {foodInfo.AnalysisItem}");
                        Console.WriteLine($"每100克含量: {foodInfo.ContentPer100g} {foodInfo.ContentUnit}");
                        Console.WriteLine($"食品分類: {foodInfo.FoodCategory}");
                        Console.WriteLine($"內容物描述: {foodInfo.ContentDescription}");
                        Console.WriteLine(new string('-', 50));
                    }

                    if (foodInfoList.Count > 10)
                    {
                        Console.WriteLine($"... 還有 {foodInfoList.Count - 10} 筆資料未顯示\n");
                    }

                    // ========== 將資料寫入資料庫 ==========
                    Console.WriteLine("\n正在將資料寫入資料庫...");
                    
                    using (var dbContext = new FoodDbContext())
                    {
                        // 確保資料庫已建立
                        dbContext.Database.EnsureCreated();

                        // 檢查資料庫中是否已有資料
                        int existingCount = dbContext
                            .FoodInfos
                            .Count();

                        Console.WriteLine($"資料庫中現有資料筆數: {existingCount}");

                        // 詢問是否要清空資料表
                        if (existingCount > 0)
                        {
                            Console.Write("資料庫中已有資料，是否要先清空資料表？(Y/N): ");
                            string? answer = Console.ReadLine();
                            if (answer?.ToUpper() == "Y")
                            {
                                Console.WriteLine("正在清空資料表...");
                                dbContext.Database.ExecuteSqlRaw("TRUNCATE TABLE FoodInfos");
                                Console.WriteLine("資料表已清空");
                            }
                            else
                            {
                                Console.WriteLine("將新增資料到現有資料中");
                            }
                        }

                        // 轉換 FoodInfo 到 FoodInfoEntity 並寫入資料庫
                        Console.WriteLine($"開始寫入 {foodInfoList.Count} 筆資料到資料庫...");
                        
                        int batchSize = 1000;
                        int totalBatches = (foodInfoList.Count + batchSize - 1) / batchSize;
                        
                        for (int batch = 0; batch < totalBatches; batch++)
                        {
                            var batchData = foodInfoList
                                .Skip(batch * batchSize)
                                .Take(batchSize)
                                .Select(f => new FoodInfoEntity
                                {
                                    ContentPerUnitWeight = f.ContentPerUnitWeight,
                                    IntegratedNumber = f.IntegratedNumber,
                                    AnalysisCategory = f.AnalysisCategory,
                                    SampleName = f.SampleName,
                                    ContentPer100g = f.ContentPer100g,
                                    ContentPerUnit = f.ContentPerUnit,
                                    StandardDeviation = f.StandardDeviation,
                                    UnitWeight = f.UnitWeight,
                                    ContentUnit = f.ContentUnit,
                                    SampleCount = f.SampleCount,
                                    WasteRate = f.WasteRate,
                                    SampleEnglishName = f.SampleEnglishName,
                                    DataCategory = f.DataCategory,
                                    AnalysisItem = f.AnalysisItem,
                                    FoodCategory = f.FoodCategory,
                                    ContentDescription = f.ContentDescription,
                                    CommonName = f.CommonName
                                })
                                .ToList();

                            dbContext.FoodInfos.AddRange(batchData);
                            dbContext.SaveChanges();

                            int processedCount = (batch + 1) * batchSize;
                            if (processedCount > foodInfoList.Count)
                                processedCount = foodInfoList.Count;
                            
                            Console.WriteLine($"已寫入 {processedCount}/{foodInfoList.Count} 筆資料 ({(processedCount * 100.0 / foodInfoList.Count):F1}%)");
                        }

                        Console.WriteLine("\n資料寫入完成！");
                        
                        // 顯示資料庫統計
                        int finalCount = dbContext.FoodInfos.Count();
                        Console.WriteLine($"資料庫中總共有 {finalCount} 筆資料");
                        
                        // 顯示一些統計資訊
                        var categoryStats = dbContext.FoodInfos
                            .GroupBy(f => f.AnalysisCategory)
                            .Select(g => new { Category = g.Key, Count = g.Count() })
                            .OrderByDescending(x => x.Count)
                            .Take(5)
                            .ToList();

                        Console.WriteLine("\n前 5 大分析項分類統計:");
                        foreach (var stat in categoryStats)
                        {
                            Console.WriteLine($"  {stat.Category}: {stat.Count} 筆");
                        }
                    }
                }
                else
                {
                    Console.WriteLine("反序列化失敗：資料為 null 或空");
                }
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"JSON 反序列化錯誤: {ex.Message}");
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine($"檔案不存在錯誤: {ex.Message}");
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine($"檔案存取權限錯誤: {ex.Message}");
            }
            catch (IOException ex)
            {
                Console.WriteLine($"檔案讀取錯誤: {ex.Message}");
            }
            catch (OutOfMemoryException ex)
            {
                Console.WriteLine($"記憶體不足錯誤 (檔案可能太大): {ex.Message}");
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"資料庫更新錯誤: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"詳細錯誤: {ex.InnerException.Message}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"發生未預期的錯誤: {ex.Message}");
                Console.WriteLine($"錯誤類型: {ex.GetType().Name}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"內部錯誤: {ex.InnerException.Message}");
                }
            }

            Console.WriteLine("\n按任意鍵結束...");
            Console.ReadKey();
        }
    }
}
