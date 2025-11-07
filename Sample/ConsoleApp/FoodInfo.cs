using System.Text.Json.Serialization;

namespace ConsoleApp
{
    /// <summary>
    /// 食品 / 或新 JSON 的紀錄類別（已對應到新 JSON 的欄位名稱）
    /// </summary>
    public class FoodInfo
    {
        // 對應 JSON "sitename"
        [JsonPropertyName("sitename")]
        public string? SampleName { get; set; }

        // 對應 JSON "siteid"（舊程式把 SampleEnglishName 寫入 DB，用來存 site id）
        [JsonPropertyName("siteid")]
        public string? SampleEnglishName { get; set; }

        // 對應 JSON "status"
        [JsonPropertyName("status")]
        public string? AnalysisCategory { get; set; }

        // 對應 JSON "pollutant"
        [JsonPropertyName("pollutant")]
        public string? AnalysisItem { get; set; }

        // 對應 JSON "aqi"
        [JsonPropertyName("aqi")]
        public string? ContentPer100g { get; set; }

        // 對應 JSON "county"
        [JsonPropertyName("county")]
        public string? FoodCategory { get; set; }

        // 對應 JSON "publishtime"
        [JsonPropertyName("publishtime")]
        public string? ContentDescription { get; set; }

        // 其他欄位（可視需要寫入 DB 或顯示）
        [JsonPropertyName("so2")]
        public string? So2 { get; set; }

        [JsonPropertyName("co")]
        public string? Co { get; set; }

        [JsonPropertyName("o3")]
        public string? O3 { get; set; }

        [JsonPropertyName("o3_8hr")]
        public string? O3_8hr { get; set; }

        [JsonPropertyName("pm10")]
        public string? Pm10 { get; set; }

        // JSON key 有點 (pm2.5)，C# 屬性取名為 Pm25
        [JsonPropertyName("pm2.5")]
        public string? Pm25 { get; set; }

        [JsonPropertyName("no2")]
        public string? No2 { get; set; }

        [JsonPropertyName("nox")]
        public string? Nox { get; set; }

        [JsonPropertyName("no")]
        public string? No { get; set; }

        [JsonPropertyName("wind_speed")]
        public string? WindSpeed { get; set; }

        [JsonPropertyName("wind_direc")]
        public string? WindDirec { get; set; }

        [JsonPropertyName("co_8hr")]
        public string? Co_8hr { get; set; }

        [JsonPropertyName("pm2.5_avg")]
        public string? Pm25_Avg { get; set; }

        [JsonPropertyName("pm10_avg")]
        public string? Pm10_Avg { get; set; }

        [JsonPropertyName("so2_avg")]
        public string? So2_Avg { get; set; }

        [JsonPropertyName("longitude")]
        public string? Longitude { get; set; }

        [JsonPropertyName("latitude")]
        public string? Latitude { get; set; }

        // 舊程式需要但新 JSON 沒提供的欄位（保留，會是 null）
        public string? ContentPerUnitWeight { get; set; }
        public string? IntegratedNumber { get; set; }
        public string? ContentPerUnit { get; set; }
        public string? StandardDeviation { get; set; }
        public string? UnitWeight { get; set; }
        public string? ContentUnit { get; set; }
        public string? SampleCount { get; set; }
        public string? WasteRate { get; set; }
        public string? DataCategory { get; set; }
        public string? FoodCategoryDescription { get; set; }
        public string? CommonName { get; set; }

        public override string ToString()
        {
            return $"Record: {SampleName} ({SampleEnglishName}) - AQI: {ContentPer100g} Status: {AnalysisCategory}";
        }
    }
}