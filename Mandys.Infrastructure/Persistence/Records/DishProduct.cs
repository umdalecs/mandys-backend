using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mandys.Infrastructure.Persistence.Records;

public class DishProductRecord : Record
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    [Required]
    public int DishId { get; set; }
    public DishRecord Dish { get; set; } = null!;
    public int ProductId { get; set; }
    public ProductRecord Product { get; set; } = null!;
    [Required]
    public decimal Quantity { get; set; }
}
