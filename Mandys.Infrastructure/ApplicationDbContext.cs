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
                    Email = "admin@mandyspos.com",
                    // plain password: administrator
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
        });

        modelBuilder.Entity<ProductRecord>(entity =>
        {
            // Note: Make sure to only use static data here
            var seededAt = new DateTime(2026, 9, 9, 0, 0, 0, DateTimeKind.Utc);
            entity.HasData(
                new { Id = 1, Description = "Jitomate saladet", IsSupply = true, Price = 28.50m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 2, Description = "Jitomate cherry", IsSupply = true, Price = 55.00m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 3, Description = "Lechuga romana", IsSupply = true, Price = 18.00m, MeasureUnit = "pieza", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 4, Description = "Lechuga iceberg", IsSupply = true, Price = 22.00m, MeasureUnit = "pieza", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 5, Description = "Cebolla blanca", IsSupply = true, Price = 32.00m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 6, Description = "Cebolla morada", IsSupply = true, Price = 38.00m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 7, Description = "Cebolla cambray", IsSupply = true, Price = 25.00m, MeasureUnit = "manojo", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 8, Description = "Papa blanca para freír", IsSupply = true, Price = 26.00m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 9, Description = "Pepino", IsSupply = true, Price = 24.00m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 10, Description = "Chile jalapeño", IsSupply = true, Price = 35.00m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 11, Description = "Aguacate hass", IsSupply = true, Price = 85.00m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 12, Description = "Limón sin semilla", IsSupply = true, Price = 30.00m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 13, Description = "Cilantro fresco", IsSupply = true, Price = 12.00m, MeasureUnit = "manojo", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 14, Description = "Zanahoria", IsSupply = true, Price = 22.00m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 15, Description = "Champiñón blanco", IsSupply = true, Price = 95.00m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 16, Description = "Pimiento morrón verde", IsSupply = true, Price = 48.00m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 17, Description = "Pimiento morrón rojo", IsSupply = true, Price = 62.00m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 18, Description = "Apio", IsSupply = true, Price = 28.00m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 19, Description = "Ajo fresco", IsSupply = true, Price = 110.00m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 20, Description = "Elote amarillo", IsSupply = true, Price = 9.00m, MeasureUnit = "pieza", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 21, Description = "Calabacita", IsSupply = true, Price = 26.00m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 22, Description = "Espinaca baby", IsSupply = true, Price = 120.00m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 23, Description = "Col blanca", IsSupply = true, Price = 20.00m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 24, Description = "Rábano", IsSupply = true, Price = 24.00m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 25, Description = "Nopal limpio", IsSupply = true, Price = 30.00m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 26, Description = "Piña miel", IsSupply = true, Price = 25.00m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 27, Description = "Mango ataulfo", IsSupply = true, Price = 40.00m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 28, Description = "Fresa", IsSupply = true, Price = 70.00m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 29, Description = "Plátano tabasco", IsSupply = true, Price = 22.00m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 30, Description = "Manzana roja", IsSupply = true, Price = 45.00m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 31, Description = "Naranja para jugo", IsSupply = true, Price = 20.00m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 32, Description = "Perejil fresco", IsSupply = true, Price = 12.00m, MeasureUnit = "manojo", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 33, Description = "Epazote fresco", IsSupply = true, Price = 10.00m, MeasureUnit = "manojo", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 34, Description = "Chayote", IsSupply = true, Price = 18.00m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 35, Description = "Betabel", IsSupply = true, Price = 26.00m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 36, Description = "Carne molida de res 80/20", IsSupply = true, Price = 165.00m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 37, Description = "Carne molida de res 90/10", IsSupply = true, Price = 185.00m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 38, Description = "Arrachera marinada", IsSupply = true, Price = 240.00m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 39, Description = "Milanesa de res", IsSupply = true, Price = 190.00m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 40, Description = "Pechuga de pollo sin hueso", IsSupply = true, Price = 120.00m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 41, Description = "Muslo de pollo sin hueso", IsSupply = true, Price = 95.00m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 42, Description = "Tiras de pollo", IsSupply = true, Price = 115.00m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 43, Description = "Milanesa de pollo", IsSupply = true, Price = 125.00m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 44, Description = "Tocino ahumado", IsSupply = true, Price = 210.00m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 45, Description = "Jamón de pavo", IsSupply = true, Price = 130.00m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 46, Description = "Chuleta ahumada", IsSupply = true, Price = 150.00m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 47, Description = "Salchicha para asar", IsSupply = true, Price = 90.00m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 48, Description = "Longaniza", IsSupply = true, Price = 110.00m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 49, Description = "Pepperoni", IsSupply = true, Price = 220.00m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 50, Description = "Huevo fresco", IsSupply = true, Price = 52.00m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 51, Description = "Huevo líquido pasteurizado", IsSupply = true, Price = 65.00m, MeasureUnit = "litro", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 52, Description = "Filete de pescado empanizado", IsSupply = true, Price = 140.00m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 53, Description = "Camarón mediano sin cáscara", IsSupply = true, Price = 260.00m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 54, Description = "Atún enlatado", IsSupply = true, Price = 32.00m, MeasureUnit = "lata", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 55, Description = "Costilla de cerdo", IsSupply = true, Price = 145.00m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 56, Description = "Pulled pork", IsSupply = true, Price = 175.00m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 57, Description = "Salami", IsSupply = true, Price = 200.00m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 58, Description = "Chorizo argentino", IsSupply = true, Price = 135.00m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 59, Description = "Pavo molido", IsSupply = true, Price = 150.00m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 60, Description = "Lomo de cerdo", IsSupply = true, Price = 130.00m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 61, Description = "Pan para hamburguesa con ajonjolí", IsSupply = true, Price = 8.50m, MeasureUnit = "pieza", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 62, Description = "Pan brioche para hamburguesa", IsSupply = true, Price = 11.00m, MeasureUnit = "pieza", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 63, Description = "Pan integral para hamburguesa", IsSupply = true, Price = 10.00m, MeasureUnit = "pieza", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 64, Description = "Pan de papa para hamburguesa", IsSupply = true, Price = 12.00m, MeasureUnit = "pieza", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 65, Description = "Pan para hot dog", IsSupply = true, Price = 7.50m, MeasureUnit = "pieza", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 66, Description = "Pan telera", IsSupply = true, Price = 6.50m, MeasureUnit = "pieza", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 67, Description = "Pan pita", IsSupply = true, Price = 9.00m, MeasureUnit = "pieza", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 68, Description = "Tortilla de harina", IsSupply = true, Price = 3.50m, MeasureUnit = "pieza", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 69, Description = "Tortilla de maíz", IsSupply = true, Price = 2.50m, MeasureUnit = "pieza", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 70, Description = "Totopos de maíz", IsSupply = true, Price = 55.00m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 71, Description = "Pan molido para empanizar", IsSupply = true, Price = 48.00m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 72, Description = "Crotones sazonados", IsSupply = true, Price = 85.00m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 73, Description = "Queso americano en rebanadas", IsSupply = true, Price = 145.00m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 74, Description = "Queso cheddar rallado", IsSupply = true, Price = 160.00m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 75, Description = "Queso mozzarella rallado", IsSupply = true, Price = 155.00m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 76, Description = "Queso gouda en rebanadas", IsSupply = true, Price = 175.00m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 77, Description = "Queso asadero", IsSupply = true, Price = 150.00m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 78, Description = "Queso cotija rallado", IsSupply = true, Price = 140.00m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 79, Description = "Queso crema", IsSupply = true, Price = 95.00m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 80, Description = "Mantequilla sin sal", IsSupply = true, Price = 180.00m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 81, Description = "Crema ácida", IsSupply = true, Price = 60.00m, MeasureUnit = "litro", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 82, Description = "Leche entera", IsSupply = true, Price = 28.00m, MeasureUnit = "litro", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 83, Description = "Leche deslactosada", IsSupply = true, Price = 30.00m, MeasureUnit = "litro", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 84, Description = "Yogur natural", IsSupply = true, Price = 45.00m, MeasureUnit = "litro", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 85, Description = "Mayonesa", IsSupply = true, Price = 68.00m, MeasureUnit = "litro", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 86, Description = "Mayonesa chipotle", IsSupply = true, Price = 78.00m, MeasureUnit = "litro", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 87, Description = "Catsup", IsSupply = true, Price = 45.00m, MeasureUnit = "litro", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 88, Description = "Mostaza amarilla", IsSupply = true, Price = 42.00m, MeasureUnit = "litro", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 89, Description = "Mostaza dijon", IsSupply = true, Price = 95.00m, MeasureUnit = "litro", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 90, Description = "Salsa BBQ", IsSupply = true, Price = 55.00m, MeasureUnit = "litro", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 91, Description = "Salsa BBQ picante", IsSupply = true, Price = 62.00m, MeasureUnit = "litro", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 92, Description = "Aderezo ranch", IsSupply = true, Price = 72.00m, MeasureUnit = "litro", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 93, Description = "Aderezo césar", IsSupply = true, Price = 75.00m, MeasureUnit = "litro", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 94, Description = "Aderezo mil islas", IsSupply = true, Price = 70.00m, MeasureUnit = "litro", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 95, Description = "Salsa de chipotle", IsSupply = true, Price = 65.00m, MeasureUnit = "litro", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 96, Description = "Salsa inglesa", IsSupply = true, Price = 58.00m, MeasureUnit = "litro", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 97, Description = "Salsa de soya", IsSupply = true, Price = 48.00m, MeasureUnit = "litro", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 98, Description = "Salsa verde envasada", IsSupply = true, Price = 52.00m, MeasureUnit = "litro", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 99, Description = "Salsa roja de árbol envasada", IsSupply = true, Price = 56.00m, MeasureUnit = "litro", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 100, Description = "Salsa de habanero envasada", IsSupply = true, Price = 68.00m, MeasureUnit = "litro", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 101, Description = "Salsa teriyaki", IsSupply = true, Price = 82.00m, MeasureUnit = "litro", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 102, Description = "Salsa agridulce", IsSupply = true, Price = 60.00m, MeasureUnit = "litro", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 103, Description = "Alioli de ajo", IsSupply = true, Price = 80.00m, MeasureUnit = "litro", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 104, Description = "Salsa de tamarindo", IsSupply = true, Price = 54.00m, MeasureUnit = "litro", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 105, Description = "Sal refinada", IsSupply = true, Price = 14.00m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 106, Description = "Sal de mar", IsSupply = true, Price = 22.00m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 107, Description = "Pimienta negra molida", IsSupply = true, Price = 180.00m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 108, Description = "Paprika", IsSupply = true, Price = 160.00m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 109, Description = "Ajo en polvo", IsSupply = true, Price = 140.00m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 110, Description = "Cebolla en polvo", IsSupply = true, Price = 130.00m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 111, Description = "Orégano seco", IsSupply = true, Price = 120.00m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 112, Description = "Comino molido", IsSupply = true, Price = 150.00m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 113, Description = "Hoja de laurel", IsSupply = true, Price = 200.00m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 114, Description = "Tomillo seco", IsSupply = true, Price = 220.00m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 115, Description = "Romero seco", IsSupply = true, Price = 230.00m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 116, Description = "Albahaca seca", IsSupply = true, Price = 210.00m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 117, Description = "Perejil seco", IsSupply = true, Price = 110.00m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 118, Description = "Chile de árbol seco", IsSupply = true, Price = 170.00m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 119, Description = "Chile chipotle seco", IsSupply = true, Price = 190.00m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 120, Description = "Consomé de pollo en polvo", IsSupply = true, Price = 85.00m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 121, Description = "Sazonador tipo tajín", IsSupply = true, Price = 95.00m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 122, Description = "Azúcar estándar", IsSupply = true, Price = 32.00m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 123, Description = "Azúcar mascabado", IsSupply = true, Price = 45.00m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 124, Description = "Miel de abeja", IsSupply = true, Price = 150.00m, MeasureUnit = "litro", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 125, Description = "Aceite vegetal", IsSupply = true, Price = 48.00m, MeasureUnit = "litro", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 126, Description = "Aceite de oliva extra virgen", IsSupply = true, Price = 180.00m, MeasureUnit = "litro", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 127, Description = "Aceite en aerosol", IsSupply = true, Price = 95.00m, MeasureUnit = "pieza", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 128, Description = "Vinagre blanco", IsSupply = true, Price = 25.00m, MeasureUnit = "litro", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 129, Description = "Vinagre de manzana", IsSupply = true, Price = 42.00m, MeasureUnit = "litro", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 130, Description = "Vinagre balsámico", IsSupply = true, Price = 120.00m, MeasureUnit = "litro", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 131, Description = "Jugo de limón embotellado", IsSupply = true, Price = 38.00m, MeasureUnit = "litro", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 132, Description = "Manteca vegetal", IsSupply = true, Price = 55.00m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 133, Description = "Refresco de cola en lata", IsSupply = true, Price = 16.50m, MeasureUnit = "pieza", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 134, Description = "Refresco de naranja en lata", IsSupply = true, Price = 16.50m, MeasureUnit = "pieza", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 135, Description = "Refresco de limón en lata", IsSupply = true, Price = 16.50m, MeasureUnit = "pieza", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 136, Description = "Refresco de manzana en lata", IsSupply = true, Price = 16.50m, MeasureUnit = "pieza", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 137, Description = "Agua embotellada 600ml", IsSupply = true, Price = 9.00m, MeasureUnit = "pieza", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 138, Description = "Agua mineral 600ml", IsSupply = true, Price = 12.00m, MeasureUnit = "pieza", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 139, Description = "Jugo de naranja envasado", IsSupply = true, Price = 35.00m, MeasureUnit = "litro", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 140, Description = "Café molido americano", IsSupply = true, Price = 280.00m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 141, Description = "Café descafeinado molido", IsSupply = true, Price = 300.00m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 142, Description = "Té negro en bolsitas", IsSupply = true, Price = 1.80m, MeasureUnit = "pieza", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 143, Description = "Té verde en bolsitas", IsSupply = true, Price = 2.20m, MeasureUnit = "pieza", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 144, Description = "Chocolate en polvo", IsSupply = true, Price = 120.00m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 145, Description = "Jarabe de vainilla", IsSupply = true, Price = 110.00m, MeasureUnit = "litro", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 146, Description = "Jarabe de caramelo", IsSupply = true, Price = 115.00m, MeasureUnit = "litro", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 147, Description = "Crema para café en polvo", IsSupply = true, Price = 90.00m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 148, Description = "Mermelada de fresa", IsSupply = true, Price = 75.00m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 149, Description = "Papas corte recto congeladas", IsSupply = true, Price = 65.00m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 150, Description = "Papas gajo congeladas", IsSupply = true, Price = 70.00m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 151, Description = "Aros de cebolla congelados", IsSupply = true, Price = 85.00m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 152, Description = "Nuggets de pollo congelados", IsSupply = true, Price = 110.00m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 153, Description = "Palitos de queso mozzarella congelados", IsSupply = true, Price = 150.00m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 154, Description = "Medallón de res congelado", IsSupply = true, Price = 38.00m, MeasureUnit = "pieza", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 155, Description = "Filete de pollo empanizado congelado", IsSupply = true, Price = 32.00m, MeasureUnit = "pieza", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 156, Description = "Elote amarillo congelado", IsSupply = true, Price = 45.00m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 157, Description = "Mezcla de verduras congeladas", IsSupply = true, Price = 50.00m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 158, Description = "Pulpa de mango congelada", IsSupply = true, Price = 60.00m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 159, Description = "Fresa congelada", IsSupply = true, Price = 75.00m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 160, Description = "Helado de vainilla", IsSupply = true, Price = 85.00m, MeasureUnit = "litro", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 161, Description = "Hielo en bolsa 5kg", IsSupply = true, Price = 35.00m, MeasureUnit = "bolsa", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 162, Description = "Masa de pizza congelada", IsSupply = true, Price = 28.00m, MeasureUnit = "pieza", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 163, Description = "Harina de trigo", IsSupply = true, Price = 28.00m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 164, Description = "Fécula de maíz", IsSupply = true, Price = 45.00m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 165, Description = "Arroz blanco", IsSupply = true, Price = 30.00m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 166, Description = "Frijoles refritos en lata", IsSupply = true, Price = 22.00m, MeasureUnit = "lata", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 167, Description = "Elote enlatado", IsSupply = true, Price = 25.00m, MeasureUnit = "lata", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 168, Description = "Champiñones enlatados", IsSupply = true, Price = 35.00m, MeasureUnit = "lata", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 169, Description = "Chiles jalapeños enlatados", IsSupply = true, Price = 28.00m, MeasureUnit = "lata", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 170, Description = "Aceitunas negras enlatadas", IsSupply = true, Price = 55.00m, MeasureUnit = "lata", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 171, Description = "Pepinillos en rodajas", IsSupply = true, Price = 48.00m, MeasureUnit = "litro", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 172, Description = "Puré de tomate envasado", IsSupply = true, Price = 32.00m, MeasureUnit = "litro", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 173, Description = "Cacahuate tostado", IsSupply = true, Price = 90.00m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 174, Description = "Ajonjolí", IsSupply = true, Price = 110.00m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 175, Description = "Cajeta quemada", IsSupply = true, Price = 95.00m, MeasureUnit = "litro", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 176, Description = "Chamoy envasado", IsSupply = true, Price = 50.00m, MeasureUnit = "litro", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 177, Description = "Vasos de cartón 12oz", IsSupply = true, Price = 1.80m, MeasureUnit = "pieza", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 178, Description = "Vasos de cartón 16oz", IsSupply = true, Price = 2.20m, MeasureUnit = "pieza", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 179, Description = "Tapas para vaso", IsSupply = true, Price = 0.90m, MeasureUnit = "pieza", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 180, Description = "Popotes de papel", IsSupply = true, Price = 0.40m, MeasureUnit = "pieza", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 181, Description = "Charolas de cartón", IsSupply = true, Price = 3.50m, MeasureUnit = "pieza", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 182, Description = "Cajas para hamburguesa", IsSupply = true, Price = 2.80m, MeasureUnit = "pieza", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 183, Description = "Envolturas de papel encerado", IsSupply = true, Price = 0.60m, MeasureUnit = "pieza", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 184, Description = "Bolsas de papel para llevar", IsSupply = true, Price = 1.50m, MeasureUnit = "pieza", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 185, Description = "Servilletas", IsSupply = true, Price = 38.00m, MeasureUnit = "paquete", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 186, Description = "Toallas de papel en rollo", IsSupply = true, Price = 45.00m, MeasureUnit = "pieza", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 187, Description = "Guantes desechables", IsSupply = true, Price = 120.00m, MeasureUnit = "caja", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 188, Description = "Papel aluminio en rollo", IsSupply = true, Price = 85.00m, MeasureUnit = "pieza", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 189, Description = "Película plástica adherente", IsSupply = true, Price = 75.00m, MeasureUnit = "pieza", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 190, Description = "Jabón líquido para manos", IsSupply = true, Price = 55.00m, MeasureUnit = "litro", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 191, Description = "Queso parmesano rallado", IsSupply = true, Price = 180.00m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 192, Description = "Salsa sriracha", IsSupply = true, Price = 70.00m, MeasureUnit = "litro", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 193, Description = "Aderezo de mostaza y miel", IsSupply = true, Price = 76.00m, MeasureUnit = "litro", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 194, Description = "Jugo de manzana envasado", IsSupply = true, Price = 34.00m, MeasureUnit = "litro", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 195, Description = "Agua tónica en lata", IsSupply = true, Price = 15.00m, MeasureUnit = "pieza", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 196, Description = "Té de manzanilla en bolsitas", IsSupply = true, Price = 1.80m, MeasureUnit = "pieza", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 197, Description = "Champiñón portobello", IsSupply = true, Price = 130.00m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 198, Description = "Cebolla perla", IsSupply = true, Price = 42.00m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 199, Description = "Manteca de cerdo", IsSupply = true, Price = 60.00m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false },
                new { Id = 200, Description = "Queso oaxaca", IsSupply = true, Price = 145.00m, MeasureUnit = "kg", CreatedAt = seededAt, UpdatedAt = seededAt, IsDeleted = false }
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
