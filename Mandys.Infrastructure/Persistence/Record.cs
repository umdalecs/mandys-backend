namespace Mandys.Infrastructure.Persistence;

/// <summary>
/// Base class for persistence records (table rows). Audit columns and the
/// soft-delete flag are storage concerns; the domain entity stays clean.
/// </summary>
public abstract class Record
{
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? DeletedAt { get; set; }
    public bool IsDeleted { get; set; }
}
