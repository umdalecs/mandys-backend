using Mandys.Application.DTOs;
using Mandys.Domain;

namespace Mandys.Seeding;

public static class SeedData
{
    public const string UserPassword = "mandyspos";

    public static readonly IReadOnlyList<CreateBranchRequest> Branches =
    [
        new("Sucursal Culiacán Centro", "Av. Álvaro Obregón 1200, Centro, Culiacán", true),
        new("Sucursal Cañadas", "Blvd. Virreyes 2222, Las Cañadas, Culiacán", false)
    ];

    public static readonly IReadOnlyList<CreateUserRequest> Users =
    [
        new("Carlos", "Mendoza", "almacen.central@mandyspos.com", UserPassword, "López", Roles.CentralWarehouseChief, 1),
        new("Sofía", "Ramírez", "almacen@mandyspos.com", UserPassword, "Castro", Roles.WarehouseChief, 2),
        new("Luis", "Hernández", "caja@mandyspos.com", UserPassword, "Gómez", Roles.Cashier, 2),
        new("Elena", "Torres", "cocina@mandyspos.com", UserPassword, "Ruiz", Roles.KitchenChief, 2),
        new("Miguel", "Valenzuela", "sucursal@mandyspos.com", UserPassword, "Soto", Roles.BranchChief, 2)
    ];

