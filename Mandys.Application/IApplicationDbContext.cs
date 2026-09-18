using Mandys.Entities;
using Microsoft.EntityFrameworkCore;

namespace Mandys;

/// <summary>
/// Persistence abstraction owned by Application and implemented by
/// Infrastructure, so application services never depend on EF Core directly.
/// </summary>
public interface IApplicationDbContext
{
    DbSet<UserEntity> Users { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
