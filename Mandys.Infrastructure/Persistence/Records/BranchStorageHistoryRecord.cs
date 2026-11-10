using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mandys.Infrastructure.Persistence.Records;

public class BranchStorageHistoryRecord : Record
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    [Required]
    public int BranchId { get; set; }
    public BranchRecord Branch { get; set; } = null!;
    [Required]
    [MaxLength(10)]
    public string Type { get; set; } = string.Empty;
    [MaxLength(50)]
    public string? Folio { get; set; }
    [MaxLength(150)]
    public string? SupplierName { get; set; }
    [MaxLength(255)]
    public string? Reason { get; set; }
    public ICollection<BranchStorageHistoryProductRecord> Products { get; set; } = new List<BranchStorageHistoryProductRecord>();
}
