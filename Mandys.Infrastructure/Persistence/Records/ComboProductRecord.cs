using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mandys.Infrastructure.Persistence.Records;

public class ComboProductRecord : Record
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    [Required]
    public int ComboId { get; set; }
    public ComboRecord Combo { get; set; } = null!;
    [Required]
    public int ProductId { get; set; }
    public ProductRecord Product { get; set; } = null!;
    [Required]
    public decimal Quantity { get; set; }
}
