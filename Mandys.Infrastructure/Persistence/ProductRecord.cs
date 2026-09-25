using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Mandys.Domain;

namespace Mandys.Infrastructure.Persistence;

/// <summary>
/// Persistence record for the product table. Mapped to/from
/// <see cref="Product"/> by <see cref="ProductMapper"/>.
/// </summary>
public class ProductRecord : Record
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    [Required]
    public string Description { get; set; } = string.Empty;
    [Required]
    public string IsSupply { get; set; } = string.Empty;
    [Required]
    public string Price { get; set; } = string.Empty;
    [Required]
    public string MeasureUnit { get; set; } = string.Empty;
}
