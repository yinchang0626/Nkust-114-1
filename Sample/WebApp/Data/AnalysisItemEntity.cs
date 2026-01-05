using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApp.Data;

/// <summary>
/// ????????
/// </summary>
[Table("AnalysisItems")]
public class AnalysisItemEntity
{
    /// <summary>
    /// ?? ID
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    /// <summary>
    /// ?????
    /// </summary>
    [Column(TypeName = "nvarchar(200)")]
    [Required]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// ????
    /// </summary>
    [Column(TypeName = "nvarchar(100)")]
    [Required]
    public string DataCategory { get; set; } = string.Empty;

    /// <summary>
    /// ?????
    /// </summary>
    [Column(TypeName = "nvarchar(500)")]
    public string? Description { get; set; }

    /// <summary>
    /// ????
    /// </summary>
    [Column(TypeName = "nvarchar(50)")]
    public string? DefaultUnit { get; set; }

    /// <summary>
    /// ????
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    /// <summary>
    /// ????
    /// </summary>
    public bool IsActive { get; set; } = true;

    // ????
    /// <summary>
    /// ??????????
    /// </summary>
    public virtual ICollection<FoodAnalysisItemEntity> FoodAnalysisItems { get; set; } = new List<FoodAnalysisItemEntity>();
}