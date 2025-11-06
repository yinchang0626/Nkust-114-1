using System.Text.Json.Serialization;

namespace C111181149
{
    public class FoodInfo
    {
        [JsonPropertyName("食品項目識別碼")] 
        public string? FoodNo { get; set; }

        [JsonPropertyName("食品名稱")] 
        public string? FoodName { get; set; }

        [JsonPropertyName("熱量(kcal)")]
        public double? Energy { get; set; }

        [JsonPropertyName("蛋白質(g)")]
        public double? Protein { get; set; }

        [JsonPropertyName("脂肪(g)")]
        public double? Fat { get; set; }

        [JsonPropertyName("碳水化合物(g)")]
        public double? Carbohydrate { get; set; }
    }
}
