using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mandys.Infrastructure.Persistence.Records;

public class BranchStorageRecord : Record
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    [Required]
    public int BranchId { get; set; }
    public BranchRecord Branch { get; set; } = null!;
    [Required]
    public int ProductId { get; set; }
    public ProductRecord Product { get; set; } = null!;
    [Required]
    public decimal MinimumStock { get; set; }
    [Required]
    public decimal CurrentStock { get; set; }
}
