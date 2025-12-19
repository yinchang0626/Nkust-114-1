using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebApp.Data;
using WebApp.Models.ViewComponents;

namespace WebApp.ViewComponents
{
    /// <summary>
    /// 食品下拉選單 ViewComponent
    /// </summary>
    public class FoodDropdownViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;

        public FoodDropdownViewComponent(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// 調用 ViewComponent
        /// </summary>
        /// <param name="name">HTML 元素的 name 屬性</param>
        /// <param name="selectedValue">選中的值</param>
        /// <param name="includeAllOption">是否包含"全部"選項</param>
        /// <param name="isRequired">是否必選</param>
        /// <param name="id">HTML 元素的 id 屬性</param>
        /// <param name="cssClass">CSS 類別</param>
        /// <param name="integratedNumberPrefix">按整合編號前綴篩選</param>
        /// <returns></returns>
        public IViewComponentResult Invoke(
            string name,
            object? selectedValue = null,
            bool includeAllOption = true,
            bool isRequired = false,
            string? id = null,
            string? cssClass = "form-select",
            string? integratedNumberPrefix = null)
        {
            var query = _context.FoodEntities.AsQueryable();

            // 如果有指定整合編號前綴，則進行篩選
            if (!string.IsNullOrEmpty(integratedNumberPrefix))
            {
                query = query.Where(x => x.IntegratedNumber.StartsWith(integratedNumberPrefix));
            }

            var foods = query
                .OrderBy(x => x.IntegratedNumber)
                .ToList();

            var selectList = new List<SelectListItem>();

            // 如果不是必選且包含全部選項
            if (!isRequired && includeAllOption)
            {
                selectList.Add(new SelectListItem
                {
                    Value = "",
                    Text = "-- 全部 --",
                    Selected = selectedValue == null || selectedValue.ToString() == ""
                });
            }
            // 如果不是必選但不包含全部選項
            else if (!isRequired && !includeAllOption)
            {
                selectList.Add(new SelectListItem
                {
                    Value = "",
                    Text = "-- 請選擇 --",
                    Selected = selectedValue == null || selectedValue.ToString() == ""
                });
            }

            // 加入食品選項
            foreach (var item in foods)
            {
                selectList.Add(new SelectListItem
                {
                    Value = item.Id.ToString(),
                    Text = $"[{item.IntegratedNumber}] {item.SampleName}",
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