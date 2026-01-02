using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApp.Data;

namespace WebApp.Controllers
{
    /// <summary>
    /// 食品分析項目關聯控制器
    /// </summary>
    public class FoodAnalysisItemController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FoodAnalysisItemController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// 顯示食品分析項目關聯列表，支援兩個下拉選單搜尋
        /// </summary>
        /// <param name="foodId">食品 ID（直接使用 ID 查詢）</param>
        /// <param name="analysisItemId">分析項目 ID（直接使用 ID 查詢）</param>
        /// <returns></returns>
        public async Task<IActionResult> Index(int? foodId, int? analysisItemId)
        {
            ViewBag.foodId = foodId;
            ViewBag.analysisItemId = analysisItemId;

            var query = _context.FoodAnalysisItemEntities
                .Include(fai => fai.Food)
                .Include(fai => fai.AnalysisItem)
                .Where(fai => fai.IsActive)
                .AsQueryable();

            // 根據食品 ID 直接查詢
            if (foodId.HasValue && foodId > 0)
            {
                query = query.Where(fai => fai.FoodId == foodId);
            }

            // 根據分析項目 ID 直接查詢
            if (analysisItemId.HasValue && analysisItemId > 0)
            {
                query = query.Where(fai => fai.AnalysisItemId == analysisItemId);
            }

            var results = await query
                .OrderBy(fai => fai.Food.SampleName)
                .ThenBy(fai => fai.AnalysisItem.Name)
                .ToListAsync();

            return View(results);
        }

        /// <summary>
        /// 顯示詳細資料
        /// </summary>
        /// <param name="id">食品分析項目關聯 ID</param>
        /// <returns></returns>
        public async Task<IActionResult> Details(int id)
        {
            var foodAnalysisItem = await _context.FoodAnalysisItemEntities
                .Include(fai => fai.Food)
                .Include(fai => fai.AnalysisItem)
                .FirstOrDefaultAsync(fai => fai.Id == id);

            if (foodAnalysisItem == null)
            {
                return NotFound();
            }

            return View(foodAnalysisItem);
        }

        /// <summary>
        /// 匯入功能頁面
        /// </summary>
        /// <returns></returns>
        public IActionResult Import()
        {
            return View();
        }

