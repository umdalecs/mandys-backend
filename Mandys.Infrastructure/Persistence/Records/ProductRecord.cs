using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Mandys.Domain;

namespace Mandys.Infrastructure.Persistence.Records;

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
    public decimal? CostPrice { get; set; }
    [Required]
    public decimal? SalePrice { get; set; }
    [Required]
    [MaxLength(50)]
    public string MeasureUnit { get; set; } = string.Empty;
    public ICollection<DishProductRecord> DishProducts { get; set; } = new List<DishProductRecord>();
    public ICollection<ComboProductRecord> ComboProducts { get; set; } = new List<ComboProductRecord>();
}
