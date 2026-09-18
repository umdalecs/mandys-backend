using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Mandys.Domain;

namespace Mandys.Infrastructure.Persistence;

/// <summary>
/// Persistence record for the users table. Mapped to/from
/// <see cref="User"/> by <see cref="UserRepository"/>.
/// </summary>
public class UserRecord : Record
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    [Required]
    public string FirstName { get; set; } = string.Empty;
    [Required]
    public string LastName { get; set; } = string.Empty;
    [Required]
    public string Email { get; set; } = string.Empty;
    [Required]
    [Column("password")]
    public string PasswordHash { get; set; } = string.Empty;
    [Required]
    public string Role { get; set; } = Roles.User;
}
