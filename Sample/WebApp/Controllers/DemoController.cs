using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers
{
    public class DemoController : Controller
    {
        /// <summary>
        /// ?????? ViewComponent ???????
        /// </summary>
        /// <returns></returns>
        public IActionResult DropdownDemo()
        {
            return View();
        }

        /// <summary>
        /// ??????
        /// </summary>
        /// <param name="analysisItemId">??????? ID</param>
        /// <param name="foodId">????? ID</param>
        /// <param name="requiredAnalysisItemId">??????? ID</param>
        /// <param name="requiredFoodId">????? ID</param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult DropdownDemo(int? analysisItemId, int? foodId, int? requiredAnalysisItemId, int? requiredFoodId)
        {
            ViewBag.Message = $"????: ????ID={analysisItemId}, ??ID={foodId}, ??????ID={requiredAnalysisItemId}, ????ID={requiredFoodId}";
            return View();
        }
    }
}