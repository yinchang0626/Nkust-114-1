using Microsoft.AspNetCore.Mvc;
using WebApp.Data;

namespace WebApp.Controllers
{
    public class FoodInfoController : Controller
    {
        private readonly ApplicationDbContext applicationDbContext;

        public FoodInfoController(
            ApplicationDbContext applicationDbContext
            )
        {
            this.applicationDbContext = applicationDbContext;
        }

        public IActionResult Index(int? foodId, int? analysisItemId)
        {
            ViewBag.foodId = foodId;
            ViewBag.analysisItemId = analysisItemId;
            
            var query = applicationDbContext.FoodInfoEntities.AsQueryable();

            // 根據選中的食品實體進行搜尋
            if (foodId.HasValue && foodId > 0)
            {
                var selectedFood = applicationDbContext.FoodEntities.FirstOrDefault(f => f.Id == foodId);
                if (selectedFood != null)
                {
                    // 使用食品的所有欄位作為搜尋條件
                    query = query.Where(x => 
                        x.IntegratedNumber == selectedFood.IntegratedNumber && 
                        x.SampleName == selectedFood.SampleName);
                }
            }

            // 根據選中的分析項目實體進行搜尋
            if (analysisItemId.HasValue && analysisItemId > 0)
            {
                var selectedAnalysisItem = applicationDbContext.AnalysisItemEntities.FirstOrDefault(a => a.Id == analysisItemId);
                if (selectedAnalysisItem != null)
                {
                    // 使用分析項目的所有欄位作為搜尋條件
                    query = query.Where(x => 
                        x.AnalysisItem == selectedAnalysisItem.Name && 
                        x.DataCategory == selectedAnalysisItem.DataCategory);
                }
            }

            return View(query.Take(100).ToList());
        }
    }
}