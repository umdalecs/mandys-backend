using Mandys.Domain;
using Mandys.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Mandys;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : DbContext(options)
{
    public DbSet<UserRecord> Users { get; set; }
    public DbSet<BranchRecord> Branches { get; set; }
    public DbSet<ProductRecord> Products { get; set; }
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
            // Many users belong to one branch. Optional so existing/central
            // users without a branch keep working; deleting a branch keeps
            // the users and clears their BranchId.
            entity.HasOne(u => u.Branch)
                .WithMany(b => b.Users)
                .HasForeignKey(u => u.BranchId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasIndex(u => u.BranchId);

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

        modelBuilder.Entity<ProductRecord>(entity =>
        {
            // Note: Make sure to only use static data here
            var seededAt = new DateTime(2026, 9, 9, 0, 0, 0, DateTimeKind.Utc);
            entity.HasData(
                new { Id = 1, Description = "Jitomate saladet", IsSupply = true, Price = "28.50", MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 2, Description = "Lechuga romana", IsSupply = true, Price = "18.00", MeasureUnit = "pieza", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 3, Description = "Cebolla blanca", IsSupply = true, Price = "32.00", MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 4, Description = "Papa blanca para freír", IsSupply = true, Price = "26.00", MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 5, Description = "Carne molida de res 80/20", IsSupply = true, Price = "165.00", MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 6, Description = "Pechuga de pollo sin hueso", IsSupply = true, Price = "120.00", MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 7, Description = "Pan para hamburguesa con ajonjolí", IsSupply = true, Price = "8.50", MeasureUnit = "pieza", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 8, Description = "Queso americano en rebanadas", IsSupply = true, Price = "145.00", MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 9, Description = "Tocino ahumado", IsSupply = true, Price = "210.00", MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 10, Description = "Mayonesa", IsSupply = true, Price = "68.00", MeasureUnit = "litro", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 11, Description = "Catsup", IsSupply = true, Price = "45.00", MeasureUnit = "litro", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 12, Description = "Mostaza amarilla", IsSupply = true, Price = "42.00", MeasureUnit = "litro", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 13, Description = "Salsa BBQ", IsSupply = true, Price = "55.00", MeasureUnit = "litro", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 14, Description = "Aderezo ranch", IsSupply = true, Price = "72.00", MeasureUnit = "litro", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 15, Description = "Aceite vegetal", IsSupply = true, Price = "48.00", MeasureUnit = "litro", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 16, Description = "Sal refinada", IsSupply = true, Price = "14.00", MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 17, Description = "Refresco de cola en lata", IsSupply = true, Price = "16.50", MeasureUnit = "pieza", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 18, Description = "Servilletas", IsSupply = true, Price = "38.00", MeasureUnit = "paquete", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false }
            );
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
