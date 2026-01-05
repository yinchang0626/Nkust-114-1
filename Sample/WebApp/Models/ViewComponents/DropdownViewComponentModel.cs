namespace WebApp.Models.ViewComponents
{
    /// <summary>
    /// ???? ViewComponent ?????
    /// </summary>
    public class DropdownViewComponentModel
    {
        /// <summary>
        /// HTML ??? name ??
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// HTML ??? id ??
        /// </summary>
        public string? Id { get; set; }

        /// <summary>
        /// ????
        /// </summary>
        public object? SelectedValue { get; set; }

        /// <summary>
        /// ????"??"????? null ??
        /// </summary>
        public bool IncludeAllOption { get; set; } = true;

        /// <summary>
        /// "??"???????
        /// </summary>
        public string AllOptionText { get; set; } = "-- ?? --";

        /// <summary>
        /// ????????????
        /// </summary>
        public bool IsRequired { get; set; } = false;

        /// <summary>
        /// CSS ??
        /// </summary>
        public string? CssClass { get; set; } = "form-select";

        /// <summary>
        /// ???????????????????
        /// </summary>
        public string PlaceholderText { get; set; } = "-- ??? --";
    }
}