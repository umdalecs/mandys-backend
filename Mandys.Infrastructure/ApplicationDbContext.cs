using Mandys.Domain;
using Mandys.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Mandys;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : DbContext(options)
{
    public DbSet<UserRecord> Users { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSeeding((context, _) =>
        {

        });
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<UserRecord>(entity =>
        {
            // Note: Make sure to only use static data here
            entity.HasData(
                new
                {
                    Id = 1,
                    Email = "admin@mandys.com",
                    PasswordHash = "$argon2id$v=19$m=16,t=2,p=1$YWRtaW5pc3RyYXRvcnNhbHQ$n/2qmo8rW3KVIHy7g2Y0XA",
                    FirstName = "Administrator",
                    LastName = "Administrator",
                    Role = Roles.Administrator,
                    CreatedAt = new DateTime(2026, 9, 9, 0, 0, 0, DateTimeKind.Utc),
                    UpdatedAt = new DateTime(2026, 9, 9, 0, 0, 0, DateTimeKind.Utc),
                    IsDeleted = false,
                }
            );
        });

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasOne<UserRecord>()
                .WithMany()
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(r => r.TokenHash).IsUnique();
        });

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(Record).IsAssignableFrom(entityType.ClrType))
            {
                var parameter = System.Linq.Expressions.Expression.Parameter(entityType.ClrType, "e");
                var property = System.Linq.Expressions.Expression.Property(parameter, nameof(Record.IsDeleted));
                var filter = System.Linq.Expressions.Expression.Lambda(System.Linq.Expressions.Expression.Not(property), parameter);
                entityType.SetQueryFilter(filter);
            }
        }
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries<Record>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = DateTime.UtcNow;
                entry.Entity.UpdatedAt = DateTime.UtcNow;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = DateTime.UtcNow;
            }
            else if (entry.State == EntityState.Deleted)
            {
                entry.State = EntityState.Modified;
                entry.Entity.IsDeleted = true;
                entry.Entity.DeletedAt = DateTime.UtcNow;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}
