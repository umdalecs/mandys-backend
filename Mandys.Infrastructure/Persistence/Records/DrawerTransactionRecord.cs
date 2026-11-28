using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mandys.Infrastructure.Persistence.Records;

public class DrawerTransactionRecord : Record
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    [Required]
    public int BranchId { get; set; }
    public BranchRecord Branch { get; set; } = null!;
    [Required]
    public int UserId { get; set; }
    public UserRecord User { get; set; } = null!;
    public int? OrderId { get; set; }
    public OrderRecord? Order { get; set; }
    [Required]
    [MaxLength(30)]
    public string Type { get; set; } = string.Empty;
    [Required]
    public decimal Amount { get; set; }
    public decimal? AmountPaid { get; set; }
    public decimal? ChangeAmount { get; set; }
    [MaxLength(255)]
    public string? Notes { get; set; }
}
