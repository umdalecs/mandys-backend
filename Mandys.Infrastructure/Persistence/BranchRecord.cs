using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Mandys.Domain;

namespace Mandys.Infrastructure.Persistence;

/// <summary>
/// Persistence record for the branches table. Mapped to/from
/// <see cref="Branch"/> by <see cref="BranchMapper"/>.
/// One branch has many <see cref="UserRecord"/> rows; the FK lives on
/// the user side (<see cref="UserRecord.BranchId"/>).
/// </summary>
public class BranchRecord : Record
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    [Required]
    public string Name { get; set; } = string.Empty;
    [Required]
    public string Address { get; set; } = string.Empty;
    [Required]
    public string WarehouseOnly { get; set; } = string.Empty;

    /// <summary>
    /// Users assigned to this branch (inverse of <see cref="UserRecord.Branch"/>).
    /// </summary>
    public ICollection<UserRecord> Users { get; set; } = new List<UserRecord>();
}
