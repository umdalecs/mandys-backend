using Mandys.Domain;
using Mandys.Infrastructure.Persistence.Records;
using Microsoft.EntityFrameworkCore;

namespace Mandys;

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

            // Credential-less users keep a null email, and Postgres allows
            // any number of nulls in a unique index, so this only stops two
            // login handles from colliding.
            entity.HasIndex(u => u.Email).IsUnique();

            // Note: Make sure to only use static data here
            // One account per role so every role can be exercised locally.
            // plain password for all of them: mandyspos
            const string passwordHash = "$argon2id$v=19$m=16,t=2,p=1$bWFuZHlzcG9z$C8kgZO6/V+MkFbWFE5pl9Q";
            var seededAt = new DateTime(2026, 9, 9, 0, 0, 0, DateTimeKind.Utc);

            // Every role but administrador and cliente belongs to the seeded
            // branch, so those rows carry BranchId 1.
            entity.HasData(
                new
                {
                    Id = 1,
                    Email = "admin@mandyspos.com",
                    PasswordHash = passwordHash,
                    FirstName = "Administrador",
                    LastName = "Sistema",
                    Role = Roles.Administrator,
                    BranchId = (int?)null,
                    CreatedAt = seededAt,
                    UpdatedAt = seededAt,
                    IsDeleted = false,
                },
                new
                {
                    Id = 2,
                    Email = "operaciones@mandyspos.com",
                    PasswordHash = passwordHash,
                    FirstName = "Gerente",
                    LastName = "Operaciones",
                    Role = Roles.OpChief,
                    BranchId = (int?)1,
                    CreatedAt = seededAt,
                    UpdatedAt = seededAt,
                    IsDeleted = false,
                },
                new
                {
                    Id = 3,
                    Email = "almacen.central@mandyspos.com",
                    PasswordHash = passwordHash,
                    FirstName = "Encargado",
                    LastName = "Almacén Central",
                    Role = Roles.CentralWarehouseChief,
                    BranchId = (int?)1,
                    CreatedAt = seededAt,
                    UpdatedAt = seededAt,
                    IsDeleted = false,
                },
                new
                {
                    Id = 4,
                    Email = "almacen@mandyspos.com",
                    PasswordHash = passwordHash,
                    FirstName = "Encargado",
                    LastName = "Almacén",
                    Role = Roles.WarehouseChief,
                    BranchId = (int?)1,
                    CreatedAt = seededAt,
                    UpdatedAt = seededAt,
                    IsDeleted = false,
                },
                new
                {
                    Id = 5,
                    Email = "caja@mandyspos.com",
                    PasswordHash = passwordHash,
                    FirstName = "Cajero",
                    LastName = "Sucursal",
                    Role = Roles.Cashier,
                    BranchId = (int?)1,
                    CreatedAt = seededAt,
                    UpdatedAt = seededAt,
                    IsDeleted = false,
                },
                new
                {
                    Id = 6,
                    Email = "cocina@mandyspos.com",
                    PasswordHash = passwordHash,
                    FirstName = "Jefe",
                    LastName = "Cocina",
                    Role = Roles.KitchenChief,
                    BranchId = (int?)1,
                    CreatedAt = seededAt,
                    UpdatedAt = seededAt,
                    IsDeleted = false,
                },
                new
                {
                    Id = 7,
                    Email = "sucursal@mandyspos.com",
                    PasswordHash = passwordHash,
                    FirstName = "Gerente",
                    LastName = "Sucursal",
                    Role = Roles.BranchChief,
                    BranchId = (int?)1,
                    CreatedAt = seededAt,
                    UpdatedAt = seededAt,
                    IsDeleted = false,
                },
                new
                {
                    Id = 8,
                    Email = "cliente@mandyspos.com",
                    PasswordHash = passwordHash,
                    FirstName = "Cliente",
                    LastName = "Demo",
                    Role = Roles.Customer,
                    BranchId = (int?)null,
                    CreatedAt = seededAt,
                    UpdatedAt = seededAt,
                    IsDeleted = false,
                }
            );
        });

        modelBuilder.Entity<BranchRecord>(entity =>
        {
            // Note: Make sure to only use static data here
            var seededAt = new DateTime(2026, 9, 9, 0, 0, 0, DateTimeKind.Utc);
            entity.HasData(
                new
                {
                    Id = 1,
                    Name = "Sucursal Culiacán Centro",
                    Address = "Av. Río Presa 1200, Centro, Culiacán",
                    WarehouseOnly = false,
                    CreatedAt = seededAt,
                    UpdatedAt = seededAt,
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

        modelBuilder.Entity<DishRecord>(entity =>
        {
            // Note: Make sure to only use static data here
            var seededAt = new DateTime(2026, 9, 9, 0, 0, 0, DateTimeKind.Utc);
            entity.HasData(
                new { Id = 1, Name = "Hamburguesa clásica", Price = 85m, CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 2, Name = "Hamburguesa doble", Price = 125m, CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 3, Name = "Hamburguesa de pollo", Price = 92m, CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 4, Name = "Taco al pastor", Price = 48m, CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 5, Name = "Taco de bistec", Price = 52m, CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 6, Name = "Burrito de carne", Price = 98m, CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 7, Name = "Quesadilla de pollo", Price = 78m, CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 8, Name = "Ensalada César", Price = 95m, CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 9, Name = "Churros con cajeta", Price = 65m, CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 10, Name = "Sopa de tortilla", Price = 55m, CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false }
            );
        });

        modelBuilder.Entity<DishProductRecord>(entity =>
        {
            // Many recipe lines belong to one dish; deleting a dish deletes
            // its recipe lines.
            entity.HasOne(dp => dp.Dish)
                .WithMany(d => d.DishProducts)
                .HasForeignKey(dp => dp.DishId)
                .OnDelete(DeleteBehavior.Cascade);

            // Many recipe lines reference one product; deleting a product
            // deletes its recipe lines.
            entity.HasOne(dp => dp.Product)
                .WithMany(p => p.DishProducts)
                .HasForeignKey(dp => dp.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(dp => dp.DishId);
            entity.HasIndex(dp => dp.ProductId);
            // Note: Make sure to only use static data here
            var seededAt = new DateTime(2026, 9, 9, 0, 0, 0, DateTimeKind.Utc);
            entity.HasData(
                new { Id = 1, DishId = 1, ProductId = 61, Quantity = 1m, CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 2, DishId = 1, ProductId = 36, Quantity = 0.15m, CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 3, DishId = 1, ProductId = 73, Quantity = 0.04m, CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 4, DishId = 1, ProductId = 4, Quantity = 2m, CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 5, DishId = 1, ProductId = 1, Quantity = 0.05m, CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 6, DishId = 1, ProductId = 5, Quantity = 0.03m, CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 7, DishId = 1, ProductId = 87, Quantity = 0.02m, CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 8, DishId = 1, ProductId = 88, Quantity = 0.01m, CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 9, DishId = 1, ProductId = 171, Quantity = 0.02m, CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 10, DishId = 2, ProductId = 62, Quantity = 1m, CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 11, DishId = 2, ProductId = 36, Quantity = 0.3m, CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 12, DishId = 2, ProductId = 75, Quantity = 0.06m, CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 13, DishId = 2, ProductId = 73, Quantity = 0.04m, CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 14, DishId = 2, ProductId = 1, Quantity = 0.08m, CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 15, DishId = 2, ProductId = 5, Quantity = 0.04m, CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 16, DishId = 2, ProductId = 44, Quantity = 0.05m, CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 17, DishId = 2, ProductId = 90, Quantity = 0.03m, CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 18, DishId = 3, ProductId = 64, Quantity = 1m, CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 19, DishId = 3, ProductId = 43, Quantity = 0.18m, CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 20, DishId = 3, ProductId = 78, Quantity = 0.03m, CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 21, DishId = 3, ProductId = 3, Quantity = 2m, CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 22, DishId = 3, ProductId = 9, Quantity = 0.04m, CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 23, DishId = 3, ProductId = 86, Quantity = 0.02m, CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 24, DishId = 4, ProductId = 69, Quantity = 2m, CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 25, DishId = 4, ProductId = 38, Quantity = 0.15m, CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 26, DishId = 4, ProductId = 10, Quantity = 0.02m, CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 27, DishId = 4, ProductId = 13, Quantity = 0.05m, CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 28, DishId = 4, ProductId = 2, Quantity = 0.05m, CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 29, DishId = 4, ProductId = 6, Quantity = 0.03m, CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 30, DishId = 4, ProductId = 19, Quantity = 0.01m, CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 31, DishId = 5, ProductId = 69, Quantity = 2m, CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 32, DishId = 5, ProductId = 39, Quantity = 0.12m, CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 33, DishId = 5, ProductId = 7, Quantity = 0.08m, CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 34, DishId = 5, ProductId = 32, Quantity = 0.03m, CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 35, DishId = 5, ProductId = 105, Quantity = 0.002m, CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 36, DishId = 5, ProductId = 107, Quantity = 0.001m, CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 37, DishId = 6, ProductId = 68, Quantity = 1m, CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 38, DishId = 6, ProductId = 36, Quantity = 0.18m, CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 39, DishId = 6, ProductId = 80, Quantity = 0.01m, CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 40, DishId = 6, ProductId = 172, Quantity = 0.08m, CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 41, DishId = 6, ProductId = 22, Quantity = 0.03m, CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 42, DishId = 6, ProductId = 14, Quantity = 0.03m, CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 43, DishId = 6, ProductId = 82, Quantity = 0.05m, CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 44, DishId = 6, ProductId = 77, Quantity = 0.04m, CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 45, DishId = 6, ProductId = 91, Quantity = 0.02m, CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 46, DishId = 7, ProductId = 68, Quantity = 2m, CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 47, DishId = 7, ProductId = 42, Quantity = 0.15m, CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 48, DishId = 7, ProductId = 73, Quantity = 0.08m, CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 49, DishId = 7, ProductId = 17, Quantity = 0.04m, CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 50, DishId = 7, ProductId = 92, Quantity = 0.03m, CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 51, DishId = 7, ProductId = 125, Quantity = 0.01m, CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 52, DishId = 8, ProductId = 22, Quantity = 0.08m, CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 53, DishId = 8, ProductId = 23, Quantity = 0.06m, CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 54, DishId = 8, ProductId = 74, Quantity = 0.04m, CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 55, DishId = 8, ProductId = 93, Quantity = 0.05m, CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 56, DishId = 8, ProductId = 171, Quantity = 0.01m, CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 57, DishId = 8, ProductId = 71, Quantity = 0.03m, CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 58, DishId = 8, ProductId = 105, Quantity = 0.002m, CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 59, DishId = 8, ProductId = 107, Quantity = 0.001m, CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 60, DishId = 9, ProductId = 163, Quantity = 0.1m, CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 61, DishId = 9, ProductId = 132, Quantity = 0.05m, CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 62, DishId = 9, ProductId = 105, Quantity = 0.002m, CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 63, DishId = 9, ProductId = 175, Quantity = 0.08m, CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 64, DishId = 10, ProductId = 1, Quantity = 0.2m, CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 65, DishId = 10, ProductId = 69, Quantity = 2m, CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 66, DishId = 10, ProductId = 5, Quantity = 0.06m, CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 67, DishId = 10, ProductId = 19, Quantity = 0.01m, CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 68, DishId = 10, ProductId = 125, Quantity = 0.02m, CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 69, DishId = 10, ProductId = 80, Quantity = 0.01m, CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 70, DishId = 10, ProductId = 32, Quantity = 0.02m, CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false }
            );
        });

        modelBuilder.Entity<ComboRecord>(entity =>
        {
            // Note: Make sure to only use static data here
            var seededAt = new DateTime(2026, 9, 9, 0, 0, 0, DateTimeKind.Utc);
            entity.HasData(
                new { Id = 1, Name = "Combo Hamburguesa clásica", Price = 129m, CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 2, Name = "Combo Hamburguesa doble", Price = 169m, CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 3, Name = "Combo Hamburguesa de pollo", Price = 139m, CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 4, Name = "Combo Tacos al pastor", Price = 119m, CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 5, Name = "Combo Tacos de bistec", Price = 125m, CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 6, Name = "Combo Burrito", Price = 145m, CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 7, Name = "Combo Quesadillas", Price = 125m, CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 8, Name = "Combo Ensalada", Price = 119m, CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 9, Name = "Combo Postre", Price = 119m, CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 10, Name = "Combo Familiar", Price = 349m, CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false }
            );
        });

        modelBuilder.Entity<ComboDishRecord>(entity =>
        {
            // Many combo lines belong to one combo; deleting a combo deletes
            // its dish lines.
            entity.HasOne(cd => cd.Combo)
                .WithMany(c => c.ComboDishes)
                .HasForeignKey(cd => cd.ComboId)
                .OnDelete(DeleteBehavior.Cascade);

            // Many combo lines reference one dish; deleting a dish deletes
            // its combo lines.
            entity.HasOne(cd => cd.Dish)
                .WithMany(d => d.ComboDishes)
                .HasForeignKey(cd => cd.DishId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(cd => cd.ComboId);
            entity.HasIndex(cd => cd.DishId);
            // Note: Make sure to only use static data here
            var seededAt = new DateTime(2026, 9, 9, 0, 0, 0, DateTimeKind.Utc);
            entity.HasData(
                new { Id = 1, ComboId = 1, DishId = 1, Quantity = 1m, CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 2, ComboId = 2, DishId = 2, Quantity = 1m, CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 3, ComboId = 3, DishId = 3, Quantity = 1m, CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 4, ComboId = 4, DishId = 4, Quantity = 2m, CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 5, ComboId = 5, DishId = 5, Quantity = 2m, CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 6, ComboId = 6, DishId = 6, Quantity = 1m, CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 7, ComboId = 7, DishId = 7, Quantity = 2m, CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 8, ComboId = 8, DishId = 8, Quantity = 1m, CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 9, ComboId = 9, DishId = 9, Quantity = 1m, CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 10, ComboId = 10, DishId = 1, Quantity = 2m, CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 11, ComboId = 10, DishId = 3, Quantity = 2m, CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false }
            );
        });

        modelBuilder.Entity<ComboProductRecord>(entity =>
        {
            // Many combo lines belong to one combo; deleting a combo deletes
            // its product lines.
            entity.HasOne(cp => cp.Combo)
                .WithMany(c => c.ComboProducts)
                .HasForeignKey(cp => cp.ComboId)
                .OnDelete(DeleteBehavior.Cascade);

            // Many combo lines reference one product; deleting a product
            // deletes its combo lines.
            entity.HasOne(cp => cp.Product)
                .WithMany(p => p.ComboProducts)
                .HasForeignKey(cp => cp.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(cp => cp.ComboId);
            entity.HasIndex(cp => cp.ProductId);
            // Note: Make sure to only use static data here
            var seededAt = new DateTime(2026, 9, 9, 0, 0, 0, DateTimeKind.Utc);
            entity.HasData(
                new { Id = 1, ComboId = 1, ProductId = 201, Quantity = 1m, CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 2, ComboId = 1, ProductId = 210, Quantity = 1m, CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 3, ComboId = 2, ProductId = 202, Quantity = 1m, CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 4, ComboId = 2, ProductId = 210, Quantity = 1m, CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 5, ComboId = 3, ProductId = 203, Quantity = 1m, CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 6, ComboId = 3, ProductId = 210, Quantity = 1m, CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 7, ComboId = 4, ProductId = 201, Quantity = 1m, CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 8, ComboId = 5, ProductId = 202, Quantity = 1m, CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 9, ComboId = 6, ProductId = 204, Quantity = 1m, CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 10, ComboId = 6, ProductId = 211, Quantity = 1m, CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 11, ComboId = 7, ProductId = 205, Quantity = 1m, CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 12, ComboId = 8, ProductId = 203, Quantity = 1m, CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 13, ComboId = 9, ProductId = 209, Quantity = 1m, CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 14, ComboId = 9, ProductId = 215, Quantity = 1m, CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 15, ComboId = 10, ProductId = 201, Quantity = 4m, CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 16, ComboId = 10, ProductId = 210, Quantity = 2m, CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false }
            );
        });

        modelBuilder.Entity<ProductRecord>(entity =>
        {
            // Note: Make sure to only use static data here
            var seededAt = new DateTime(2026, 9, 9, 0, 0, 0, DateTimeKind.Utc);
            entity.HasData(
                new { Id = 1, Description = "Jitomate saladet", IsSupply = true, SalePrice = 28.50m, CostPrice = 0m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 2, Description = "Jitomate cherry", IsSupply = true, SalePrice = 55.00m, CostPrice = 0m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 3, Description = "Lechuga romana", IsSupply = true, SalePrice = 18.00m, CostPrice = 0m, MeasureUnit = "pieza", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 4, Description = "Lechuga iceberg", IsSupply = true, SalePrice = 22.00m, CostPrice = 0m, MeasureUnit = "pieza", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 5, Description = "Cebolla blanca", IsSupply = true, SalePrice = 32.00m, CostPrice = 0m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 6, Description = "Cebolla morada", IsSupply = true, SalePrice = 38.00m, CostPrice = 0m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 7, Description = "Cebolla cambray", IsSupply = true, SalePrice = 25.00m, CostPrice = 0m, MeasureUnit = "manojo", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 8, Description = "Papa blanca para freír", IsSupply = true, SalePrice = 26.00m, CostPrice = 0m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 9, Description = "Pepino", IsSupply = true, SalePrice = 24.00m, CostPrice = 0m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 10, Description = "Chile jalapeño", IsSupply = true, SalePrice = 35.00m, CostPrice = 0m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 11, Description = "Aguacate hass", IsSupply = true, SalePrice = 85.00m, CostPrice = 0m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 12, Description = "Limón sin semilla", IsSupply = true, SalePrice = 30.00m, CostPrice = 0m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 13, Description = "Cilantro fresco", IsSupply = true, SalePrice = 12.00m, CostPrice = 0m, MeasureUnit = "manojo", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 14, Description = "Zanahoria", IsSupply = true, SalePrice = 22.00m, CostPrice = 0m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 15, Description = "Champiñón blanco", IsSupply = true, SalePrice = 95.00m, CostPrice = 0m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 16, Description = "Pimiento morrón verde", IsSupply = true, SalePrice = 48.00m, CostPrice = 0m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 17, Description = "Pimiento morrón rojo", IsSupply = true, SalePrice = 62.00m, CostPrice = 0m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 18, Description = "Apio", IsSupply = true, SalePrice = 28.00m, CostPrice = 0m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 19, Description = "Ajo fresco", IsSupply = true, SalePrice = 110.00m, CostPrice = 0m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 20, Description = "Elote amarillo", IsSupply = true, SalePrice = 9.00m, CostPrice = 0m, MeasureUnit = "pieza", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 21, Description = "Calabacita", IsSupply = true, SalePrice = 26.00m, CostPrice = 0m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 22, Description = "Espinaca baby", IsSupply = true, SalePrice = 120.00m, CostPrice = 0m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 23, Description = "Col blanca", IsSupply = true, SalePrice = 20.00m, CostPrice = 0m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 24, Description = "Rábano", IsSupply = true, SalePrice = 24.00m, CostPrice = 0m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 25, Description = "Nopal limpio", IsSupply = true, SalePrice = 30.00m, CostPrice = 0m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 26, Description = "Piña miel", IsSupply = true, SalePrice = 25.00m, CostPrice = 0m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 27, Description = "Mango ataulfo", IsSupply = true, SalePrice = 40.00m, CostPrice = 0m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 28, Description = "Fresa", IsSupply = true, SalePrice = 70.00m, CostPrice = 0m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 29, Description = "Plátano tabasco", IsSupply = true, SalePrice = 22.00m, CostPrice = 0m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 30, Description = "Manzana roja", IsSupply = true, SalePrice = 45.00m, CostPrice = 0m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 31, Description = "Naranja para jugo", IsSupply = true, SalePrice = 20.00m, CostPrice = 0m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 32, Description = "Perejil fresco", IsSupply = true, SalePrice = 12.00m, CostPrice = 0m, MeasureUnit = "manojo", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 33, Description = "Epazote fresco", IsSupply = true, SalePrice = 10.00m, CostPrice = 0m, MeasureUnit = "manojo", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 34, Description = "Chayote", IsSupply = true, SalePrice = 18.00m, CostPrice = 0m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 35, Description = "Betabel", IsSupply = true, SalePrice = 26.00m, CostPrice = 0m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 36, Description = "Carne molida de res 80/20", IsSupply = true, SalePrice = 165.00m, CostPrice = 0m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 37, Description = "Carne molida de res 90/10", IsSupply = true, SalePrice = 185.00m, CostPrice = 0m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 38, Description = "Arrachera marinada", IsSupply = true, SalePrice = 240.00m, CostPrice = 0m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 39, Description = "Milanesa de res", IsSupply = true, SalePrice = 190.00m, CostPrice = 0m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 40, Description = "Pechuga de pollo sin hueso", IsSupply = true, SalePrice = 120.00m, CostPrice = 0m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 41, Description = "Muslo de pollo sin hueso", IsSupply = true, SalePrice = 95.00m, CostPrice = 0m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 42, Description = "Tiras de pollo", IsSupply = true, SalePrice = 115.00m, CostPrice = 0m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 43, Description = "Milanesa de pollo", IsSupply = true, SalePrice = 125.00m, CostPrice = 0m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 44, Description = "Tocino ahumado", IsSupply = true, SalePrice = 210.00m, CostPrice = 0m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 45, Description = "Jamón de pavo", IsSupply = true, SalePrice = 130.00m, CostPrice = 0m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 46, Description = "Chuleta ahumada", IsSupply = true, SalePrice = 150.00m, CostPrice = 0m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 47, Description = "Salchicha para asar", IsSupply = true, SalePrice = 90.00m, CostPrice = 0m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 48, Description = "Longaniza", IsSupply = true, SalePrice = 110.00m, CostPrice = 0m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 49, Description = "Pepperoni", IsSupply = true, SalePrice = 220.00m, CostPrice = 0m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 50, Description = "Huevo fresco", IsSupply = true, SalePrice = 52.00m, CostPrice = 0m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 51, Description = "Huevo líquido pasteurizado", IsSupply = true, SalePrice = 65.00m, CostPrice = 0m, MeasureUnit = "litro", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 52, Description = "Filete de pescado empanizado", IsSupply = true, SalePrice = 140.00m, CostPrice = 0m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 53, Description = "Camarón mediano sin cáscara", IsSupply = true, SalePrice = 260.00m, CostPrice = 0m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 54, Description = "Atún enlatado", IsSupply = true, SalePrice = 32.00m, CostPrice = 0m, MeasureUnit = "lata", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 55, Description = "Costilla de cerdo", IsSupply = true, SalePrice = 145.00m, CostPrice = 0m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 56, Description = "Pulled pork", IsSupply = true, SalePrice = 175.00m, CostPrice = 0m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 57, Description = "Salami", IsSupply = true, SalePrice = 200.00m, CostPrice = 0m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 58, Description = "Chorizo argentino", IsSupply = true, SalePrice = 135.00m, CostPrice = 0m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 59, Description = "Pavo molido", IsSupply = true, SalePrice = 150.00m, CostPrice = 0m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 60, Description = "Lomo de cerdo", IsSupply = true, SalePrice = 130.00m, CostPrice = 0m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 61, Description = "Pan para hamburguesa con ajonjolí", IsSupply = true, SalePrice = 8.50m, CostPrice = 0m, MeasureUnit = "pieza", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 62, Description = "Pan brioche para hamburguesa", IsSupply = true, SalePrice = 11.00m, CostPrice = 0m, MeasureUnit = "pieza", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 63, Description = "Pan integral para hamburguesa", IsSupply = true, SalePrice = 10.00m, CostPrice = 0m, MeasureUnit = "pieza", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 64, Description = "Pan de papa para hamburguesa", IsSupply = true, SalePrice = 12.00m, CostPrice = 0m, MeasureUnit = "pieza", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 65, Description = "Pan para hot dog", IsSupply = true, SalePrice = 7.50m, CostPrice = 0m, MeasureUnit = "pieza", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 66, Description = "Pan telera", IsSupply = true, SalePrice = 6.50m, CostPrice = 0m, MeasureUnit = "pieza", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 67, Description = "Pan pita", IsSupply = true, SalePrice = 9.00m, CostPrice = 0m, MeasureUnit = "pieza", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 68, Description = "Tortilla de harina", IsSupply = true, SalePrice = 3.50m, CostPrice = 0m, MeasureUnit = "pieza", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 69, Description = "Tortilla de maíz", IsSupply = true, SalePrice = 2.50m, CostPrice = 0m, MeasureUnit = "pieza", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 70, Description = "Totopos de maíz", IsSupply = true, SalePrice = 55.00m, CostPrice = 0m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 71, Description = "Pan molido para empanizar", IsSupply = true, SalePrice = 48.00m, CostPrice = 0m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 72, Description = "Crotones sazonados", IsSupply = true, SalePrice = 85.00m, CostPrice = 0m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 73, Description = "Queso americano en rebanadas", IsSupply = true, SalePrice = 145.00m, CostPrice = 0m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 74, Description = "Queso cheddar rallado", IsSupply = true, SalePrice = 160.00m, CostPrice = 0m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 75, Description = "Queso mozzarella rallado", IsSupply = true, SalePrice = 155.00m, CostPrice = 0m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 76, Description = "Queso gouda en rebanadas", IsSupply = true, SalePrice = 175.00m, CostPrice = 0m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 77, Description = "Queso asadero", IsSupply = true, SalePrice = 150.00m, CostPrice = 0m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 78, Description = "Queso cotija rallado", IsSupply = true, SalePrice = 140.00m, CostPrice = 0m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 79, Description = "Queso crema", IsSupply = true, SalePrice = 95.00m, CostPrice = 0m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 80, Description = "Mantequilla sin sal", IsSupply = true, SalePrice = 180.00m, CostPrice = 0m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 81, Description = "Crema ácida", IsSupply = true, SalePrice = 60.00m, CostPrice = 0m, MeasureUnit = "litro", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 82, Description = "Leche entera", IsSupply = true, SalePrice = 28.00m, CostPrice = 0m, MeasureUnit = "litro", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 83, Description = "Leche deslactosada", IsSupply = true, SalePrice = 30.00m, CostPrice = 0m, MeasureUnit = "litro", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 84, Description = "Yogur natural", IsSupply = true, SalePrice = 45.00m, CostPrice = 0m, MeasureUnit = "litro", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 85, Description = "Mayonesa", IsSupply = true, SalePrice = 68.00m, CostPrice = 0m, MeasureUnit = "litro", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 86, Description = "Mayonesa chipotle", IsSupply = true, SalePrice = 78.00m, CostPrice = 0m, MeasureUnit = "litro", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 87, Description = "Catsup", IsSupply = true, SalePrice = 45.00m, CostPrice = 0m, MeasureUnit = "litro", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 88, Description = "Mostaza amarilla", IsSupply = true, SalePrice = 42.00m, CostPrice = 0m, MeasureUnit = "litro", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 89, Description = "Mostaza dijon", IsSupply = true, SalePrice = 95.00m, CostPrice = 0m, MeasureUnit = "litro", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 90, Description = "Salsa BBQ", IsSupply = true, SalePrice = 55.00m, CostPrice = 0m, MeasureUnit = "litro", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 91, Description = "Salsa BBQ picante", IsSupply = true, SalePrice = 62.00m, CostPrice = 0m, MeasureUnit = "litro", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 92, Description = "Aderezo ranch", IsSupply = true, SalePrice = 72.00m, CostPrice = 0m, MeasureUnit = "litro", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 93, Description = "Aderezo césar", IsSupply = true, SalePrice = 75.00m, CostPrice = 0m, MeasureUnit = "litro", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 94, Description = "Aderezo mil islas", IsSupply = true, SalePrice = 70.00m, CostPrice = 0m, MeasureUnit = "litro", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 95, Description = "Salsa de chipotle", IsSupply = true, SalePrice = 65.00m, CostPrice = 0m, MeasureUnit = "litro", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 96, Description = "Salsa inglesa", IsSupply = true, SalePrice = 58.00m, CostPrice = 0m, MeasureUnit = "litro", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 97, Description = "Salsa de soya", IsSupply = true, SalePrice = 48.00m, CostPrice = 0m, MeasureUnit = "litro", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 98, Description = "Salsa verde envasada", IsSupply = true, SalePrice = 52.00m, CostPrice = 0m, MeasureUnit = "litro", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 99, Description = "Salsa roja de árbol envasada", IsSupply = true, SalePrice = 56.00m, CostPrice = 0m, MeasureUnit = "litro", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 100, Description = "Salsa de habanero envasada", IsSupply = true, SalePrice = 68.00m, CostPrice = 0m, MeasureUnit = "litro", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 101, Description = "Salsa teriyaki", IsSupply = true, SalePrice = 82.00m, CostPrice = 0m, MeasureUnit = "litro", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 102, Description = "Salsa agridulce", IsSupply = true, SalePrice = 60.00m, CostPrice = 0m, MeasureUnit = "litro", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 103, Description = "Alioli de ajo", IsSupply = true, SalePrice = 80.00m, CostPrice = 0m, MeasureUnit = "litro", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 104, Description = "Salsa de tamarindo", IsSupply = true, SalePrice = 54.00m, CostPrice = 0m, MeasureUnit = "litro", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 105, Description = "Sal refinada", IsSupply = true, SalePrice = 14.00m, CostPrice = 0m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 106, Description = "Sal de mar", IsSupply = true, SalePrice = 22.00m, CostPrice = 0m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 107, Description = "Pimienta negra molida", IsSupply = true, SalePrice = 180.00m, CostPrice = 0m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 108, Description = "Paprika", IsSupply = true, SalePrice = 160.00m, CostPrice = 0m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 109, Description = "Ajo en polvo", IsSupply = true, SalePrice = 140.00m, CostPrice = 0m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 110, Description = "Cebolla en polvo", IsSupply = true, SalePrice = 130.00m, CostPrice = 0m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 111, Description = "Orégano seco", IsSupply = true, SalePrice = 120.00m, CostPrice = 0m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 112, Description = "Comino molido", IsSupply = true, SalePrice = 150.00m, CostPrice = 0m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 113, Description = "Hoja de laurel", IsSupply = true, SalePrice = 200.00m, CostPrice = 0m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 114, Description = "Tomillo seco", IsSupply = true, SalePrice = 220.00m, CostPrice = 0m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 115, Description = "Romero seco", IsSupply = true, SalePrice = 230.00m, CostPrice = 0m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 116, Description = "Albahaca seca", IsSupply = true, SalePrice = 210.00m, CostPrice = 0m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 117, Description = "Perejil seco", IsSupply = true, SalePrice = 110.00m, CostPrice = 0m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 118, Description = "Chile de árbol seco", IsSupply = true, SalePrice = 170.00m, CostPrice = 0m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 119, Description = "Chile chipotle seco", IsSupply = true, SalePrice = 190.00m, CostPrice = 0m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 120, Description = "Consomé de pollo en polvo", IsSupply = true, SalePrice = 85.00m, CostPrice = 0m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 121, Description = "Sazonador tipo tajín", IsSupply = true, SalePrice = 95.00m, CostPrice = 0m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 122, Description = "Azúcar estándar", IsSupply = true, SalePrice = 32.00m, CostPrice = 0m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 123, Description = "Azúcar mascabado", IsSupply = true, SalePrice = 45.00m, CostPrice = 0m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 124, Description = "Miel de abeja", IsSupply = true, SalePrice = 150.00m, CostPrice = 0m, MeasureUnit = "litro", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 125, Description = "Aceite vegetal", IsSupply = true, SalePrice = 48.00m, CostPrice = 0m, MeasureUnit = "litro", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 126, Description = "Aceite de oliva extra virgen", IsSupply = true, SalePrice = 180.00m, CostPrice = 0m, MeasureUnit = "litro", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 127, Description = "Aceite en aerosol", IsSupply = true, SalePrice = 95.00m, CostPrice = 0m, MeasureUnit = "pieza", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 128, Description = "Vinagre blanco", IsSupply = true, SalePrice = 25.00m, CostPrice = 0m, MeasureUnit = "litro", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 129, Description = "Vinagre de manzana", IsSupply = true, SalePrice = 42.00m, CostPrice = 0m, MeasureUnit = "litro", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 130, Description = "Vinagre balsámico", IsSupply = true, SalePrice = 120.00m, CostPrice = 0m, MeasureUnit = "litro", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 131, Description = "Jugo de limón embotellado", IsSupply = true, SalePrice = 38.00m, CostPrice = 0m, MeasureUnit = "litro", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 132, Description = "Manteca vegetal", IsSupply = true, SalePrice = 55.00m, CostPrice = 0m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 133, Description = "Refresco de cola en lata", IsSupply = true, SalePrice = 16.50m, CostPrice = 0m, MeasureUnit = "pieza", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 134, Description = "Refresco de naranja en lata", IsSupply = true, SalePrice = 16.50m, CostPrice = 0m, MeasureUnit = "pieza", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 135, Description = "Refresco de limón en lata", IsSupply = true, SalePrice = 16.50m, CostPrice = 0m, MeasureUnit = "pieza", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 136, Description = "Refresco de manzana en lata", IsSupply = true, SalePrice = 16.50m, CostPrice = 0m, MeasureUnit = "pieza", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 137, Description = "Agua embotellada 600ml", IsSupply = true, SalePrice = 9.00m, CostPrice = 0m, MeasureUnit = "pieza", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 138, Description = "Agua mineral 600ml", IsSupply = true, SalePrice = 12.00m, CostPrice = 0m, MeasureUnit = "pieza", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 139, Description = "Jugo de naranja envasado", IsSupply = true, SalePrice = 35.00m, CostPrice = 0m, MeasureUnit = "litro", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 140, Description = "Café molido americano", IsSupply = true, SalePrice = 280.00m, CostPrice = 0m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 141, Description = "Café descafeinado molido", IsSupply = true, SalePrice = 300.00m, CostPrice = 0m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 142, Description = "Té negro en bolsitas", IsSupply = true, SalePrice = 1.80m, CostPrice = 0m, MeasureUnit = "pieza", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 143, Description = "Té verde en bolsitas", IsSupply = true, SalePrice = 2.20m, CostPrice = 0m, MeasureUnit = "pieza", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 144, Description = "Chocolate en polvo", IsSupply = true, SalePrice = 120.00m, CostPrice = 0m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 145, Description = "Jarabe de vainilla", IsSupply = true, SalePrice = 110.00m, CostPrice = 0m, MeasureUnit = "litro", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 146, Description = "Jarabe de caramelo", IsSupply = true, SalePrice = 115.00m, CostPrice = 0m, MeasureUnit = "litro", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 147, Description = "Crema para café en polvo", IsSupply = true, SalePrice = 90.00m, CostPrice = 0m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 148, Description = "Mermelada de fresa", IsSupply = true, SalePrice = 75.00m, CostPrice = 0m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 149, Description = "Papas corte recto congeladas", IsSupply = true, SalePrice = 65.00m, CostPrice = 0m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 150, Description = "Papas gajo congeladas", IsSupply = true, SalePrice = 70.00m, CostPrice = 0m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 151, Description = "Aros de cebolla congelados", IsSupply = true, SalePrice = 85.00m, CostPrice = 0m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 152, Description = "Nuggets de pollo congelados", IsSupply = true, SalePrice = 110.00m, CostPrice = 0m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 153, Description = "Palitos de queso mozzarella congelados", IsSupply = true, SalePrice = 150.00m, CostPrice = 0m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 154, Description = "Medallón de res congelado", IsSupply = true, SalePrice = 38.00m, CostPrice = 0m, MeasureUnit = "pieza", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 155, Description = "Filete de pollo empanizado congelado", IsSupply = true, SalePrice = 32.00m, CostPrice = 0m, MeasureUnit = "pieza", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 156, Description = "Elote amarillo congelado", IsSupply = true, SalePrice = 45.00m, CostPrice = 0m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 157, Description = "Mezcla de verduras congeladas", IsSupply = true, SalePrice = 50.00m, CostPrice = 0m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 158, Description = "Pulpa de mango congelada", IsSupply = true, SalePrice = 60.00m, CostPrice = 0m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 159, Description = "Fresa congelada", IsSupply = true, SalePrice = 75.00m, CostPrice = 0m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 160, Description = "Helado de vainilla", IsSupply = true, SalePrice = 85.00m, CostPrice = 0m, MeasureUnit = "litro", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 161, Description = "Hielo en bolsa 5kg", IsSupply = true, SalePrice = 35.00m, CostPrice = 0m, MeasureUnit = "bolsa", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 162, Description = "Masa de pizza congelada", IsSupply = true, SalePrice = 28.00m, CostPrice = 0m, MeasureUnit = "pieza", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 163, Description = "Harina de trigo", IsSupply = true, SalePrice = 28.00m, CostPrice = 0m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 164, Description = "Fécula de maíz", IsSupply = true, SalePrice = 45.00m, CostPrice = 0m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 165, Description = "Arroz blanco", IsSupply = true, SalePrice = 30.00m, CostPrice = 0m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 166, Description = "Frijoles refritos en lata", IsSupply = true, SalePrice = 22.00m, CostPrice = 0m, MeasureUnit = "lata", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 167, Description = "Elote enlatado", IsSupply = true, SalePrice = 25.00m, CostPrice = 0m, MeasureUnit = "lata", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 168, Description = "Champiñones enlatados", IsSupply = true, SalePrice = 35.00m, CostPrice = 0m, MeasureUnit = "lata", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 169, Description = "Chiles jalapeños enlatados", IsSupply = true, SalePrice = 28.00m, CostPrice = 0m, MeasureUnit = "lata", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 170, Description = "Aceitunas negras enlatadas", IsSupply = true, SalePrice = 55.00m, CostPrice = 0m, MeasureUnit = "lata", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 171, Description = "Pepinillos en rodajas", IsSupply = true, SalePrice = 48.00m, CostPrice = 0m, MeasureUnit = "litro", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 172, Description = "Puré de tomate envasado", IsSupply = true, SalePrice = 32.00m, CostPrice = 0m, MeasureUnit = "litro", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 173, Description = "Cacahuate tostado", IsSupply = true, SalePrice = 90.00m, CostPrice = 0m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 174, Description = "Ajonjolí", IsSupply = true, SalePrice = 110.00m, CostPrice = 0m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 175, Description = "Cajeta quemada", IsSupply = true, SalePrice = 95.00m, CostPrice = 0m, MeasureUnit = "litro", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 176, Description = "Chamoy envasado", IsSupply = true, SalePrice = 50.00m, CostPrice = 0m, MeasureUnit = "litro", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 177, Description = "Vasos de cartón 12oz", IsSupply = true, SalePrice = 1.80m, CostPrice = 0m, MeasureUnit = "pieza", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 178, Description = "Vasos de cartón 16oz", IsSupply = true, SalePrice = 2.20m, CostPrice = 0m, MeasureUnit = "pieza", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 179, Description = "Tapas para vaso", IsSupply = true, SalePrice = 0.90m, CostPrice = 0m, MeasureUnit = "pieza", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 180, Description = "Popotes de papel", IsSupply = true, SalePrice = 0.40m, CostPrice = 0m, MeasureUnit = "pieza", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 181, Description = "Charolas de cartón", IsSupply = true, SalePrice = 3.50m, CostPrice = 0m, MeasureUnit = "pieza", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 182, Description = "Cajas para hamburguesa", IsSupply = true, SalePrice = 2.80m, CostPrice = 0m, MeasureUnit = "pieza", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 183, Description = "Envolturas de papel encerado", IsSupply = true, SalePrice = 0.60m, CostPrice = 0m, MeasureUnit = "pieza", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 184, Description = "Bolsas de papel para llevar", IsSupply = true, SalePrice = 1.50m, CostPrice = 0m, MeasureUnit = "pieza", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 185, Description = "Servilletas", IsSupply = true, SalePrice = 38.00m, CostPrice = 0m, MeasureUnit = "paquete", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 186, Description = "Toallas de papel en rollo", IsSupply = true, SalePrice = 45.00m, CostPrice = 0m, MeasureUnit = "pieza", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 187, Description = "Guantes desechables", IsSupply = true, SalePrice = 120.00m, CostPrice = 0m, MeasureUnit = "caja", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 188, Description = "Papel aluminio en rollo", IsSupply = true, SalePrice = 85.00m, CostPrice = 0m, MeasureUnit = "pieza", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 189, Description = "Película plástica adherente", IsSupply = true, SalePrice = 75.00m, CostPrice = 0m, MeasureUnit = "pieza", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 190, Description = "Jabón líquido para manos", IsSupply = true, SalePrice = 55.00m, CostPrice = 0m, MeasureUnit = "litro", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 191, Description = "Queso parmesano rallado", IsSupply = true, SalePrice = 180.00m, CostPrice = 0m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 192, Description = "Salsa sriracha", IsSupply = true, SalePrice = 70.00m, CostPrice = 0m, MeasureUnit = "litro", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 193, Description = "Aderezo de mostaza y miel", IsSupply = true, SalePrice = 76.00m, CostPrice = 0m, MeasureUnit = "litro", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 194, Description = "Jugo de manzana envasado", IsSupply = true, SalePrice = 34.00m, CostPrice = 0m, MeasureUnit = "litro", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 195, Description = "Agua tónica en lata", IsSupply = true, SalePrice = 15.00m, CostPrice = 0m, MeasureUnit = "pieza", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 196, Description = "Té de manzanilla en bolsitas", IsSupply = true, SalePrice = 1.80m, CostPrice = 0m, MeasureUnit = "pieza", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 197, Description = "Champiñón portobello", IsSupply = true, SalePrice = 130.00m, CostPrice = 0m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 198, Description = "Cebolla perla", IsSupply = true, SalePrice = 42.00m, CostPrice = 0m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 199, Description = "Manteca de cerdo", IsSupply = true, SalePrice = 60.00m, CostPrice = 0m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 200, Description = "Queso oaxaca", IsSupply = true, SalePrice = 145.00m, CostPrice = 0m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 201, Description = "Refresco de cola 600 ml", IsSupply = false, SalePrice = 35m, CostPrice = 0m, MeasureUnit = "pieza", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 202, Description = "Refresco de naranja 600 ml", IsSupply = false, SalePrice = 35m, CostPrice = 0m, MeasureUnit = "pieza", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 203, Description = "Limonada de limón natural 500 ml", IsSupply = false, SalePrice = 38m, CostPrice = 0m, MeasureUnit = "pieza", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 204, Description = "Té de manzanilla 500 ml", IsSupply = false, SalePrice = 32m, CostPrice = 0m, MeasureUnit = "pieza", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 205, Description = "Jugo de naranja natural 450 ml", IsSupply = false, SalePrice = 48m, CostPrice = 0m, MeasureUnit = "pieza", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 206, Description = "Batido de fresa 450 ml", IsSupply = false, SalePrice = 52m, CostPrice = 0m, MeasureUnit = "pieza", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 207, Description = "Malteada de vainilla 500 ml", IsSupply = false, SalePrice = 78m, CostPrice = 0m, MeasureUnit = "pieza", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 208, Description = "Espresso", IsSupply = false, SalePrice = 28m, CostPrice = 0m, MeasureUnit = "pieza", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 209, Description = "Capuchino", IsSupply = false, SalePrice = 45m, CostPrice = 0m, MeasureUnit = "pieza", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 210, Description = "Porción de papas fritas", IsSupply = false, SalePrice = 45m, CostPrice = 0m, MeasureUnit = "pieza", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 211, Description = "Nachos con queso", IsSupply = false, SalePrice = 69m, CostPrice = 0m, MeasureUnit = "pieza", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 212, Description = "Alitas de pollo Buffalo", IsSupply = false, SalePrice = 89m, CostPrice = 0m, MeasureUnit = "pieza", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 213, Description = "Bowl de pollo César", IsSupply = false, SalePrice = 118m, CostPrice = 0m, MeasureUnit = "pieza", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 214, Description = "Flan de caramelo", IsSupply = false, SalePrice = 42m, CostPrice = 0m, MeasureUnit = "pieza", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 215, Description = "Pastel de chocolate", IsSupply = false, SalePrice = 48m, CostPrice = 0m, MeasureUnit = "pieza", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false }
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
