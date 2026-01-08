using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApp.Data;

/// <summary>
/// ????????
/// </summary>
[Table("Foods")]
public class FoodEntity
{
    /// <summary>
    /// ?? ID
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    /// <summary>
    /// ????
    /// </summary>
    [Column(TypeName = "nvarchar(100)")]
    [Required]
    public string IntegratedNumber { get; set; } = string.Empty;

    /// <summary>
    /// ????
    /// </summary>
    [Column(TypeName = "nvarchar(500)")]
    [Required]
    public string SampleName { get; set; } = string.Empty;

    /// <summary>
    /// ??????
    /// </summary>
    [Column(TypeName = "nvarchar(500)")]
    public string? SampleEnglishName { get; set; }

    /// <summary>
    /// ??
    /// </summary>
    [Column(TypeName = "nvarchar(500)")]
    public string? CommonName { get; set; }

    /// <summary>
    /// ?????
    /// </summary>
    [Column(TypeName = "nvarchar(max)")]
    public string? ContentDescription { get; set; }

    /// <summary>
    /// ????
    /// </summary>
    [Column(TypeName = "nvarchar(200)")]
    public string? FoodCategory { get; set; }

    /// <summary>
    /// ????
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    // ????
    /// <summary>
    /// ??????????
    /// </summary>
    public virtual ICollection<FoodAnalysisItemEntity> FoodAnalysisItems { get; set; } = new List<FoodAnalysisItemEntity>();
}