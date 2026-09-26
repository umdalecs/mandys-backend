using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mandys.Infrastructure.Persistence.Records;

/// <summary>
/// Persistence record for the dish products table. Mapped to/from
/// <see cref="DishProduct"/> by <see cref="Mappers.DishProductMapper"/>.
/// A recipe line: one <see cref="DishRecord"/> uses one
/// <see cref="ProductRecord"/> in the given <see cref="Quantity"/>.
/// </summary>
public class DishProductRecord : Record
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    /// <summary>
    /// Dish this recipe line belongs to (inverse of
    /// <see cref="DishRecord.DishProducts"/>).
    /// </summary>
    [Required]
    public int DishId { get; set; }
    public DishRecord Dish { get; set; } = null!;

    /// <summary>
    /// Product used by this recipe line (inverse of
    /// <see cref="ProductRecord.DishProducts"/>).
    /// </summary>
    [Required]
    public int ProductId { get; set; }
    public ProductRecord Product { get; set; } = null!;

    /// <summary>
    /// Amount of the product used in the dish, in the product's measure unit.
    /// </summary>
    [Required]
    public decimal Quantity { get; set; }
}