    // 75 supplies (1..75) + 15 sellable products (76..90) = 90 products total
    public static readonly IReadOnlyList<CreateProductRequest> Products =
    [
        // 1..20: Verduras y frescos (insumos)
        new("Jitomate saladet", true, MeasureUnits.Grams),                  // 1
        new("Jitomate cherry", true, MeasureUnits.Grams),                   // 2
        new("Lechuga romana", true, MeasureUnits.Units),                    // 3
        new("Lechuga iceberg", true, MeasureUnits.Units),                   // 4
        new("Cebolla blanca", true, MeasureUnits.Grams),                    // 5
        new("Cebolla morada", true, MeasureUnits.Grams),                    // 6
        new("Cebolla cambray", true, MeasureUnits.Grams),                   // 7
        new("Papa blanca para freír", true, MeasureUnits.Grams),            // 8
        new("Pepino", true, MeasureUnits.Grams),                            // 9
        new("Chile jalapeño", true, MeasureUnits.Grams),                    // 10
        new("Aguacate hass", true, MeasureUnits.Grams),                     // 11
        new("Limón sin semilla", true, MeasureUnits.Grams),                 // 12
        new("Cilantro fresco", true, MeasureUnits.Grams),                   // 13
        new("Zanahoria", true, MeasureUnits.Grams),                         // 14
        new("Champiñón blanco", true, MeasureUnits.Grams),                  // 15
        new("Pimiento morrón", true, MeasureUnits.Grams),                   // 16
        new("Ajo fresco", true, MeasureUnits.Grams),                        // 17
        new("Espinaca baby", true, MeasureUnits.Grams),                     // 18
        new("Col blanca", true, MeasureUnits.Grams),                        // 19
        new("Perejil fresco", true, MeasureUnits.Grams),                    // 20

        // 21..35: Carnes y proteínas (insumos)
        new("Carne molida de res 80/20", true, MeasureUnits.Grams),         // 21
        new("Arrachera marinada", true, MeasureUnits.Grams),                // 22
        new("Milanesa de res", true, MeasureUnits.Grams),                   // 23
        new("Pechuga de pollo sin hueso", true, MeasureUnits.Grams),        // 24
        new("Tiras de pollo", true, MeasureUnits.Grams),                    // 25
        new("Milanesa de pollo", true, MeasureUnits.Grams),                 // 26
        new("Tocino ahumado", true, MeasureUnits.Grams),                    // 27
        new("Jamón de pavo", true, MeasureUnits.Grams),                     // 28
        new("Salchicha para asar", true, MeasureUnits.Grams),               // 29
        new("Chorizo de cerdo", true, MeasureUnits.Grams),                  // 30
        new("Pepperoni", true, MeasureUnits.Grams),                         // 31
        new("Huevo fresco", true, MeasureUnits.Units),                      // 32
        new("Medallón de res congelado", true, MeasureUnits.Units),         // 33
        new("Filete de pollo empanizado", true, MeasureUnits.Units),        // 34
        new("Nuggets de pollo congelados", true, MeasureUnits.Grams),       // 35

        // 36..45: Panadería y tortillas (insumos)
        new("Pan para hamburguesa con ajonjolí", true, MeasureUnits.Units), // 36
        new("Pan brioche para hamburguesa", true, MeasureUnits.Units),      // 37
        new("Pan de papa para hamburguesa", true, MeasureUnits.Units),      // 38
        new("Pan para hot dog", true, MeasureUnits.Units),                  // 39
        new("Tortilla de harina", true, MeasureUnits.Units),                // 40
        new("Tortilla de maíz", true, MeasureUnits.Units),                  // 41
        new("Totopos de maíz", true, MeasureUnits.Grams),                   // 42
        new("Crotones sazonados", true, MeasureUnits.Grams),                // 43
        new("Harina de trigo", true, MeasureUnits.Grams),                   // 44
        new("Pan molido para empanizar", true, MeasureUnits.Grams),         // 45

        // 46..55: Lácteos y quesos (insumos)
        new("Queso americano en rebanadas", true, MeasureUnits.Grams),      // 46
        new("Queso cheddar rallado", true, MeasureUnits.Grams),             // 47
        new("Queso mozzarella rallado", true, MeasureUnits.Grams),          // 48
        new("Queso asadero", true, MeasureUnits.Grams),                     // 49
        new("Queso cotija rallado", true, MeasureUnits.Grams),              // 50
        new("Queso parmesano rallado", true, MeasureUnits.Grams),           // 51
        new("Mantequilla sin sal", true, MeasureUnits.Grams),               // 52
        new("Crema ácida", true, MeasureUnits.Milliliters),                 // 53
        new("Leche entera", true, MeasureUnits.Milliliters),                // 54
        new("Helado de vainilla", true, MeasureUnits.Milliliters),          // 55

        // 56..75: Salsas, aderezos, abarrotes y empaques (insumos)
        new("Mayonesa", true, MeasureUnits.Milliliters),                    // 56
        new("Mayonesa chipotle", true, MeasureUnits.Milliliters),           // 57
        new("Catsup", true, MeasureUnits.Milliliters),                      // 58
        new("Mostaza amarilla", true, MeasureUnits.Milliliters),            // 59
        new("Salsa BBQ", true, MeasureUnits.Milliliters),                   // 60
        new("Salsa BBQ picante", true, MeasureUnits.Milliliters),           // 61
        new("Aderezo ranch", true, MeasureUnits.Milliliters),               // 62
        new("Aderezo césar", true, MeasureUnits.Milliliters),               // 63
        new("Pepinillos en rodajas", true, MeasureUnits.Grams),             // 64
        new("Puré de tomate envasado", true, MeasureUnits.Milliliters),     // 65
        new("Aceite vegetal", true, MeasureUnits.Milliliters),              // 66
        new("Manteca vegetal", true, MeasureUnits.Grams),                   // 67
        new("Sal refinada", true, MeasureUnits.Grams),                      // 68
        new("Pimienta negra molida", true, MeasureUnits.Grams),             // 69
        new("Azúcar estándar", true, MeasureUnits.Grams),                   // 70
        new("Cajeta quemada", true, MeasureUnits.Milliliters),              // 71
        new("Papas corte recto congeladas", true, MeasureUnits.Grams),      // 72
        new("Frijoles refritos", true, MeasureUnits.Grams),                 // 73
        new("Cajas para hamburguesa", true, MeasureUnits.Units),            // 74
        new("Bolsas de papel para llevar", true, MeasureUnits.Units),       // 75

        // 76..90: Productos de venta directa (IsSupply = false)
        new("Refresco de cola 600 ml", false, MeasureUnits.Units, 35m),            // 76
        new("Refresco de naranja 600 ml", false, MeasureUnits.Units, 35m),         // 77
        new("Limonada natural 500 ml", false, MeasureUnits.Units, 38m),            // 78
        new("Té helado de durazno 500 ml", false, MeasureUnits.Units, 32m),        // 79
        new("Jugo de naranja natural 450 ml", false, MeasureUnits.Units, 48m),     // 80
        new("Agua natural embotellada 600 ml", false, MeasureUnits.Units, 25m),    // 81
        new("Malteada de vainilla 500 ml", false, MeasureUnits.Units, 78m),        // 82
        new("Malteada de chocolate 500 ml", false, MeasureUnits.Units, 78m),       // 83
        new("Café americano 355 ml", false, MeasureUnits.Units, 35m),              // 84
        new("Capuchino 355 ml", false, MeasureUnits.Units, 45m),                   // 85
        new("Porción de papas a la francesa", false, MeasureUnits.Units, 45m),     // 86
        new("Orden de aros de cebolla", false, MeasureUnits.Units, 55m),           // 87
        new("Nachos con queso", false, MeasureUnits.Units, 69m),                   // 88
        new("Flan napolitano", false, MeasureUnits.Units, 42m),                    // 89
        new("Rebanada de pastel de chocolate", false, MeasureUnits.Units, 48m)     // 90
    ];

