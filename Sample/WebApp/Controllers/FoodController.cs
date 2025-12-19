using Microsoft.AspNetCore.Mvc;
using WebApp.Data;

namespace WebApp.Controllers
{
    public class FoodController : Controller
    {
        private readonly ApplicationDbContext applicationDbContext;

        public FoodController(
            ApplicationDbContext applicationDbContext
            )
        {
            this.applicationDbContext = applicationDbContext;
        }

        public IActionResult Index(string? integratedNumber, string? sampleName)
        {
            ViewBag.IntegratedNumber = integratedNumber;
            ViewBag.SampleName = sampleName;
            
            var query = applicationDbContext
                .FoodEntities
                .AsQueryable();

            if (!string.IsNullOrEmpty(integratedNumber))
            {
                query = query.Where(x => x.IntegratedNumber.Contains(integratedNumber));
            }

            if (!string.IsNullOrEmpty(sampleName))
            {
                query = query.Where(x => x.SampleName.Contains(sampleName));
            }

            return View(query.OrderBy(x => x.IntegratedNumber).ToList());
        }

        public IActionResult Details(int id)
        {
            var food = applicationDbContext.FoodEntities
                .FirstOrDefault(x => x.Id == id);

            if (food == null)
            {
                return NotFound();
            }

            return View(food);
        }
    }
}