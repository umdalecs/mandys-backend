namespace Mandys.Domain;

/// <summary>
/// Base class for domain entities. Persistence concerns (tables, audit
/// columns, soft-delete flags) live on records in Infrastructure.
/// </summary>
public abstract class Entity
{
    public Guid Id { get; protected set; }

    protected Entity(Guid id)
    {
        Id = id;
    }
}
