using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Mandys.Domain;

namespace Mandys.Infrastructure.Persistence;

/// <summary>
/// Persistence record for the product table. Mapped to/from
/// <see cref="Product"/> by <see cref="ProductMapper"/>.
/// One product is used by many <see cref="DishProductRecord"/> recipe lines.
/// </summary>
public class ProductRecord : Record
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    [Required]
    [MaxLength(50)]
    public string Description { get; set; } = string.Empty;
    [Required]
    public bool IsSupply { get; set; }
    [Required]
    public decimal Price { get; set; }
    [Required]
    [MaxLength(50)]
    public string MeasureUnit { get; set; } = string.Empty;

    /// <summary>
    /// Recipe lines using this product (inverse of
    /// <see cref="DishProductRecord.Product"/>).
    /// </summary>
    public ICollection<DishProductRecord> DishProducts { get; set; } = new List<DishProductRecord>();
}
