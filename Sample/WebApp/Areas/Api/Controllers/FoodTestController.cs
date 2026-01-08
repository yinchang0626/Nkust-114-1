using Microsoft.AspNetCore.Mvc;

namespace WebApp.Areas.Api.Controllers
{
    [Area("Api")]
    public class FoodTestController : Controller
    {
        /// <summary>
        /// 食品 API 測試頁面
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            return View();
        }
    }
}
