using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebApp.Data;
using WebApp.Models.ViewComponents;

namespace WebApp.ViewComponents
{
    /// <summary>
    /// ???????? ViewComponent
    /// </summary>
    public class AnalysisItemDropdownViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;

        public AnalysisItemDropdownViewComponent(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// ?? ViewComponent
        /// </summary>
        /// <param name="name">HTML ??? name ??</param>
        /// <param name="selectedValue">????</param>
        /// <param name="includeAllOption">????"??"??</param>
        /// <param name="isRequired">????</param>
        /// <param name="id">HTML ??? id ??</param>
        /// <param name="cssClass">CSS ??</param>
        /// <param name="dataCategory">???????</param>
        /// <returns></returns>
        public IViewComponentResult Invoke(
            string name,
            object? selectedValue = null,
            bool includeAllOption = true,
            bool isRequired = false,
            string? id = null,
            string? cssClass = "form-select",
            string? dataCategory = null)
        {
            var query = _context.AnalysisItemEntities.AsQueryable();

            // ???????????????
            if (!string.IsNullOrEmpty(dataCategory))
            {
                query = query.Where(x => x.DataCategory == dataCategory);
            }

            var analysisItems = query
                .OrderBy(x => x.DataCategory)
                .ThenBy(x => x.Name)
                .ToList();

            var selectList = new List<SelectListItem>();

            // ?????????????
            if (!isRequired && includeAllOption)
            {
                selectList.Add(new SelectListItem
                {
                    Value = "",
                    Text = "-- ?? --",
                    Selected = selectedValue == null || selectedValue.ToString() == ""
                });
            }
            // ??????????????
            else if (!isRequired && !includeAllOption)
            {
                selectList.Add(new SelectListItem
                {
                    Value = "",
                    Text = "-- ??? --",
                    Selected = selectedValue == null || selectedValue.ToString() == ""
                });
            }

            // ????????
            foreach (var item in analysisItems)
            {
                selectList.Add(new SelectListItem
                {
                    Value = item.Id.ToString(),
                    Text = $"[{item.DataCategory}] {item.Name}",
                    Selected = selectedValue?.ToString() == item.Id.ToString()
                });
            }

            var model = new DropdownViewComponentModel
            {
                Name = name,
                Id = id ?? name,
                SelectedValue = selectedValue,
                IncludeAllOption = includeAllOption,
                IsRequired = isRequired,
                CssClass = cssClass
            };

            ViewBag.SelectList = selectList;
            return View(model);
        }
    }
}