    public static readonly IReadOnlyList<CreateDishRequest> Dishes =
    [
        new(
            "Hamburguesa clásica",
            95m,
            [
                new(36, 1m),   // Pan para hamburguesa con ajonjolí (1 unidad)
                new(21, 150m), // Carne molida de res 80/20 (150 g)
                new(46, 30m),  // Queso americano en rebanadas (30 g)
                new(4, 1m),    // Lechuga iceberg (1 unidad)
                new(1, 40m),   // Jitomate saladet (40 g)
                new(5, 25m),   // Cebolla blanca (25 g)
                new(58, 15m),  // Catsup (15 ml)
                new(59, 10m),  // Mostaza amarilla (10 ml)
                new(64, 15m)   // Pepinillos en rodajas (15 g)
            ]
        ),
        new(
            "Hamburguesa doble con tocino",
            145m,
            [
                new(37, 1m),   // Pan brioche para hamburguesa (1 unidad)
                new(21, 300m), // Carne molida de res 80/20 (300 g)
                new(48, 45m),  // Queso mozzarella rallado (45 g)
                new(46, 30m),  // Queso americano en rebanadas (30 g)
                new(27, 40m),  // Tocino ahumado (40 g)
                new(1, 50m),   // Jitomate saladet (50 g)
                new(5, 30m),   // Cebolla blanca (30 g)
                new(60, 25m)   // Salsa BBQ (25 ml)
            ]
        ),
        new(
            "Hamburguesa crispy de pollo",
            110m,
            [
                new(38, 1m),   // Pan de papa para hamburguesa (1 unidad)
                new(26, 160m), // Milanesa de pollo (160 g)
                new(47, 30m),  // Queso cheddar rallado (30 g)
                new(3, 1m),    // Lechuga romana (1 unidad)
                new(9, 25m),   // Pepino (25 g)
                new(57, 20m)   // Mayonesa chipotle (20 ml)
            ]
        ),
        new(
            "Orden de tacos de arrachera",
            115m,
            [
                new(41, 3m),   // Tortilla de maíz (3 unidades)
                new(22, 180m), // Arrachera marinada (180 g)
                new(11, 50m),  // Aguacate hass (50 g)
                new(13, 15m),  // Cilantro fresco (15 g)
                new(6, 25m),   // Cebolla morada (25 g)
                new(12, 20m)   // Limón sin semilla (20 g)
            ]
        ),
        new(
            "Orden de tacos de bistec",
            98m,
            [
                new(41, 3m),   // Tortilla de maíz (3 unidades)
                new(23, 160m), // Milanesa de res (160 g)
                new(7, 40m),   // Cebolla cambray (40 g)
                new(13, 15m),  // Cilantro fresco (15 g)
                new(68, 2m),   // Sal refinada (2 g)
                new(69, 1m)    // Pimienta negra molida (1 g)
            ]
        ),
        new(
            "Burrito norteño de carne",
            118m,
            [
                new(40, 1m),   // Tortilla de harina (1 unidad)
                new(22, 160m), // Arrachera marinada (160 g)
                new(73, 80m),  // Frijoles refritos (80 g)
                new(49, 45m),  // Queso asadero (45 g)
                new(11, 40m),  // Aguacate hass (40 g)
                new(53, 25m),  // Crema ácida (25 ml)
                new(61, 15m)   // Salsa BBQ picante (15 ml)
            ]
        ),
        new(
            "Quesadillas de pollo",
            88m,
            [
                new(40, 2m),   // Tortilla de harina (2 unidades)
                new(25, 140m), // Tiras de pollo (140 g)
                new(49, 60m),  // Queso asadero (60 g)
                new(16, 30m),  // Pimiento morrón (30 g)
                new(62, 25m)   // Aderezo ranch (25 ml)
            ]
        ),
        new(
            "Ensalada César con pollo",
            105m,
            [
                new(3, 1m),    // Lechuga romana (1 unidad)
                new(24, 150m), // Pechuga de pollo sin hueso (150 g)
                new(51, 25m),  // Queso parmesano rallado (25 g)
                new(43, 35m),  // Crotones sazonados (35 g)
                new(63, 40m),  // Aderezo césar (40 ml)
                new(68, 2m),   // Sal refinada (2 g)
                new(69, 1m)    // Pimienta negra molida (1 g)
            ]
        ),
        new(
            "Churros con cajeta",
            68m,
            [
                new(44, 100m), // Harina de trigo (100 g)
                new(67, 40m),  // Manteca vegetal (40 g)
                new(70, 20m),  // Azúcar estándar (20 g)
                new(71, 45m)   // Cajeta quemada (45 ml)
            ]
        ),
        new(
            "Sopa de tortilla",
            72m,
            [
                new(42, 80m),  // Totopos de maíz (80 g)
                new(65, 150m), // Puré de tomate envasado (150 ml)
                new(49, 35m),  // Queso asadero (35 g)
                new(11, 35m),  // Aguacate hass (35 g)
                new(53, 20m),  // Crema ácida (20 ml)
                new(5, 20m)    // Cebolla blanca (20 g)
            ]
        )
    ];

