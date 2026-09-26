using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mandys.Infrastructure.Persistence.Records;

/// <summary>
/// Persistence record for the combos table. A combo bundles sellable dishes
/// and products at a single price.
/// One combo has many <see cref="ComboDishRecord"/> and many
/// <see cref="ComboProductRecord"/> lines.
/// </summary>
public class ComboRecord : Record
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;
    [Required]
    public decimal Price { get; set; }

    /// <summary>
    /// Dishes included in this combo (inverse of
    /// <see cref="ComboDishRecord.Combo"/>).
    /// </summary>
    public ICollection<ComboDishRecord> ComboDishes { get; set; } = new List<ComboDishRecord>();

    /// <summary>
    /// Products included in this combo (inverse of
    /// <see cref="ComboProductRecord.Combo"/>).
    /// </summary>
    public ICollection<ComboProductRecord> ComboProducts { get; set; } = new List<ComboProductRecord>();
}
