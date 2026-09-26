using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mandys.Infrastructure.Persistence.Records;

/// <summary>
/// Persistence record for the combo dishes table. A combo line: one
/// <see cref="ComboRecord"/> includes one <see cref="DishRecord"/> in the
/// given <see cref="Quantity"/>.
/// </summary>
public class ComboDishRecord : Record
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    /// <summary>
    /// Combo this line belongs to (inverse of
    /// <see cref="ComboRecord.ComboDishes"/>).
    /// </summary>
    [Required]
    public int ComboId { get; set; }
    public ComboRecord Combo { get; set; } = null!;

    /// <summary>
    /// Dish included in the combo (inverse of
    /// <see cref="DishRecord.ComboDishes"/>).
    /// </summary>
    [Required]
    public int DishId { get; set; }
    public DishRecord Dish { get; set; } = null!;

    /// <summary>
    /// How many portions of the dish the combo includes.
    /// </summary>
    [Required]
    public decimal Quantity { get; set; }
}
