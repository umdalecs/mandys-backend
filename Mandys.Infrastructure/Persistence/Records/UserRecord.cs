using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Mandys.Domain;

namespace Mandys.Infrastructure.Persistence.Records;

public class UserRecord : Record
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    [Required]
    [MaxLength(50)]
    public string FirstName { get; set; } = string.Empty;
    [Required]
    [MaxLength(50)]
    public string FatherLastName { get; set; } = string.Empty;
    [MaxLength(50)]
    public string? MotherLastName { get; set; }
    [Required]
    [MaxLength(50)]
    public string Email { get; set; } = string.Empty;
    [Column("password")]
    public string PasswordHash { get; set; } = string.Empty;
    [Required]
    [MaxLength(50)]
    public string Role { get; set; } = Roles.Customer;
    public int? BranchId { get; set; }
    [ForeignKey(nameof(BranchId))]
    public BranchRecord? Branch { get; set; }
}
