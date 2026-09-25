using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mandys.Infrastructure.Persistence;

/// <summary>
/// Persistence record for the dishes table. Mapped to/from
/// <see cref="Dish"/> by <see cref="DishMapper"/>.
/// One dish has many <see cref="DishProductRecord"/> recipe lines.
/// </summary>
public class DishRecord : Record
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
    /// Recipe lines of this dish (inverse of
    /// <see cref="DishProductRecord.Dish"/>).
    /// </summary>
    public ICollection<DishProductRecord> DishProducts { get; set; } = new List<DishProductRecord>();
}
