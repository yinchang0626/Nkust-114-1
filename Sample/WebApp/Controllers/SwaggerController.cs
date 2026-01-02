using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers
{
    /// <summary>
    /// Swagger ?????
    /// </summary>
    public class SwaggerController : Controller
    {
        /// <summary>
        /// Swagger ?? - ???? Swagger UI
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            return Redirect("/swagger");
        }
    }
}