    public static readonly IReadOnlyList<CreateComboRequest> Combos =
    [
        new(
            "Combo Hamburguesa clásica",
            149m,
            [new(1, 1m)],
            [new(76, 1m), new(86, 1m)]
        ),
        new(
            "Combo Hamburguesa doble",
            199m,
            [new(2, 1m)],
            [new(77, 1m), new(86, 1m)]
        ),
        new(
            "Combo Hamburguesa de pollo",
            165m,
            [new(3, 1m)],
            [new(78, 1m), new(86, 1m)]
        ),
        new(
            "Combo Tacos de arrachera",
            159m,
            [new(4, 1m)],
            [new(76, 1m), new(88, 1m)]
        ),
        new(
            "Combo Tacos de bistec",
            139m,
            [new(5, 1m)],
            [new(77, 1m)]
        ),
        new(
            "Combo Burrito norteño",
            175m,
            [new(6, 1m)],
            [new(79, 1m), new(88, 1m)]
        ),
        new(
            "Combo Quesadillas",
            135m,
            [new(7, 1m)],
            [new(80, 1m)]
        ),
        new(
            "Combo Ensalada César",
            139m,
            [new(8, 1m)],
            [new(81, 1m)]
        ),
        new(
            "Combo Postre y Café",
            129m,
            [new(9, 1m)],
            [new(85, 1m), new(90, 1m)]
        ),
        new(
            "Combo Familiar",
            549m,
            [new(1, 2m), new(3, 2m)],
            [new(76, 4m), new(86, 2m)]
        )
    ];

