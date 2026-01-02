using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers
{
    /// <summary>
    /// API ?????
    /// </summary>
    public class ApiTestController : Controller
    {
        /// <summary>
        /// API ????
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// ?? API ????
        /// </summary>
        /// <returns></returns>
        public IActionResult Food()
        {
            return View();
        }
    }
}