using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Mandys.Domain;

namespace Mandys.Infrastructure.Persistence.Records;

public class BranchRecord : Record
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;
    [Required]
    [MaxLength(50)]
    public string Address { get; set; } = string.Empty;
    [Required]
    public bool WarehouseOnly { get; set; } = false;
    public ICollection<UserRecord> Users { get; set; } = [];
}
