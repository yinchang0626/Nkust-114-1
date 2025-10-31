using System.Text.Json;

namespace ConsoleApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("食品營養成分資訊反序列化範例");

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

                foodInfoList
                    .OrderBy(x => x.IntegratedNumber)
                    .ThenByDescending(y => y.AnalysisCategory);
                var groups = foodInfoList.GroupBy(x => x.IntegratedNumber).ToList();

                if (foodInfoList != null)
                {
                    Console.WriteLine($"成功反序列化 {foodInfoList.Count} 筆食品資料：\n");

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
                        Console.WriteLine($"... 還有 {foodInfoList.Count - 10} 筆資料未顯示");
                    }
                }
                else
                {
                    Console.WriteLine("反序列化失敗：資料為 null");
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
            catch (Exception ex)
            {
                Console.WriteLine($"發生未預期的錯誤: {ex.Message}");
            }

            Console.WriteLine("\n按任意鍵結束...");
            Console.ReadKey();
        }
    }
}