        /// <summary>
        /// 執行匯入功能 - 從 FoodInfo 轉寫至 FoodAnalysisItem
        /// 支援單一分析項目或該食品的全部分析項目匯入
        /// </summary>
        /// <param name="foodId">選中的食品 ID（必須）</param>
        /// <param name="analysisItemId">選中的分析項目 ID（如果為空或0，則匯入該食品的全部分析項目）</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> ImportFromFoodInfo(int? foodId, int? analysisItemId)
        {
            // 驗證食品必須有選擇
            if (!foodId.HasValue || foodId <= 0)
            {
                return Json(new { success = false, message = "請選擇食品才能執行匯入功能" });
            }

            try
            {
                // 取得選中的食品
                var selectedFood = await _context.FoodEntities.FindAsync(foodId);
                if (selectedFood == null)
                {
                    return Json(new { success = false, message = "找不到指定的食品" });
                }

                // 如果有選擇特定分析項目，執行單筆匯入
                if (analysisItemId.HasValue && analysisItemId > 0)
                {
                    return await ImportSingleAnalysisItem(foodId.Value, analysisItemId.Value);
                }
                // 如果分析項目為空或0，執行該食品的全部分析項目匯入
                else
                {
                    return await ImportAllAnalysisItemsForFood(foodId.Value);
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"匯入失敗: {ex.Message}" });
            }
        }

        /// <summary>
        /// 匯入單一食品分析項目組合
        /// </summary>
        /// <param name="foodId">食品 ID</param>
        /// <param name="analysisItemId">分析項目 ID</param>
        /// <returns></returns>
        private async Task<IActionResult> ImportSingleAnalysisItem(int foodId, int analysisItemId)
        {
            // 取得選中的食品和分析項目
            var selectedFood = await _context.FoodEntities.FindAsync(foodId);
            var selectedAnalysisItem = await _context.AnalysisItemEntities.FindAsync(analysisItemId);

            if (selectedFood == null || selectedAnalysisItem == null)
            {
                return Json(new { success = false, message = "找不到指定的食品或分析項目" });
            }

            // 檢查是否已存在此關聯
            var existingRecord = await _context.FoodAnalysisItemEntities
                .FirstOrDefaultAsync(fai => fai.FoodId == foodId && fai.AnalysisItemId == analysisItemId);

            if (existingRecord != null)
            {
                return Json(new { success = false, message = "此食品與分析項目的關聯已存在" });
            }

            // 從 FoodInfo 中找到匹配的資料
            var matchingFoodInfo = await _context.FoodInfoEntities
                .FirstOrDefaultAsync(fi => 
                    fi.IntegratedNumber == selectedFood.IntegratedNumber &&
                    fi.SampleName == selectedFood.SampleName &&
                    fi.AnalysisItem == selectedAnalysisItem.Name &&
                    fi.DataCategory == selectedAnalysisItem.DataCategory);

            if (matchingFoodInfo == null)
            {
                return Json(new { success = false, message = "在 FoodInfo 中找不到匹配的資料" });
            }

            // 建立新的 FoodAnalysisItem 記錄
            var newRecord = new FoodAnalysisItemEntity
            {
                FoodId = foodId,
                AnalysisItemId = analysisItemId,
                ContentPer100g = TryParseDecimal(matchingFoodInfo.ContentPer100g),
                ContentPerUnit = TryParseDecimal(matchingFoodInfo.ContentPerUnit),
                ContentPerUnitWeight = TryParseDecimal(matchingFoodInfo.ContentPerUnitWeight),
                StandardDeviation = TryParseDecimal(matchingFoodInfo.StandardDeviation),
                SampleCount = TryParseInt(matchingFoodInfo.SampleCount),
                ContentUnit = matchingFoodInfo.ContentUnit,
                DataCategory = matchingFoodInfo.DataCategory,
                UnitWeight = TryParseDecimal(matchingFoodInfo.UnitWeight),
                WasteRate = TryParseDecimal(matchingFoodInfo.WasteRate),
                CreatedAt = DateTime.Now,
                IsActive = true
            };

            _context.FoodAnalysisItemEntities.Add(newRecord);
            await _context.SaveChangesAsync();

            return Json(new { 
                success = true, 
                message = $"成功匯入 {selectedFood.SampleName} 的 {selectedAnalysisItem.Name} 資料",
                recordId = newRecord.Id 
            });
        }

        /// <summary>
        /// 匯入指定食品的所有分析項目
        /// </summary>
        /// <param name="foodId">食品 ID</param>
        /// <returns></returns>
        private async Task<IActionResult> ImportAllAnalysisItemsForFood(int foodId)
        {
            var selectedFood = await _context.FoodEntities.FindAsync(foodId);
            if (selectedFood == null)
            {
                return Json(new { success = false, message = "找不到指定的食品" });
            }

            var importCount = 0;
            var skipCount = 0;
            var errorCount = 0;

            // 找到該食品在 FoodInfo 中的所有記錄
            var foodInfoRecords = await _context.FoodInfoEntities
                .Where(fi => fi.IntegratedNumber == selectedFood.IntegratedNumber 
                            && fi.SampleName == selectedFood.SampleName
                            && !string.IsNullOrEmpty(fi.AnalysisItem)
                            && !string.IsNullOrEmpty(fi.DataCategory))
                .ToListAsync();

            foreach (var foodInfo in foodInfoRecords)
            {
                try
                {
                    // 查找對應的 AnalysisItem
                    var analysisItem = await _context.AnalysisItemEntities
                        .FirstOrDefaultAsync(ai => ai.Name == foodInfo.AnalysisItem 
                                                   && ai.DataCategory == foodInfo.DataCategory);

                    if (analysisItem != null)
                    {
                        // 檢查是否已存在
                        var existingRecord = await _context.FoodAnalysisItemEntities
                            .FirstOrDefaultAsync(fai => fai.FoodId == foodId 
                                                        && fai.AnalysisItemId == analysisItem.Id);

                        if (existingRecord == null)
                        {
                            // 建立新的關聯記錄
                            var newRecord = new FoodAnalysisItemEntity
                            {
                                FoodId = foodId,
                                AnalysisItemId = analysisItem.Id,
                                ContentPer100g = TryParseDecimal(foodInfo.ContentPer100g),
                                ContentPerUnit = TryParseDecimal(foodInfo.ContentPerUnit),
                                ContentPerUnitWeight = TryParseDecimal(foodInfo.ContentPerUnitWeight),
                                StandardDeviation = TryParseDecimal(foodInfo.StandardDeviation),
                                SampleCount = TryParseInt(foodInfo.SampleCount),
                                ContentUnit = foodInfo.ContentUnit,
                                DataCategory = foodInfo.DataCategory,
                                UnitWeight = TryParseDecimal(foodInfo.UnitWeight),
                                WasteRate = TryParseDecimal(foodInfo.WasteRate),
                                CreatedAt = DateTime.Now,
                                IsActive = true
                            };

                            _context.FoodAnalysisItemEntities.Add(newRecord);
                            importCount++;
                        }
                        else
                        {
                            skipCount++;
                        }
                    }
                    else
                    {
                        errorCount++;
                    }
                }
                catch
                {
                    errorCount++;
                }
            }

            await _context.SaveChangesAsync();

            return Json(new { 
                success = true, 
                message = $"成功匯入 {selectedFood.SampleName} 的所有分析項目：成功 {importCount} 筆，跳過 {skipCount} 筆，錯誤 {errorCount} 筆",
                importCount = importCount,
                skipCount = skipCount,
                errorCount = errorCount
            });
        }

        /// <summary>
        /// 批次匯入所有可能的組合
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> ImportAllFromFoodInfo()
        {
            try
            {
                var importCount = 0;
                var skipCount = 0;
                var errorCount = 0;

                // 取得所有 FoodInfo 資料
                var foodInfos = await _context.FoodInfoEntities.ToListAsync();

                foreach (var foodInfo in foodInfos)
                {
                    try
                    {
                        // 查找對應的 Food
                        var food = await _context.FoodEntities
                            .FirstOrDefaultAsync(f => f.IntegratedNumber == foodInfo.IntegratedNumber 
                                                      && f.SampleName == foodInfo.SampleName);

                        // 查找對應的 AnalysisItem
                        var analysisItem = await _context.AnalysisItemEntities
                            .FirstOrDefaultAsync(ai => ai.Name == foodInfo.AnalysisItem 
                                                       && ai.DataCategory == foodInfo.DataCategory);

                        if (food != null && analysisItem != null)
                        {
                            // 檢查是否已存在
                            var existingRecord = await _context.FoodAnalysisItemEntities
                                .FirstOrDefaultAsync(fai => fai.FoodId == food.Id 
                                                            && fai.AnalysisItemId == analysisItem.Id);

                            if (existingRecord == null)
                            {
                                // 建立新的關聯記錄
                                var newRecord = new FoodAnalysisItemEntity
                                {
                                    FoodId = food.Id,
                                    AnalysisItemId = analysisItem.Id,
                                    ContentPer100g = TryParseDecimal(foodInfo.ContentPer100g),
                                    ContentPerUnit = TryParseDecimal(foodInfo.ContentPerUnit),
                                    ContentPerUnitWeight = TryParseDecimal(foodInfo.ContentPerUnitWeight),
                                    StandardDeviation = TryParseDecimal(foodInfo.StandardDeviation),
                                    SampleCount = TryParseInt(foodInfo.SampleCount),
                                    ContentUnit = foodInfo.ContentUnit,
                                    DataCategory = foodInfo.DataCategory,
                                    UnitWeight = TryParseDecimal(foodInfo.UnitWeight),
                                    WasteRate = TryParseDecimal(foodInfo.WasteRate),
                                    CreatedAt = DateTime.Now,
                                    IsActive = true
                                };

                                _context.FoodAnalysisItemEntities.Add(newRecord);
                                importCount++;
                            }
                            else
                            {
                                skipCount++;
                            }
                        }
                        else
                        {
                            errorCount++;
                        }
                    }
                    catch
                    {
                        errorCount++;
                    }
                }

                await _context.SaveChangesAsync();

                return Json(new { 
                    success = true, 
                    message = $"批次匯入完成：成功 {importCount} 筆，跳過 {skipCount} 筆，錯誤 {errorCount} 筆" 
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"批次匯入失敗: {ex.Message}" });
            }
        }

        #region Helper Methods

        /// <summary>
        /// 嘗試將字串轉換為 decimal
        /// </summary>
        private static decimal? TryParseDecimal(string? value)
        {
            if (string.IsNullOrWhiteSpace(value) || value == "-" || value == "tr")
                return null;

            return decimal.TryParse(value, out var result) ? result : null;
        }

        /// <summary>
        /// 嘗試將字串轉換為 int
        /// </summary>
        private static int? TryParseInt(string? value)
        {
            if (string.IsNullOrWhiteSpace(value) || value == "-")
                return null;

            return int.TryParse(value, out var result) ? result : null;
        }

        #endregion
    }
}