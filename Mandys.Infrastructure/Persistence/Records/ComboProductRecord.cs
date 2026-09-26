using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mandys.Infrastructure.Persistence.Records;

/// <summary>
/// Persistence record for the combo products table. A combo line: one
/// <see cref="ComboRecord"/> includes one <see cref="ProductRecord"/> in the
/// given <see cref="Quantity"/>.
/// </summary>
public class ComboProductRecord : Record
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    /// <summary>
    /// Combo this line belongs to (inverse of
    /// <see cref="ComboRecord.ComboProducts"/>).
    /// </summary>
    [Required]
    public int ComboId { get; set; }
    public ComboRecord Combo { get; set; } = null!;

    /// <summary>
    /// Product included in the combo (inverse of
    /// <see cref="ProductRecord.ComboProducts"/>).
    /// </summary>
    [Required]
    public int ProductId { get; set; }
    public ProductRecord Product { get; set; } = null!;

    /// <summary>
    /// Amount of the product the combo includes, in the product's measure
    /// unit.
    /// </summary>
    [Required]
    public int Quantity { get; set; }
}
