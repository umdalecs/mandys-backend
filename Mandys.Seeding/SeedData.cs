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
        new("Jitomate saladet", true, MeasureUnits.Kilograms),                  // 1
        new("Jitomate cherry", true, MeasureUnits.Kilograms),                   // 2
        new("Lechuga romana", true, MeasureUnits.Units),                    // 3
        new("Lechuga iceberg", true, MeasureUnits.Units),                   // 4
        new("Cebolla blanca", true, MeasureUnits.Kilograms),                    // 5
        new("Cebolla morada", true, MeasureUnits.Kilograms),                    // 6
        new("Cebolla cambray", true, MeasureUnits.Kilograms),                   // 7
        new("Papa blanca para freír", true, MeasureUnits.Kilograms),            // 8
        new("Pepino", true, MeasureUnits.Kilograms),                            // 9
        new("Chile jalapeño", true, MeasureUnits.Kilograms),                    // 10
        new("Aguacate hass", true, MeasureUnits.Kilograms),                     // 11
        new("Limón sin semilla", true, MeasureUnits.Kilograms),                 // 12
        new("Cilantro fresco", true, MeasureUnits.Kilograms),                   // 13
        new("Zanahoria", true, MeasureUnits.Kilograms),                         // 14
        new("Champiñón blanco", true, MeasureUnits.Kilograms),                  // 15
        new("Pimiento morrón", true, MeasureUnits.Kilograms),                   // 16
        new("Ajo fresco", true, MeasureUnits.Kilograms),                        // 17
        new("Espinaca baby", true, MeasureUnits.Kilograms),                     // 18
        new("Col blanca", true, MeasureUnits.Kilograms),                        // 19
        new("Perejil fresco", true, MeasureUnits.Kilograms),                    // 20

        // 21..35: Carnes y proteínas (insumos)
        new("Carne molida de res 80/20", true, MeasureUnits.Kilograms),         // 21
        new("Arrachera marinada", true, MeasureUnits.Kilograms),                // 22
        new("Milanesa de res", true, MeasureUnits.Kilograms),                   // 23
        new("Pechuga de pollo sin hueso", true, MeasureUnits.Kilograms),        // 24
        new("Tiras de pollo", true, MeasureUnits.Kilograms),                    // 25
        new("Milanesa de pollo", true, MeasureUnits.Kilograms),                 // 26
        new("Tocino ahumado", true, MeasureUnits.Kilograms),                    // 27
        new("Jamón de pavo", true, MeasureUnits.Kilograms),                     // 28
        new("Salchicha para asar", true, MeasureUnits.Kilograms),               // 29
        new("Chorizo de cerdo", true, MeasureUnits.Kilograms),                  // 30
        new("Pepperoni", true, MeasureUnits.Kilograms),                         // 31
        new("Huevo fresco", true, MeasureUnits.Units),                      // 32
        new("Medallón de res congelado", true, MeasureUnits.Units),         // 33
        new("Filete de pollo empanizado", true, MeasureUnits.Units),        // 34
        new("Nuggets de pollo congelados", true, MeasureUnits.Kilograms),       // 35

        // 36..45: Panadería y tortillas (insumos)
        new("Pan para hamburguesa con ajonjolí", true, MeasureUnits.Units), // 36
        new("Pan brioche para hamburguesa", true, MeasureUnits.Units),      // 37
        new("Pan de papa para hamburguesa", true, MeasureUnits.Units),      // 38
        new("Pan para hot dog", true, MeasureUnits.Units),                  // 39
        new("Tortilla de harina", true, MeasureUnits.Units),                // 40
        new("Tortilla de maíz", true, MeasureUnits.Units),                  // 41
        new("Totopos de maíz", true, MeasureUnits.Kilograms),                   // 42
        new("Crotones sazonados", true, MeasureUnits.Kilograms),                // 43
        new("Harina de trigo", true, MeasureUnits.Kilograms),                   // 44
        new("Pan molido para empanizar", true, MeasureUnits.Kilograms),         // 45

        // 46..55: Lácteos y quesos (insumos)
        new("Queso americano en rebanadas", true, MeasureUnits.Kilograms),      // 46
        new("Queso cheddar rallado", true, MeasureUnits.Kilograms),             // 47
        new("Queso mozzarella rallado", true, MeasureUnits.Kilograms),          // 48
        new("Queso asadero", true, MeasureUnits.Kilograms),                     // 49
        new("Queso cotija rallado", true, MeasureUnits.Kilograms),              // 50
        new("Queso parmesano rallado", true, MeasureUnits.Kilograms),           // 51
        new("Mantequilla sin sal", true, MeasureUnits.Kilograms),               // 52
        new("Crema ácida", true, MeasureUnits.Liters),                 // 53
        new("Leche entera", true, MeasureUnits.Liters),                // 54
        new("Helado de vainilla", true, MeasureUnits.Liters),          // 55

        // 56..75: Salsas, aderezos, abarrotes y empaques (insumos)
        new("Mayonesa", true, MeasureUnits.Liters),                    // 56
        new("Mayonesa chipotle", true, MeasureUnits.Liters),           // 57
        new("Catsup", true, MeasureUnits.Liters),                      // 58
        new("Mostaza amarilla", true, MeasureUnits.Liters),            // 59
        new("Salsa BBQ", true, MeasureUnits.Liters),                   // 60
        new("Salsa BBQ picante", true, MeasureUnits.Liters),           // 61
        new("Aderezo ranch", true, MeasureUnits.Liters),               // 62
        new("Aderezo césar", true, MeasureUnits.Liters),               // 63
        new("Pepinillos en rodajas", true, MeasureUnits.Kilograms),             // 64
        new("Puré de tomate envasado", true, MeasureUnits.Liters),     // 65
        new("Aceite vegetal", true, MeasureUnits.Liters),              // 66
        new("Manteca vegetal", true, MeasureUnits.Kilograms),                   // 67
        new("Sal refinada", true, MeasureUnits.Kilograms),                      // 68
        new("Pimienta negra molida", true, MeasureUnits.Kilograms),             // 69
        new("Azúcar estándar", true, MeasureUnits.Kilograms),                   // 70
        new("Cajeta quemada", true, MeasureUnits.Liters),              // 71
        new("Papas corte recto congeladas", true, MeasureUnits.Kilograms),      // 72
        new("Frijoles refritos", true, MeasureUnits.Kilograms),                 // 73
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
                new(21, .150m), // Carne molida de res 80/20 (150 g)
                new(46, .030m),  // Queso americano en rebanadas (30 g)
                new(4, 1m),    // Lechuga iceberg (1 unidad)
                new(1, .040m),   // Jitomate saladet (40 g)
                new(5, .025m),   // Cebolla blanca (25 g)
                new(58, .015m),  // Catsup (15 ml)
                new(59, .010m),  // Mostaza amarilla (10 ml)
                new(64, .015m)   // Pepinillos en rodajas (15 g)
            ]
        ),
        new(
            "Hamburguesa doble con tocino",
            145m,
            [
                new(37, 1m),   // Pan brioche para hamburguesa (1 unidad)
                new(21, .300m), // Carne molida de res 80/20 (300 g)
                new(48, .045m),  // Queso mozzarella rallado (45 g)
                new(46, .030m),  // Queso americano en rebanadas (30 g)
                new(27, .040m),  // Tocino ahumado (40 g)
                new(1, .050m),   // Jitomate saladet (50 g)
                new(5, .030m),   // Cebolla blanca (30 g)
                new(60, .025m)   // Salsa BBQ (25 ml)
            ]
        ),
        new(
            "Hamburguesa crispy de pollo",
            110m,
            [
                new(38, 1m),   // Pan de papa para hamburguesa (1 unidad)
                new(26, .160m), // Milanesa de pollo (160 g)
                new(47, .030m),  // Queso cheddar rallado (30 g)
                new(3, 1m),    // Lechuga romana (1 unidad)
                new(9, .025m),   // Pepino (25 g)
                new(57, .020m)   // Mayonesa chipotle (20 ml)
            ]
        ),
        new(
            "Orden de tacos de arrachera",
            115m,
            [
                new(41, 3m),   // Tortilla de maíz (3 unidades)
                new(22, .180m), // Arrachera marinada (180 g)
                new(11, .050m),  // Aguacate hass (50 g)
                new(13, .015m),  // Cilantro fresco (15 g)
                new(6, .025m),   // Cebolla morada (25 g)
                new(12, .020m)   // Limón sin semilla (20 g)
            ]
        ),
        new(
            "Orden de tacos de bistec",
            98m,
            [
                new(41, 3m),   // Tortilla de maíz (3 unidades)
                new(23, .160m), // Milanesa de res (160 g)
                new(7, .040m),   // Cebolla cambray (40 g)
                new(13, .015m),  // Cilantro fresco (15 g)
                new(68, .002m),   // Sal refinada (2 g)
                new(69, .001m)    // Pimienta negra molida (1 g)
            ]
        ),
        new(
            "Burrito norteño de carne",
            118m,
            [
                new(40, 1m),   // Tortilla de harina (1 unidad)
                new(22, .160m), // Arrachera marinada (160 g)
                new(73, .080m),  // Frijoles refritos (80 g)
                new(49, .045m),  // Queso asadero (45 g)
                new(11, .040m),  // Aguacate hass (40 g)
                new(53, .025m),  // Crema ácida (25 ml)
                new(61, .015m)   // Salsa BBQ picante (15 ml)
            ]
        ),
        new(
            "Quesadillas de pollo",
            88m,
            [
                new(40, 2m),   // Tortilla de harina (2 unidades)
                new(25, .140m), // Tiras de pollo (140 g)
                new(49, .060m),  // Queso asadero (60 g)
                new(16, .030m),  // Pimiento morrón (30 g)
                new(62, .025m)   // Aderezo ranch (25 ml)
            ]
        ),
        new(
            "Ensalada César con pollo",
            105m,
            [
                new(3, 1m),    // Lechuga romana (1 unidad)
                new(24, .150m), // Pechuga de pollo sin hueso (150 g)
                new(51, .025m),  // Queso parmesano rallado (25 g)
                new(43, .035m),  // Crotones sazonados (35 g)
                new(63, .040m),  // Aderezo césar (40 ml)
                new(68, .002m),   // Sal refinada (2 g)
                new(69, .001m)    // Pimienta negra molida (1 g)
            ]
        ),
        new(
            "Churros con cajeta",
            68m,
            [
                new(44, .100m), // Harina de trigo (100 g)
                new(67, .040m),  // Manteca vegetal (40 g)
                new(70, .020m),  // Azúcar estándar (20 g)
                new(71, .045m)   // Cajeta quemada (45 ml)
            ]
        ),
        new(
            "Sopa de tortilla",
            72m,
            [
                new(42, .080m),  // Totopos de maíz (80 g)
                new(65, .150m), // Puré de tomate envasado (150 ml)
                new(49, .035m),  // Queso asadero (35 g)
                new(11, .035m),  // Aguacate hass (35 g)
                new(53, .020m),  // Crema ácida (20 ml)
                new(5, .020m)    // Cebolla blanca (20 g)
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
                        new(1, 25m, 30m),  // Jitomate saladet (25 kg @ $30/kg)
                        new(3, 120m, 18m),      // Lechuga romana (120 unidades @ $18/unidad)
                        new(4, 120m, 16m),      // Lechuga iceberg (120 unidades @ $16/unidad)
                        new(5, 20m, 25m), // Cebolla blanca (20 kg @ $0.025/g = $25/kg)
                        new(11, 15m, 70m), // Aguacate hass (15 kg @ $0.07/g = $70/kg)
                        new(21, 50m, 160m), // Carne molida de res 80/20 (50 kg @ $0.16/g = $160/kg)
                        new(22, 35m, 220m), // Arrachera marinada (35 kg @ $0.22/g = $220/kg)
                        new(26, 30m, 130m), // Milanesa de pollo (30 kg @ $0.13/g = $130/kg)
                        new(27, 12m, 190m)  // Tocino ahumado (12 kg @ $0.19/g = $190/kg)
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
                        new(46, 15m, 140m), // Queso americano en rebanadas (15 kg @ $0.14/g)
                        new(48, 15m, 150m), // Queso mozzarella rallado (15 kg @ $0.15/g)
                        new(58, 10m, 40m), // Catsup (10 L @ $0.04/ml)
                        new(76, 240m, 16m),     // Refresco de cola 600 ml (240 unidades @ $16)
                        new(86, 180m, 18m)      // Porción de papas a la francesa (180 unidades @ $18)
                    ]
                ),
                new(
                    "ENT-CEN-003",
                    "Distribuidora Agropecuaria de Sinaloa",
                    [
                        new(2, 8m, 55m),        // Jitomate cherry (8 kg @ $55/kg)
                        new(6, 10m, 30m),       // Cebolla morada (10 kg @ $30/kg)
                        new(7, 6m, 40m),        // Cebolla cambray (6 kg @ $40/kg)
                        new(8, 40m, 22m),       // Papa blanca para freír (40 kg @ $22/kg)
                        new(9, 10m, 20m),       // Pepino (10 kg @ $20/kg)
                        new(10, 6m, 35m),       // Chile jalapeño (6 kg @ $35/kg)
                        new(12, 12m, 28m),      // Limón sin semilla (12 kg @ $28/kg)
                        new(13, 3m, 45m),       // Cilantro fresco (3 kg @ $45/kg)
                        new(14, 8m, 18m),       // Zanahoria (8 kg @ $18/kg)
                        new(15, 5m, 80m),       // Champiñón blanco (5 kg @ $80/kg)
                        new(16, 6m, 60m),       // Pimiento morrón (6 kg @ $60/kg)
                        new(17, 2m, 90m),       // Ajo fresco (2 kg @ $90/kg)
                        new(18, 3m, 120m),      // Espinaca baby (3 kg @ $120/kg)
                        new(19, 6m, 15m),       // Col blanca (6 kg @ $15/kg)
                        new(20, 2m, 50m),       // Perejil fresco (2 kg @ $50/kg)
                        new(23, 25m, 175m),     // Milanesa de res (25 kg @ $175/kg)
                        new(24, 30m, 115m),     // Pechuga de pollo sin hueso (30 kg @ $115/kg)
                        new(25, 25m, 120m),     // Tiras de pollo (25 kg @ $120/kg)
                        new(28, 8m, 110m),      // Jamón de pavo (8 kg @ $110/kg)
                        new(29, 10m, 95m),      // Salchicha para asar (10 kg @ $95/kg)
                        new(30, 8m, 105m),      // Chorizo de cerdo (8 kg @ $105/kg)
                        new(31, 5m, 210m),      // Pepperoni (5 kg @ $210/kg)
                        new(32, 360m, 3m),      // Huevo fresco (360 unidades @ $3)
                        new(33, 200m, 18m),     // Medallón de res congelado (200 unidades @ $18)
                        new(34, 150m, 15m),     // Filete de pollo empanizado (150 unidades @ $15)
                        new(35, 20m, 125m)      // Nuggets de pollo congelados (20 kg @ $125/kg)
                    ]
                ),
                new(
                    "ENT-CEN-004",
                    "Comercializadora de Alimentos del Noroeste",
                    [
                        new(39, 200m, 4.50m),   // Pan para hot dog (200 unidades @ $4.50)
                        new(42, 15m, 60m),      // Totopos de maíz (15 kg @ $60/kg)
                        new(43, 5m, 85m),       // Crotones sazonados (5 kg @ $85/kg)
                        new(44, 25m, 18m),      // Harina de trigo (25 kg @ $18/kg)
                        new(45, 10m, 30m),      // Pan molido para empanizar (10 kg @ $30/kg)
                        new(47, 12m, 155m),     // Queso cheddar rallado (12 kg @ $155/kg)
                        new(49, 15m, 135m),     // Queso asadero (15 kg @ $135/kg)
                        new(50, 5m, 160m),      // Queso cotija rallado (5 kg @ $160/kg)
                        new(51, 5m, 280m),      // Queso parmesano rallado (5 kg @ $280/kg)
                        new(52, 8m, 170m),      // Mantequilla sin sal (8 kg @ $170/kg)
                        new(53, 12m, 55m),      // Crema ácida (12 L @ $55/L)
                        new(54, 30m, 26m),      // Leche entera (30 L @ $26/L)
                        new(55, 20m, 70m),      // Helado de vainilla (20 L @ $70/L)
                        new(56, 12m, 45m),      // Mayonesa (12 L @ $45/L)
                        new(57, 8m, 60m),       // Mayonesa chipotle (8 L @ $60/L)
                        new(59, 6m, 38m),       // Mostaza amarilla (6 L @ $38/L)
                        new(60, 8m, 50m),       // Salsa BBQ (8 L @ $50/L)
                        new(61, 5m, 55m),       // Salsa BBQ picante (5 L @ $55/L)
                        new(62, 8m, 65m),       // Aderezo ranch (8 L @ $65/L)
                        new(63, 6m, 70m),       // Aderezo césar (6 L @ $70/L)
                        new(64, 6m, 48m),       // Pepinillos en rodajas (6 kg @ $48/kg)
                        new(65, 15m, 22m),      // Puré de tomate envasado (15 L @ $22/L)
                        new(66, 60m, 35m),      // Aceite vegetal (60 L @ $35/L)
                        new(67, 15m, 40m),      // Manteca vegetal (15 kg @ $40/kg)
                        new(68, 10m, 12m),      // Sal refinada (10 kg @ $12/kg)
                        new(69, 2m, 180m),      // Pimienta negra molida (2 kg @ $180/kg)
                        new(70, 15m, 25m),      // Azúcar estándar (15 kg @ $25/kg)
                        new(71, 8m, 75m),       // Cajeta quemada (8 L @ $75/L)
                        new(72, 60m, 45m),      // Papas corte recto congeladas (60 kg @ $45/kg)
                        new(73, 20m, 32m),      // Frijoles refritos (20 kg @ $32/kg)
                        new(74, 500m, 2.50m),   // Cajas para hamburguesa (500 unidades @ $2.50)
                        new(75, 500m, 1.20m)    // Bolsas de papel para llevar (500 unidades @ $1.20)
                    ]
                ),
                new(
                    "ENT-CEN-005",
                    "Bebidas y Postres del Pacífico",
                    [
                        new(77, 240m, 16m),     // Refresco de naranja 600 ml (240 unidades @ $16)
                        new(78, 120m, 12m),     // Limonada natural 500 ml (120 unidades @ $12)
                        new(79, 120m, 13m),     // Té helado de durazno 500 ml (120 unidades @ $13)
                        new(80, 100m, 20m),     // Jugo de naranja natural 450 ml (100 unidades @ $20)
                        new(81, 240m, 7m),      // Agua natural embotellada 600 ml (240 unidades @ $7)
                        new(82, 80m, 30m),      // Malteada de vainilla 500 ml (80 unidades @ $30)
                        new(83, 80m, 32m),      // Malteada de chocolate 500 ml (80 unidades @ $32)
                        new(84, 150m, 9m),      // Café americano 355 ml (150 unidades @ $9)
                        new(85, 120m, 14m),     // Capuchino 355 ml (120 unidades @ $14)
                        new(87, 120m, 20m),     // Orden de aros de cebolla (120 unidades @ $20)
                        new(88, 120m, 25m),     // Nachos con queso (120 unidades @ $25)
                        new(89, 60m, 15m),      // Flan napolitano (60 unidades @ $15)
                        new(90, 60m, 18m)       // Rebanada de pastel de chocolate (60 unidades @ $18)
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
                        new(1, 10m),  // Jitomate saladet (10 kg)
                        new(3, 50m),     // Lechuga romana (50 unidades)
                        new(4, 60m),     // Lechuga iceberg (60 unidades)
                        new(5, 8m),   // Cebolla blanca (8 kg)
                        new(11, 6m),  // Aguacate hass (6 kg)
                        new(21, 20m), // Carne molida de res 80/20 (20 kg)
                        new(22, 15m), // Arrachera marinada (15 kg)
                        new(26, 12m), // Milanesa de pollo (12 kg)
                        new(27, 5m)   // Tocino ahumado (5 kg)
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
                        new(46, 6m),  // Queso americano en rebanadas (6 kg)
                        new(48, 6m),  // Queso mozzarella rallado (6 kg)
                        new(58, 4m),  // Catsup (4 L)
                        new(76, 100m),   // Refresco de cola 600 ml (100 unidades)
                        new(86, 80m)     // Porción de papas a la francesa (80 unidades)
                    ]
                ),
                new(
                    "ENT-CAN-003",
                    "Distribuidora Agropecuaria de Sinaloa",
                    [
                        new(2, 3m),      // Jitomate cherry (3 kg)
                        new(6, 4m),      // Cebolla morada (4 kg)
                        new(7, 2.5m),    // Cebolla cambray (2.5 kg)
                        new(8, 15m),     // Papa blanca para freír (15 kg)
                        new(9, 4m),      // Pepino (4 kg)
                        new(10, 2.5m),   // Chile jalapeño (2.5 kg)
                        new(12, 5m),     // Limón sin semilla (5 kg)
                        new(13, 1.2m),   // Cilantro fresco (1.2 kg)
                        new(14, 3m),     // Zanahoria (3 kg)
                        new(15, 2m),     // Champiñón blanco (2 kg)
                        new(16, 2.5m),   // Pimiento morrón (2.5 kg)
                        new(17, .8m),    // Ajo fresco (800 g)
                        new(18, 1.2m),   // Espinaca baby (1.2 kg)
                        new(19, 2.5m),   // Col blanca (2.5 kg)
                        new(20, .8m),    // Perejil fresco (800 g)
                        new(23, 10m),    // Milanesa de res (10 kg)
                        new(24, 12m),    // Pechuga de pollo sin hueso (12 kg)
                        new(25, 10m),    // Tiras de pollo (10 kg)
                        new(28, 3m),     // Jamón de pavo (3 kg)
                        new(29, 4m),     // Salchicha para asar (4 kg)
                        new(30, 3m),     // Chorizo de cerdo (3 kg)
                        new(31, 2m),     // Pepperoni (2 kg)
                        new(32, 150m),   // Huevo fresco (150 unidades)
                        new(33, 80m),    // Medallón de res congelado (80 unidades)
                        new(34, 60m),    // Filete de pollo empanizado (60 unidades)
                        new(35, 8m)      // Nuggets de pollo congelados (8 kg)
                    ]
                ),
                new(
                    "ENT-CAN-004",
                    "Comercializadora de Alimentos del Noroeste",
                    [
                        new(39, 80m),    // Pan para hot dog (80 unidades)
                        new(42, 6m),     // Totopos de maíz (6 kg)
                        new(43, 2m),     // Crotones sazonados (2 kg)
                        new(44, 10m),    // Harina de trigo (10 kg)
                        new(45, 4m),     // Pan molido para empanizar (4 kg)
                        new(47, 5m),     // Queso cheddar rallado (5 kg)
                        new(49, 6m),     // Queso asadero (6 kg)
                        new(50, 2m),     // Queso cotija rallado (2 kg)
                        new(51, 2m),     // Queso parmesano rallado (2 kg)
                        new(52, 3m),     // Mantequilla sin sal (3 kg)
                        new(53, 5m),     // Crema ácida (5 L)
                        new(54, 12m),    // Leche entera (12 L)
                        new(55, 8m),     // Helado de vainilla (8 L)
                        new(56, 5m),     // Mayonesa (5 L)
                        new(57, 3m),     // Mayonesa chipotle (3 L)
                        new(59, 2.5m),   // Mostaza amarilla (2.5 L)
                        new(60, 3m),     // Salsa BBQ (3 L)
                        new(61, 2m),     // Salsa BBQ picante (2 L)
                        new(62, 3m),     // Aderezo ranch (3 L)
                        new(63, 2.5m),   // Aderezo césar (2.5 L)
                        new(64, 2.5m),   // Pepinillos en rodajas (2.5 kg)
                        new(65, 6m),     // Puré de tomate envasado (6 L)
                        new(66, 25m),    // Aceite vegetal (25 L)
                        new(67, 6m),     // Manteca vegetal (6 kg)
                        new(68, 4m),     // Sal refinada (4 kg)
                        new(69, .8m),    // Pimienta negra molida (800 g)
                        new(70, 6m),     // Azúcar estándar (6 kg)
                        new(71, 3m),     // Cajeta quemada (3 L)
                        new(72, 25m),    // Papas corte recto congeladas (25 kg)
                        new(73, 8m),     // Frijoles refritos (8 kg)
                        new(74, 200m),   // Cajas para hamburguesa (200 unidades)
                        new(75, 200m)    // Bolsas de papel para llevar (200 unidades)
                    ]
                ),
                new(
                    "ENT-CAN-005",
                    "Bebidas y Postres del Pacífico",
                    [
                        new(77, 100m),   // Refresco de naranja 600 ml (100 unidades)
                        new(78, 50m),    // Limonada natural 500 ml (50 unidades)
                        new(79, 50m),    // Té helado de durazno 500 ml (50 unidades)
                        new(80, 40m),    // Jugo de naranja natural 450 ml (40 unidades)
                        new(81, 100m),   // Agua natural embotellada 600 ml (100 unidades)
                        new(82, 30m),    // Malteada de vainilla 500 ml (30 unidades)
                        new(83, 30m),    // Malteada de chocolate 500 ml (30 unidades)
                        new(84, 60m),    // Café americano 355 ml (60 unidades)
                        new(85, 50m),    // Capuchino 355 ml (50 unidades)
                        new(87, 50m),    // Orden de aros de cebolla (50 unidades)
                        new(88, 50m),    // Nachos con queso (50 unidades)
                        new(89, 25m),    // Flan napolitano (25 unidades)
                        new(90, 25m)     // Rebanada de pastel de chocolate (25 unidades)
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
                        new(21, 10m), // Carne molida de res 80/20 (10 kg)
                        new(22, 8m),  // Arrachera marinada (8 kg)
                        new(36, 80m),    // Pan para hamburguesa con ajonjolí (80 unidades)
                        new(46, 3m),  // Queso americano en rebanadas (3 kg)
                        new(76, 60m)     // Refresco de cola 600 ml (60 unidades)
                    ]
                )
            ],

            [2] =
            [
                new(
                    "MERMA",
                    [
                        new(1, 2.5m),   // Jitomate saladet (2.5 kg)
                        new(4, 15m),     // Lechuga iceberg (15 unidades)
                        new(21, 6m),  // Carne molida de res 80/20 (6 kg)
                        new(36, 40m),    // Pan para hamburguesa con ajonjolí (40 unidades)
                        new(46, 1.2m),  // Queso americano en rebanadas (1.2 kg)
                        new(58, .6m),   // Catsup (600 ml)
                        new(76, 30m),    // Refresco de cola 600 ml (30 unidades)
                        new(86, 25m)     // Porción de papas a la francesa (25 unidades)
                    ]
                )
            ]
        };
}
