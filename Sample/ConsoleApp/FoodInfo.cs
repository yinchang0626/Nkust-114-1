using System.Text.Json.Serialization;

namespace ConsoleApp
{
    /// <summary>
    /// 食品營養成分資訊類別
    /// </summary>
    public class FoodInfo
    {
        /// <summary>
        /// 每單位重含量
        /// </summary>
        [JsonPropertyName("每單位重含量")]
        public string? ContentPerUnitWeight { get; set; }

        /// <summary>
        /// 整合編號
        /// </summary>
        [JsonPropertyName("整合編號")]
        public string? IntegratedNumber { get; set; }

        /// <summary>
        /// 分析項分類
        /// </summary>
        [JsonPropertyName("分析項分類")]
        public string? AnalysisCategory { get; set; }

        /// <summary>
        /// 樣品名稱
        /// </summary>
        [JsonPropertyName("樣品名稱")]
        public string? SampleName { get; set; }

        /// <summary>
        /// 每100克含量
        /// </summary>
        [JsonPropertyName("每100克含量")]
        public string? ContentPer100g { get; set; }

        /// <summary>
        /// 每單位含量
        /// </summary>
        [JsonPropertyName("每單位含量")]
        public string? ContentPerUnit { get; set; }

        /// <summary>
        /// 標準差
        /// </summary>
        [JsonPropertyName("標準差")]
        public string? StandardDeviation { get; set; }

        /// <summary>
        /// 每單位重
        /// </summary>
        [JsonPropertyName("每單位重")]
        public string? UnitWeight { get; set; }

        /// <summary>
        /// 含量單位
        /// </summary>
        [JsonPropertyName("含量單位")]
        public string? ContentUnit { get; set; }

        /// <summary>
        /// 樣本數
        /// </summary>
        [JsonPropertyName("樣本數")]
        public string? SampleCount { get; set; }

        /// <summary>
        /// 廢棄率
        /// </summary>
        [JsonPropertyName("廢棄率")]
        public string? WasteRate { get; set; }

        /// <summary>
        /// 樣品英文名稱
        /// </summary>
        [JsonPropertyName("樣品英文名稱")]
        public string? SampleEnglishName { get; set; }

        /// <summary>
        /// 資料類別
        /// </summary>
        [JsonPropertyName("資料類別")]
        public string? DataCategory { get; set; }

        /// <summary>
        /// 分析項
        /// </summary>
        [JsonPropertyName("分析項")]
        public string? AnalysisItem { get; set; }

        /// <summary>
        /// 食品分類
        /// </summary>
        [JsonPropertyName("食品分類")]
        public string? FoodCategory { get; set; }

        /// <summary>
        /// 內容物描述
        /// </summary>
        [JsonPropertyName("內容物描述")]
        public string? ContentDescription { get; set; }

        /// <summary>
        /// 俗名
        /// </summary>
        [JsonPropertyName("俗名")]
        public string? CommonName { get; set; }

        /// <summary>
        /// 覆寫 ToString 方法以便於除錯和顯示
        /// </summary>
        /// <returns>物件的字串表示</returns>
        public override string ToString()
        {
            return $"FoodInfo: {SampleName} ({SampleEnglishName}) - {AnalysisItem}: {ContentPer100g} {ContentUnit}";
        }
    }
}