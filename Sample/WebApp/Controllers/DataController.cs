using Microsoft.AspNetCore.Mvc;
using WebApp.Data;

namespace WebApp.Controllers
{
    public class DataController : Controller
    {
        private readonly ApplicationDbContext applicationDbContext;

        public DataController(
            ApplicationDbContext applicationDbContext)
        {
            this.applicationDbContext = applicationDbContext;
        }

        public IActionResult CreateFood()
        {
            if (applicationDbContext.FoodEntities.Count() > 0)
            {
                return Ok("Food already imported.");
            }
            // 使用 LINQ 查詢取得不重複的 IntegratedNumber 和 SampleName 組合
            // 對應 SQL: SELECT IntegratedNumber,SampleName,count(*) FROM [FoodNutritionDb].[dbo].[FoodInfos] group by IntegratedNumber,SampleName
            var foodGroups = this.applicationDbContext.FoodInfoEntities
                .Where(f => !string.IsNullOrEmpty(f.IntegratedNumber) && !string.IsNullOrEmpty(f.SampleName))
                .GroupBy(f => new { f.IntegratedNumber, f.SampleName })
                .ToList();

            foodGroups.ForEach(group =>
            {
                FoodEntity food = new FoodEntity()
                {
                    IntegratedNumber = group.Key.IntegratedNumber!,
                    SampleName = group.Key.SampleName!,
                };
                applicationDbContext.FoodEntities.Add(food);
            });

            applicationDbContext.SaveChanges();

            return Ok($"Food Import Done. Total {foodGroups.Count} foods imported.");
        }

        public IActionResult CreateAnalysisItem()
        {
            if (applicationDbContext.AnalysisItemEntities.Count() > 0)
            {
                return Ok("AnalysisItem already imported.");
            }
            var analysisCategories = this.applicationDbContext.FoodInfoEntities
                  .Where(f => !string.IsNullOrEmpty(f.DataCategory) && !string.IsNullOrEmpty(f.AnalysisItem))
                  .GroupBy(f => new { f.DataCategory, f.AnalysisItem })
                  .ToList();

            analysisCategories.ForEach(group =>
            {
                AnalysisItemEntity item = new AnalysisItemEntity()
                {
                    Name = group.Key.AnalysisItem!,
                    DataCategory = group.Key.DataCategory!,
                };
                applicationDbContext.AnalysisItemEntities.Add(item);
            });

            applicationDbContext.SaveChanges();

            return Ok("AnalysisItem Import Done.");
        }
    }
}
