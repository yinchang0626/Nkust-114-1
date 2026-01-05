using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.Contracts;

namespace WebApp.Data
{
    /// <summary>
    /// 食品分析項目關聯實體（多對多關係中間表）
    /// </summary>
    [Table("FoodAnalysisItems")]
    public class FoodAnalysisItemEntity
    {
        /// <summary>
        /// 主鍵 ID
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// 食品 ID（外鍵）
        /// </summary>
        [Required]
        public int FoodId { get; set; }

        /// <summary>
        /// 分析項目 ID（外鍵）
        /// </summary>
        [Required]
        public int AnalysisItemId { get; set; }

        /// <summary>
        /// 每100克含量
        /// </summary>
        [Column(TypeName = "decimal(18,4)")]
        public decimal? ContentPer100g { get; set; }

        /// <summary>
        /// 每單位含量
        /// </summary>
        [Column(TypeName = "decimal(18,4)")]
        public decimal? ContentPerUnit { get; set; }

        /// <summary>
        /// 每單位重含量
        /// </summary>
        [Column(TypeName = "decimal(18,4)")]
        public decimal? ContentPerUnitWeight { get; set; }

        /// <summary>
        /// 標準差
        /// </summary>
        [Column(TypeName = "decimal(18,4)")]
        public decimal? StandardDeviation { get; set; }

        /// <summary>
        /// 樣本數
        /// </summary>
        public int? SampleCount { get; set; }

        /// <summary>
        /// 含量單位
        /// </summary>
        [Column(TypeName = "nvarchar(100)")]
        public string? ContentUnit { get; set; }

        /// <summary>
        /// 資料類別
        /// </summary>
        [Column(TypeName = "nvarchar(100)")]
        public string? DataCategory { get; set; }

        /// <summary>
        /// 每單位重（克）
        /// </summary>
        [Column(TypeName = "decimal(10,2)")]
        public decimal? UnitWeight { get; set; }

        /// <summary>
        /// 廢棄率（%）
        /// </summary>
        [Column(TypeName = "decimal(5,2)")]
        public decimal? WasteRate { get; set; }

        /// <summary>
        /// 建立時間
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        /// <summary>
        /// 更新時間
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// 是否啟用
        /// </summary>
        public bool IsActive { get; set; } = true;

        // 導航屬性
        /// <summary>
        /// 關聯的食品實體
        /// </summary>
        public virtual FoodEntity Food { get; set; } = null!;

        /// <summary>
        /// 關聯的分析項目實體
        /// </summary>
        public virtual AnalysisItemEntity AnalysisItem { get; set; } = null!;
    }
}
