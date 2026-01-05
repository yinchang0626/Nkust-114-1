using Microsoft.AspNetCore.Mvc;
using WebApp.Data;

namespace WebApp.Controllers
{
    public class AnalysisItemController : Controller
    {
        private readonly ApplicationDbContext applicationDbContext;

        public AnalysisItemController(
            ApplicationDbContext applicationDbContext
            )
        {
            this.applicationDbContext = applicationDbContext;
        }

        public IActionResult Index(string? dataCategory, string? name)
        {
            ViewBag.DataCategory = dataCategory;
            ViewBag.Name = name;
            
            var query = applicationDbContext
                .AnalysisItemEntities
                .AsQueryable();

            if (!string.IsNullOrEmpty(dataCategory))
            {
                query = query.Where(x => x.DataCategory == dataCategory);
            }

            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(x => x.Name.Contains(name));
            }

            return View(query.OrderBy(x => x.DataCategory).ThenBy(x => x.Name).ToList());
        }

        public IActionResult Details(int id)
        {
            var analysisItem = applicationDbContext.AnalysisItemEntities
                .FirstOrDefault(x => x.Id == id);

            if (analysisItem == null)
            {
                return NotFound();
            }

            return View(analysisItem);
        }
    }
}