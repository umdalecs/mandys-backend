using System.ComponentModel.DataAnnotations;
using Mandys.Domain;

namespace Mandys.Entities;

public class UserEntity : Entity
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
    public string Password { get; set; } = string.Empty;
    [Required]
    public string Role { get; set; } = Roles.User;
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiryTime { get; set; }
}