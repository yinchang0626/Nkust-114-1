using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConsoleApp
{
    /// <summary>
    /// 食品營養成分資料庫實體類別
    /// </summary>
    [Table("FoodInfos")]
    public class FoodInfoEntity
    {
        /// <summary>
        /// 主鍵 ID（自動遞增）
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// 每單位重含量
        /// </summary>
        [Column(TypeName = "nvarchar(200)")]
        public string? ContentPerUnitWeight { get; set; }

        /// <summary>
        /// 整合編號
        /// </summary>
        [Column(TypeName = "nvarchar(100)")]
        public string? IntegratedNumber { get; set; }

        /// <summary>
        /// 分析項分類
        /// </summary>
        [Column(TypeName = "nvarchar(100)")]
        public string? AnalysisCategory { get; set; }

        /// <summary>
        /// 樣品名稱
        /// </summary>
        [Column(TypeName = "nvarchar(500)")]
        public string? SampleName { get; set; }

        /// <summary>
        /// 每100克含量
        /// </summary>
        [Column(TypeName = "nvarchar(200)")]
        public string? ContentPer100g { get; set; }

        /// <summary>
        /// 每單位含量
        /// </summary>
        [Column(TypeName = "nvarchar(200)")]
        public string? ContentPerUnit { get; set; }

        /// <summary>
        /// 標準差
        /// </summary>
        [Column(TypeName = "nvarchar(200)")]
        public string? StandardDeviation { get; set; }

        /// <summary>
        /// 每單位重
        /// </summary>
        [Column(TypeName = "nvarchar(200)")]
        public string? UnitWeight { get; set; }

        /// <summary>
        /// 含量單位
        /// </summary>
        [Column(TypeName = "nvarchar(100)")]
        public string? ContentUnit { get; set; }

        /// <summary>
        /// 樣本數
        /// </summary>
        [Column(TypeName = "nvarchar(100)")]
        public string? SampleCount { get; set; }

        /// <summary>
        /// 廢棄率
        /// </summary>
        [Column(TypeName = "nvarchar(100)")]
        public string? WasteRate { get; set; }

        /// <summary>
        /// 樣品英文名稱
        /// </summary>
        [Column(TypeName = "nvarchar(500)")]
        public string? SampleEnglishName { get; set; }

        /// <summary>
        /// 資料類別
        /// </summary>
        [Column(TypeName = "nvarchar(100)")]
        public string? DataCategory { get; set; }

        /// <summary>
        /// 分析項
        /// </summary>
        [Column(TypeName = "nvarchar(200)")]
        public string? AnalysisItem { get; set; }

        /// <summary>
        /// 食品分類
        /// </summary>
        [Column(TypeName = "nvarchar(200)")]
        public string? FoodCategory { get; set; }

        /// <summary>
        /// 內容物描述
        /// </summary>
        [Column(TypeName = "nvarchar(max)")]
        public string? ContentDescription { get; set; }

        /// <summary>
        /// 俗名
        /// </summary>
        [Column(TypeName = "nvarchar(500)")]
        public string? CommonName { get; set; }

        /// <summary>
        /// 建立時間
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
