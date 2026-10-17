using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mandys.Infrastructure.Persistence.Records;

public class ComboDishRecord : Record
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    [Required]
    public int ComboId { get; set; }
    public ComboRecord Combo { get; set; } = null!;
    [Required]
    public int DishId { get; set; }
    public DishRecord Dish { get; set; } = null!;
    [Required]
    public decimal Quantity { get; set; }
}