    public static readonly IReadOnlyDictionary<int, IReadOnlyList<CreateStorageInRequest>> StorageIns =
        new Dictionary<int, IReadOnlyList<CreateStorageInRequest>>
        {
            // Sucursal 1: Sucursal Culiacán Centro (Almacén Central)
            [1] =
            [
                new CreateStorageInRequest(
                    "ENT-CEN-001",
                    "Distribuidora Agropecuaria de Sinaloa",
                    [
                        new(1, 25000m, 0.03m),  // Jitomate saladet (25 kg @ $0.03/g = $30/kg)
                        new(3, 120m, 18m),      // Lechuga romana (120 unidades @ $18/unidad)
                        new(4, 120m, 16m),      // Lechuga iceberg (120 unidades @ $16/unidad)
                        new(5, 20000m, 0.025m), // Cebolla blanca (20 kg @ $0.025/g = $25/kg)
                        new(11, 15000m, 0.07m), // Aguacate hass (15 kg @ $0.07/g = $70/kg)
                        new(21, 50000m, 0.16m), // Carne molida de res 80/20 (50 kg @ $0.16/g = $160/kg)
                        new(22, 35000m, 0.22m), // Arrachera marinada (35 kg @ $0.22/g = $220/kg)
                        new(26, 30000m, 0.13m), // Milanesa de pollo (30 kg @ $0.13/g = $130/kg)
                        new(27, 12000m, 0.19m)  // Tocino ahumado (12 kg @ $0.19/g = $190/kg)
                    ]
                ),
                new(
                    "ENT-CEN-002",
                    "Comercializadora de Alimentos del Noroeste",
                    [
                        new(36, 300m, 5.50m),   // Pan para hamburguesa con ajonjolí (300 unidades @ $5.50)
                        new(37, 200m, 7.50m),   // Pan brioche para hamburguesa (200 unidades @ $7.50)
                        new(38, 200m, 6.50m),   // Pan de papa para hamburguesa (200 unidades @ $6.50)
                        new(40, 400m, 3.00m),   // Tortilla de harina (400 unidades @ $3.00)
                        new(41, 600m, 1.20m),   // Tortilla de maíz (600 unidades @ $1.20)
                        new(46, 15000m, 0.14m), // Queso americano en rebanadas (15 kg @ $0.14/g)
                        new(48, 15000m, 0.15m), // Queso mozzarella rallado (15 kg @ $0.15/g)
                        new(58, 10000m, 0.04m), // Catsup (10 L @ $0.04/ml)
                        new(76, 240m, 16m),     // Refresco de cola 600 ml (240 unidades @ $16)
                        new(86, 180m, 18m)      // Porción de papas a la francesa (180 unidades @ $18)
                    ]
                )
            ],

            // Sucursal 2: Sucursal Cañadas
            [2] =
            [
                new(
                    "ENT-CAN-001",
                    "Distribuidora Agropecuaria de Sinaloa",
                    [
                        new(1, 10000m),  // Jitomate saladet (10 kg)
                        new(3, 50m),     // Lechuga romana (50 unidades)
                        new(4, 60m),     // Lechuga iceberg (60 unidades)
                        new(5, 8000m),   // Cebolla blanca (8 kg)
                        new(11, 6000m),  // Aguacate hass (6 kg)
                        new(21, 20000m), // Carne molida de res 80/20 (20 kg)
                        new(22, 15000m), // Arrachera marinada (15 kg)
                        new(26, 12000m), // Milanesa de pollo (12 kg)
                        new(27, 5000m)   // Tocino ahumado (5 kg)
                    ]
                ),
                new(
                    "ENT-CAN-002",
                    "Comercializadora de Alimentos del Noroeste",
                    [
                        new(36, 120m),   // Pan para hamburguesa con ajonjolí (120 unidades)
                        new(37, 80m),    // Pan brioche para hamburguesa (80 unidades)
                        new(38, 80m),    // Pan de papa para hamburguesa (80 unidades)
                        new(40, 150m),   // Tortilla de harina (150 unidades)
                        new(41, 250m),   // Tortilla de maíz (250 unidades)
                        new(46, 6000m),  // Queso americano en rebanadas (6 kg)
                        new(48, 6000m),  // Queso mozzarella rallado (6 kg)
                        new(58, 4000m),  // Catsup (4 L)
                        new(76, 100m),   // Refresco de cola 600 ml (100 unidades)
                        new(86, 80m)     // Porción de papas a la francesa (80 unidades)
                    ]
                )
            ]
        };

    public static readonly IReadOnlyDictionary<int, IReadOnlyList<CreateStorageOutRequest>> StorageOuts =
        new Dictionary<int, IReadOnlyList<CreateStorageOutRequest>>
        {
            [1] =
            [
                new(
                    "TRASPASO",
                    [
                        new(21, 10000m), // Carne molida de res 80/20 (10 kg)
                        new(22, 8000m),  // Arrachera marinada (8 kg)
                        new(36, 80m),    // Pan para hamburguesa con ajonjolí (80 unidades)
                        new(46, 3000m),  // Queso americano en rebanadas (3 kg)
                        new(76, 60m)     // Refresco de cola 600 ml (60 unidades)
                    ]
                )
            ],

            [2] =
            [
                new(
                    "MERMA",
                    [
                        new(1, 2500m),   // Jitomate saladet (2.5 kg)
                        new(4, 15m),     // Lechuga iceberg (15 unidades)
                        new(21, 6000m),  // Carne molida de res 80/20 (6 kg)
                        new(36, 40m),    // Pan para hamburguesa con ajonjolí (40 unidades)
                        new(46, 1200m),  // Queso americano en rebanadas (1.2 kg)
                        new(58, 600m),   // Catsup (600 ml)
                        new(76, 30m),    // Refresco de cola 600 ml (30 unidades)
                        new(86, 25m)     // Porción de papas a la francesa (25 unidades)
                    ]
                )
            ]
        };
}
