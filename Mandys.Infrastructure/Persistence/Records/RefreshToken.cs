using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mandys.Infrastructure.Persistence.Records;

/// <summary>
/// Refresh token grant row. Deliberately not a <see cref="Record"/>: token
/// grants carry no audit timeline, only issuance, expiry and revocation.
/// Only the SHA-256 hash is stored; the raw value is shown to the client
/// once at issuance.
/// </summary>
[Table("refresh_tokens")]
public class RefreshToken
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    public int UserId { get; set; }

    [Required]
    public string TokenHash { get; set; } = string.Empty;

    public DateTime ExpiresAt { get; set; }

    public DateTime? RevokedAt { get; set; }
}
