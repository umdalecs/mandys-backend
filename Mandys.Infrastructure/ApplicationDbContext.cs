using Mandys.Domain;
using Mandys.Infrastructure.Persistence.Records;
using Microsoft.EntityFrameworkCore;

namespace Mandys.Infrastructure;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : DbContext(options)
{
    public DbSet<UserRecord> Users { get; set; }
    public DbSet<BranchRecord> Branches { get; set; }
    public DbSet<ProductRecord> Products { get; set; }
    public DbSet<DishRecord> Dishes { get; set; }
    public DbSet<DishProductRecord> DishProducts { get; set; }
    public DbSet<ComboRecord> Combos { get; set; }
    public DbSet<ComboDishRecord> ComboDishes { get; set; }
    public DbSet<ComboProductRecord> ComboProducts { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<LogRecord> Logs { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSeeding((context, _) =>
        {

        });
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        _ = modelBuilder.Entity<UserRecord>(entity =>
        {
            entity.HasOne(u => u.Branch)
                .WithMany(b => b.Users)
                .HasForeignKey(u => u.BranchId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasIndex(u => u.BranchId);

            entity.HasIndex(u => u.Email).IsUnique();

            const string hash = "$argon2id$v=19$m=16,t=2,p=1$bWFuZHlzcG9z$C8kgZO6/V+MkFbWFE5pl9Q";
            var seededAt = new DateTime(2026, 9, 9, 0, 0, 0, DateTimeKind.Utc);

            entity.HasData(
                new
                {
                    Id = 1,
                    Email = "admin@mandyspos.com",
                    PasswordHash = hash,
                    FirstName = "Administrador",
                    FatherLastName = "Sistema",
                    Role = Roles.Administrator,
                    BranchId = (int?)null,
                    CreatedAt = seededAt,
                    UpdatedAt = seededAt,
                },
                new
                {
                    Id = 2,
                    Email = "cliente@mandyspos.com",
                    PasswordHash = hash,
                    FirstName = "Cliente",
                    FatherLastName = "Demo",
                    Role = Roles.Customer,
                    BranchId = (int?)null,
                    CreatedAt = seededAt,
                    UpdatedAt = seededAt,
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

        modelBuilder.Entity<DishProductRecord>(entity =>
        {
            entity.HasOne(dp => dp.Dish)
                .WithMany(d => d.DishProducts)
                .HasForeignKey(dp => dp.DishId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(dp => dp.Product)
                .WithMany(p => p.DishProducts)
                .HasForeignKey(dp => dp.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(dp => dp.DishId);
            entity.HasIndex(dp => dp.ProductId);
        });

        modelBuilder.Entity<ComboDishRecord>(entity =>
        {
            entity.HasOne(cd => cd.Combo)
                .WithMany(c => c.ComboDishes)
                .HasForeignKey(cd => cd.ComboId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(cd => cd.Dish)
                .WithMany(d => d.ComboDishes)
                .HasForeignKey(cd => cd.DishId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(cd => cd.ComboId);
            entity.HasIndex(cd => cd.DishId);
        });

        modelBuilder.Entity<ComboProductRecord>(entity =>
        {
            entity.HasOne(cp => cp.Combo)
                .WithMany(c => c.ComboProducts)
                .HasForeignKey(cp => cp.ComboId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(cp => cp.Product)
                .WithMany(p => p.ComboProducts)
                .HasForeignKey(cp => cp.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(cp => cp.ComboId);
            entity.HasIndex(cp => cp.ProductId);
        });

        modelBuilder.Entity<ProductRecord>(entity =>
        {
            entity.HasIndex(p => p.Description).IsUnique();
        });

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(Record).IsAssignableFrom(entityType.ClrType))
            {
                var parameter = System.Linq.Expressions.Expression.Parameter(entityType.ClrType, "e");
                var property = System.Linq.Expressions.Expression.Property(parameter, nameof(Record.DeletedAt));
                var nullConstant = System.Linq.Expressions.Expression.Constant(null, typeof(DateTime?));
                var filter = System.Linq.Expressions.Expression.Lambda(System.Linq.Expressions.Expression.Equal(property, nullConstant), parameter);
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
                entry.Entity.DeletedAt = DateTime.UtcNow;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}
