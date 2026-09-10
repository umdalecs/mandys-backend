using Mandys.Domain;
using Mandys.Entities;
using Microsoft.EntityFrameworkCore;

namespace Mandys;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : DbContext(options)
{
    public DbSet<UserEntity> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSeeding((context, _) =>
        {

        });
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<UserEntity>(entity =>
        {
            entity.Property(u => u.Role).HasDefaultValue(Roles.User);

            // Note: Make sure to only use static data here
            entity.HasData(
                new
                {
                    Id = Guid.Parse("d1a7b9c3-4e56-4f89-a123-b456c789d012"),
                    Email = "admin@mandys.com",
                    Password = "$argon2id$v=19$m=16,t=2,p=1$YWRtaW5pc3RyYXRvcnNhbHQ$n/2qmo8rW3KVIHy7g2Y0XA",
                    FirstName = "Administrator",
                    LastName = "Administrator",
                    Role = Roles.Admin,
                    CreatedAt = new DateTime(2026, 9, 9).ToUniversalTime(),
                    UpdatedAt = new DateTime(2026, 9, 9).ToUniversalTime(),
                    IsDeleted = false,
                }
            );
        });

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(Entity).IsAssignableFrom(entityType.ClrType))
            {
                var parameter = System.Linq.Expressions.Expression.Parameter(entityType.ClrType, "e");
                var property = System.Linq.Expressions.Expression.Property(parameter, nameof(Entity.IsDeleted));
                var filter = System.Linq.Expressions.Expression.Lambda(System.Linq.Expressions.Expression.Not(property), parameter);
                entityType.SetQueryFilter(filter);
            }
        }
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries<Entity>())
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