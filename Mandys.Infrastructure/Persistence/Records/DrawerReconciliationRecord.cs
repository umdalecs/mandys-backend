using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mandys.Infrastructure.Persistence.Records;

public class DrawerReconciliationRecord : Record
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

    [Required]
    public int PerformedByUserId { get; set; }
    public UserRecord PerformedByUser { get; set; } = null!;

    [Required]
    public decimal ExpectedAmount { get; set; }

    [Required]
    public decimal CountedAmount { get; set; }

    [Required]
    public decimal Discrepancy { get; set; }

    [Required]
    public bool HasDiscrepancy { get; set; }

    [Required]
    [MaxLength(20)]
    public string Status { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Reason { get; set; }

    public int? ResolvedByUserId { get; set; }
    public UserRecord? ResolvedByUser { get; set; }

    public DateTime? ResolvedAt { get; set; }

    public int? AdjustmentTransactionId { get; set; }
    public DrawerTransactionRecord? AdjustmentTransaction { get; set; }
}
