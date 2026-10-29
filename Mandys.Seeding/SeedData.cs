using Mandys.Domain;

namespace Mandys.Seeding;

public sealed record BranchSeed(
    int Id,
    string Name,
    string Address,
    bool WarehouseOnly
);

public sealed record UserSeed(
    string Email,
    string FirstName,
    string FatherLastName,
    string Role,
    int? BranchId = null,
    string? MotherLastName = null
);

public sealed record ProductSeed(
    int Id,
    string Description,
    bool IsSupply,
    string MeasureUnit,
    decimal? SalePrice = null
);

public sealed record DishRecipeLineSeed(
    int ProductId,
    decimal Quantity
);

public sealed record DishSeed(
    int Id,
    string Name,
    decimal Price,
    IReadOnlyList<DishRecipeLineSeed> Recipe
);

public sealed record ComboDishLineSeed(
    int DishId,
    decimal Quantity
);

public sealed record ComboProductLineSeed(
    int ProductId,
    decimal Quantity
);

public sealed record ComboSeed(
    string Name,
    decimal Price,
    IReadOnlyList<ComboDishLineSeed> Dishes,
    IReadOnlyList<ComboProductLineSeed> Products
);

public static class SeedData
{
    public static readonly IReadOnlyList<BranchSeed> Branches =
    [
        new(1, "Sucursal Culiacán Centro", "Av. Río Presa 1200, Centro, Culiacán", true),
        new(2, "Sucursal Cañadas", "Sierra Madre Oriental 2222", false)
    ];

    public static readonly IReadOnlyList<UserSeed> Users =
    [
        new("operaciones@mandyspos.com", "Gerente", "Operaciones", Roles.OpChief, 1),
        new("almacen.central@mandyspos.com", "Encargado", "Almacén Central", Roles.CentralWarehouseChief, 1),
        new("almacen@mandyspos.com", "Encargado", "Almacén", Roles.WarehouseChief, 1),
        new("caja@mandyspos.com", "Cajero", "Sucursal", Roles.Cashier, 1),
        new("cocina@mandyspos.com", "Jefe", "Cocina", Roles.KitchenChief, 1),
        new("sucursal@mandyspos.com", "Gerente", "Sucursal", Roles.BranchChief, 1)
    ];

    public static readonly IReadOnlyList<ProductSeed> Products =
    [
        new(1, "Jitomate saladet", true, MeasureUnits.Grams),
        new(2, "Jitomate cherry", true, MeasureUnits.Grams),
        new(3, "Lechuga romana", true, MeasureUnits.Units),
        new(4, "Lechuga iceberg", true, MeasureUnits.Units),
        new(5, "Cebolla blanca", true, MeasureUnits.Grams),
        new(6, "Cebolla morada", true, MeasureUnits.Grams),
        new(7, "Cebolla cambray", true, MeasureUnits.Grams),
        new(8, "Papa blanca para freír", true, MeasureUnits.Grams),
        new(9, "Pepino", true, MeasureUnits.Grams),
        new(10, "Chile jalapeño", true, MeasureUnits.Grams),
        new(11, "Aguacate hass", true, MeasureUnits.Grams),
        new(12, "Limón sin semilla", true, MeasureUnits.Grams),
        new(13, "Cilantro fresco", true, MeasureUnits.Grams),
        new(14, "Zanahoria", true, MeasureUnits.Grams),
        new(15, "Champiñón blanco", true, MeasureUnits.Grams),
        new(16, "Pimiento morrón verde", true, MeasureUnits.Grams),
        new(17, "Pimiento morrón rojo", true, MeasureUnits.Grams),
        new(18, "Apio", true, MeasureUnits.Grams),
        new(19, "Ajo fresco", true, MeasureUnits.Grams),
        new(20, "Elote amarillo", true, MeasureUnits.Units),
        new(21, "Calabacita", true, MeasureUnits.Grams),
        new(22, "Espinaca baby", true, MeasureUnits.Grams),
        new(23, "Col blanca", true, MeasureUnits.Grams),
        new(24, "Rábano", true, MeasureUnits.Grams),
        new(25, "Nopal limpio", true, MeasureUnits.Grams),
        new(26, "Piña miel", true, MeasureUnits.Grams),
        new(27, "Mango ataulfo", true, MeasureUnits.Grams),
        new(28, "Fresa", true, MeasureUnits.Grams),
        new(29, "Plátano tabasco", true, MeasureUnits.Grams),
        new(30, "Manzana roja", true, MeasureUnits.Grams),
        new(31, "Naranja para jugo", true, MeasureUnits.Grams),
        new(32, "Perejil fresco", true, MeasureUnits.Grams),
        new(33, "Epazote fresco", true, MeasureUnits.Grams),
        new(34, "Chayote", true, MeasureUnits.Grams),
        new(35, "Betabel", true, MeasureUnits.Grams),
        new(36, "Carne molida de res 80/20", true, MeasureUnits.Grams),
        new(37, "Carne molida de res 90/10", true, MeasureUnits.Grams),
        new(38, "Arrachera marinada", true, MeasureUnits.Grams),
        new(39, "Milanesa de res", true, MeasureUnits.Grams),
        new(40, "Pechuga de pollo sin hueso", true, MeasureUnits.Grams),
        new(41, "Muslo de pollo sin hueso", true, MeasureUnits.Grams),
        new(42, "Tiras de pollo", true, MeasureUnits.Grams),
        new(43, "Milanesa de pollo", true, MeasureUnits.Grams),
        new(44, "Tocino ahumado", true, MeasureUnits.Grams),
        new(45, "Jamón de pavo", true, MeasureUnits.Grams),
        new(46, "Chuleta ahumada", true, MeasureUnits.Grams),
        new(47, "Salchicha para asar", true, MeasureUnits.Grams),
        new(48, "Longaniza", true, MeasureUnits.Grams),
        new(49, "Pepperoni", true, MeasureUnits.Grams),
        new(50, "Huevo fresco", true, MeasureUnits.Grams),
        new(51, "Huevo líquido pasteurizado", true, MeasureUnits.Milliliters),
        new(52, "Filete de pescado empanizado", true, MeasureUnits.Grams),
        new(53, "Camarón mediano sin cáscara", true, MeasureUnits.Grams),
        new(54, "Atún enlatado", true, MeasureUnits.Units),
        new(55, "Costilla de cerdo", true, MeasureUnits.Grams),
        new(56, "Pulled pork", true, MeasureUnits.Grams),
        new(57, "Salami", true, MeasureUnits.Grams),
        new(58, "Chorizo argentino", true, MeasureUnits.Grams),
        new(59, "Pavo molido", true, MeasureUnits.Grams),
        new(60, "Lomo de cerdo", true, MeasureUnits.Grams),
        new(61, "Pan para hamburguesa con ajonjolí", true, MeasureUnits.Units),
        new(62, "Pan brioche para hamburguesa", true, MeasureUnits.Units),
        new(63, "Pan integral para hamburguesa", true, MeasureUnits.Units),
        new(64, "Pan de papa para hamburguesa", true, MeasureUnits.Units),
        new(65, "Pan para hot dog", true, MeasureUnits.Units),
        new(66, "Pan telera", true, MeasureUnits.Units),
        new(67, "Pan pita", true, MeasureUnits.Units),
        new(68, "Tortilla de harina", true, MeasureUnits.Units),
        new(69, "Tortilla de maíz", true, MeasureUnits.Units),
        new(70, "Totopos de maíz", true, MeasureUnits.Grams),
        new(71, "Pan molido para empanizar", true, MeasureUnits.Grams),
        new(72, "Crotones sazonados", true, MeasureUnits.Grams),
        new(73, "Queso americano en rebanadas", true, MeasureUnits.Grams),
        new(74, "Queso cheddar rallado", true, MeasureUnits.Grams),
        new(75, "Queso mozzarella rallado", true, MeasureUnits.Grams),
        new(76, "Queso gouda en rebanadas", true, MeasureUnits.Grams),
        new(77, "Queso asadero", true, MeasureUnits.Grams),
        new(78, "Queso cotija rallado", true, MeasureUnits.Grams),
        new(79, "Queso crema", true, MeasureUnits.Grams),
        new(80, "Mantequilla sin sal", true, MeasureUnits.Grams),
        new(81, "Crema ácida", true, MeasureUnits.Milliliters),
        new(82, "Leche entera", true, MeasureUnits.Milliliters),
        new(83, "Leche deslactosada", true, MeasureUnits.Milliliters),
        new(84, "Yogur natural", true, MeasureUnits.Milliliters),
        new(85, "Mayonesa", true, MeasureUnits.Milliliters),
        new(86, "Mayonesa chipotle", true, MeasureUnits.Milliliters),
        new(87, "Catsup", true, MeasureUnits.Milliliters),
        new(88, "Mostaza amarilla", true, MeasureUnits.Milliliters),
        new(89, "Mostaza dijon", true, MeasureUnits.Milliliters),
        new(90, "Salsa BBQ", true, MeasureUnits.Milliliters),
        new(91, "Salsa BBQ picante", true, MeasureUnits.Milliliters),
        new(92, "Aderezo ranch", true, MeasureUnits.Milliliters),
        new(93, "Aderezo césar", true, MeasureUnits.Milliliters),
        new(94, "Aderezo mil islas", true, MeasureUnits.Milliliters),
        new(95, "Salsa de chipotle", true, MeasureUnits.Milliliters),
        new(96, "Salsa inglesa", true, MeasureUnits.Milliliters),
        new(97, "Salsa de soya", true, MeasureUnits.Milliliters),
        new(98, "Salsa verde envasada", true, MeasureUnits.Milliliters),
        new(99, "Salsa roja de árbol envasada", true, MeasureUnits.Milliliters),
        new(100, "Salsa de habanero envasada", true, MeasureUnits.Milliliters),
        new(101, "Salsa teriyaki", true, MeasureUnits.Milliliters),
        new(102, "Salsa agridulce", true, MeasureUnits.Milliliters),
        new(103, "Alioli de ajo", true, MeasureUnits.Milliliters),
        new(104, "Salsa de tamarindo", true, MeasureUnits.Milliliters),
        new(105, "Sal refinada", true, MeasureUnits.Grams),
        new(106, "Sal de mar", true, MeasureUnits.Grams),
        new(107, "Pimienta negra molida", true, MeasureUnits.Grams),
        new(108, "Paprika", true, MeasureUnits.Grams),
        new(109, "Ajo en polvo", true, MeasureUnits.Grams),
        new(110, "Cebolla en polvo", true, MeasureUnits.Grams),
        new(111, "Orégano seco", true, MeasureUnits.Grams),
        new(112, "Comino molido", true, MeasureUnits.Grams),
        new(113, "Hoja de laurel", true, MeasureUnits.Grams),
        new(114, "Tomillo seco", true, MeasureUnits.Grams),
        new(115, "Romero seco", true, MeasureUnits.Grams),
        new(116, "Albahaca seca", true, MeasureUnits.Grams),
        new(117, "Perejil seco", true, MeasureUnits.Grams),
        new(118, "Chile de árbol seco", true, MeasureUnits.Grams),
        new(119, "Chile chipotle seco", true, MeasureUnits.Grams),
        new(120, "Consomé de pollo en polvo", true, MeasureUnits.Grams),
        new(121, "Sazonador tipo tajín", true, MeasureUnits.Grams),
        new(122, "Azúcar estándar", true, MeasureUnits.Grams),
        new(123, "Azúcar mascabado", true, MeasureUnits.Grams),
        new(124, "Miel de abeja", true, MeasureUnits.Milliliters),
        new(125, "Aceite vegetal", true, MeasureUnits.Milliliters),
        new(126, "Aceite de oliva extra virgen", true, MeasureUnits.Milliliters),
        new(127, "Aceite en aerosol", true, MeasureUnits.Units),
        new(128, "Vinagre blanco", true, MeasureUnits.Milliliters),
        new(129, "Vinagre de manzana", true, MeasureUnits.Milliliters),
        new(130, "Vinagre balsámico", true, MeasureUnits.Milliliters),
        new(131, "Jugo de limón embotellado", true, MeasureUnits.Milliliters),
        new(132, "Manteca vegetal", true, MeasureUnits.Grams),
        new(133, "Refresco de cola en lata", true, MeasureUnits.Units),
        new(134, "Refresco de naranja en lata", true, MeasureUnits.Units),
        new(135, "Refresco de limón en lata", true, MeasureUnits.Units),
        new(136, "Refresco de manzana en lata", true, MeasureUnits.Units),
        new(137, "Agua embotellada 600ml", true, MeasureUnits.Units),
        new(138, "Agua mineral 600ml", true, MeasureUnits.Units),
        new(139, "Jugo de naranja envasado", true, MeasureUnits.Milliliters),
        new(140, "Café molido americano", true, MeasureUnits.Grams),
        new(141, "Café descafeinado molido", true, MeasureUnits.Grams),
        new(142, "Té negro en bolsitas", true, MeasureUnits.Units),
        new(143, "Té verde en bolsitas", true, MeasureUnits.Units),
        new(144, "Chocolate en polvo", true, MeasureUnits.Grams),
        new(145, "Jarabe de vainilla", true, MeasureUnits.Milliliters),
        new(146, "Jarabe de caramelo", true, MeasureUnits.Milliliters),
        new(147, "Crema para café en polvo", true, MeasureUnits.Grams),
        new(148, "Mermelada de fresa", true, MeasureUnits.Grams),
        new(149, "Papas corte recto congeladas", true, MeasureUnits.Grams),
        new(150, "Papas gajo congeladas", true, MeasureUnits.Grams),
        new(151, "Aros de cebolla congelados", true, MeasureUnits.Grams),
        new(152, "Nuggets de pollo congelados", true, MeasureUnits.Grams),
        new(153, "Palitos de queso mozzarella congelados", true, MeasureUnits.Grams),
        new(154, "Medallón de res congelado", true, MeasureUnits.Units),
        new(155, "Filete de pollo empanizado congelado", true, MeasureUnits.Units),
        new(156, "Elote amarillo congelado", true, MeasureUnits.Grams),
        new(157, "Mezcla de verduras congeladas", true, MeasureUnits.Grams),
        new(158, "Pulpa de mango congelada", true, MeasureUnits.Grams),
        new(159, "Fresa congelada", true, MeasureUnits.Grams),
        new(160, "Helado de vainilla", true, MeasureUnits.Milliliters),
        new(161, "Hielo en bolsa 5Gramos", true, MeasureUnits.Units),
        new(162, "Masa de pizza congelada", true, MeasureUnits.Units),
        new(163, "Harina de trigo", true, MeasureUnits.Grams),
        new(164, "Fécula de maíz", true, MeasureUnits.Grams),
        new(165, "Arroz blanco", true, MeasureUnits.Grams),
        new(166, "Frijoles refritos en lata", true, MeasureUnits.Units),
        new(167, "Elote enlatado", true, MeasureUnits.Units),
        new(168, "Champiñones enlatados", true, MeasureUnits.Units),
        new(169, "Chiles jalapeños enlatados", true, MeasureUnits.Units),
        new(170, "Aceitunas negras enlatadas", true, MeasureUnits.Units),
        new(171, "Pepinillos en rodajas", true, MeasureUnits.Milliliters),
        new(172, "Puré de tomate envasado", true, MeasureUnits.Milliliters),
        new(173, "Cacahuate tostado", true, MeasureUnits.Grams),
        new(174, "Ajonjolí", true, MeasureUnits.Grams),
        new(175, "Cajeta quemada", true, MeasureUnits.Milliliters),
        new(176, "Chamoy envasado", true, MeasureUnits.Milliliters),
        new(177, "Vasos de cartón 12oz", true, MeasureUnits.Units),
        new(178, "Vasos de cartón 16oz", true, MeasureUnits.Units),
        new(179, "Tapas para vaso", true, MeasureUnits.Units),
        new(180, "Popotes de papel", true, MeasureUnits.Units),
        new(181, "Charolas de cartón", true, MeasureUnits.Units),
        new(182, "Cajas para hamburguesa", true, MeasureUnits.Units),
        new(183, "Envolturas de papel encerado", true, MeasureUnits.Units),
        new(184, "Bolsas de papel para llevar", true, MeasureUnits.Units),
        new(185, "Servilletas 500 piezas", true, MeasureUnits.Units),
        new(186, "Toallas de papel en rollo", true, MeasureUnits.Units),
        new(187, "Guantes desechables", true, MeasureUnits.Units),
        new(188, "Papel aluminio en rollo", true, MeasureUnits.Units),
        new(189, "Película plástica adherente", true, MeasureUnits.Units),
        new(190, "Jabón líquido para manos", true, MeasureUnits.Milliliters),
        new(191, "Queso parmesano rallado", true, MeasureUnits.Grams),
        new(192, "Salsa sriracha", true, MeasureUnits.Milliliters),
        new(193, "Aderezo de mostaza y miel", true, MeasureUnits.Milliliters),
        new(194, "Jugo de manzana envasado", true, MeasureUnits.Milliliters),
        new(195, "Agua tónica en lata", true, MeasureUnits.Units),
        new(196, "Té de manzanilla en bolsitas", true, MeasureUnits.Units),
        new(197, "Champiñón portobello", true, MeasureUnits.Grams),
        new(198, "Cebolla perla", true, MeasureUnits.Grams),
        new(199, "Manteca de cerdo", true, MeasureUnits.Grams),
        new(200, "Queso oaxaca", true, MeasureUnits.Grams),
        new(201, "Refresco de cola 600 ml", false, MeasureUnits.Units, 35m),
        new(202, "Refresco de naranja 600 ml", false, MeasureUnits.Units, 35m),
        new(203, "Limonada de limón natural 500 ml", false, MeasureUnits.Units, 38m),
        new(204, "Té de manzanilla 500 ml", false, MeasureUnits.Units, 32m),
        new(205, "Jugo de naranja natural 450 ml", false, MeasureUnits.Units, 48m),
        new(206, "Batido de fresa 450 ml", false, MeasureUnits.Units, 52m),
        new(207, "Malteada de vainilla 500 ml", false, MeasureUnits.Units, 78m),
        new(208, "Espresso", false, MeasureUnits.Units, 28m),
        new(209, "Capuchino", false, MeasureUnits.Units, 45m),
        new(210, "Porción de papas fritas", false, MeasureUnits.Units, 45m),
        new(211, "Nachos con queso", false, MeasureUnits.Units, 69m),
        new(212, "Alitas de pollo Buffalo", false, MeasureUnits.Units, 89m),
        new(213, "Bowl de pollo César", false, MeasureUnits.Units, 118m),
        new(214, "Flan de caramelo", false, MeasureUnits.Units, 42m),
        new(215, "Pastel de chocolate", false, MeasureUnits.Units, 48m)
    ];

    public static readonly IReadOnlyList<DishSeed> Dishes =
    [
        new(
            1,
            "Hamburguesa clásica",
            85m,
            [
                new(61, 1m),
                new(36, 0.15m),
                new(73, 0.04m),
                new(4, 2m),
                new(1, 0.05m),
                new(5, 0.03m),
                new(87, 0.02m),
                new(88, 0.01m),
                new(171, 0.02m)
            ]
        ),
        new(
            2,
            "Hamburguesa doble",
            125m,
            [
                new(62, 1m),
                new(36, 0.3m),
                new(75, 0.06m),
                new(73, 0.04m),
                new(1, 0.08m),
                new(5, 0.04m),
                new(44, 0.05m),
                new(90, 0.03m)
            ]
        ),
        new(
            3,
            "Hamburguesa de pollo",
            92m,
            [
                new(64, 1m),
                new(43, 0.18m),
                new(78, 0.03m),
                new(3, 2m),
                new(9, 0.04m),
                new(86, 0.02m)
            ]
        ),
        new(
            4,
            "Taco al pastor",
            48m,
            [
                new(69, 2m),
                new(38, 0.15m),
                new(10, 0.02m),
                new(13, 0.05m),
                new(2, 0.05m),
                new(6, 0.03m),
                new(19, 0.01m)
            ]
        ),
        new(
            5,
            "Taco de bistec",
            52m,
            [
                new(69, 2m),
                new(39, 0.12m),
                new(7, 0.08m),
                new(32, 0.03m),
                new(105, 0.002m),
                new(107, 0.001m)
            ]
        ),
        new(
            6,
            "Burrito de carne",
            98m,
            [
                new(68, 1m),
                new(36, 0.18m),
                new(80, 0.01m),
                new(172, 0.08m),
                new(22, 0.03m),
                new(14, 0.03m),
                new(82, 0.05m),
                new(77, 0.04m),
                new(91, 0.02m)
            ]
        ),
        new(
            7,
            "Quesadilla de pollo",
            78m,
            [
                new(68, 2m),
                new(42, 0.15m),
                new(73, 0.08m),
                new(17, 0.04m),
                new(92, 0.03m),
                new(125, 0.01m)
            ]
        ),
        new(
            8,
            "Ensalada César",
            95m,
            [
                new(22, 0.08m),
                new(23, 0.06m),
                new(74, 0.04m),
                new(93, 0.05m),
                new(171, 0.01m),
                new(71, 0.03m),
                new(105, 0.002m),
                new(107, 0.001m)
            ]
        ),
        new(
            9,
            "Churros con cajeta",
            65m,
            [
                new(163, 0.1m),
                new(132, 0.05m),
                new(105, 0.002m),
                new(175, 0.08m)
            ]
        ),
        new(
            10,
            "Sopa de tortilla",
            55m,
            [
                new(1, 0.2m),
                new(69, 2m),
                new(5, 0.06m),
                new(19, 0.01m),
                new(125, 0.02m),
                new(80, 0.01m),
                new(32, 0.02m)
            ]
        )
    ];

    public static readonly IReadOnlyList<ComboSeed> Combos =
    [
        new(
            "Combo Hamburguesa clásica",
            129m,
            [new(1, 1m)],
            [new(201, 1m), new(210, 1m)]
        ),
        new(
            "Combo Hamburguesa doble",
            169m,
            [new(2, 1m)],
            [new(202, 1m), new(210, 1m)]
        ),
        new(
            "Combo Hamburguesa de pollo",
            139m,
            [new(3, 1m)],
            [new(203, 1m), new(210, 1m)]
        ),
        new(
            "Combo Tacos al pastor",
            119m,
            [new(4, 2m)],
            [new(201, 1m)]
        ),
        new(
            "Combo Tacos de bistec",
            125m,
            [new(5, 2m)],
            [new(202, 1m)]
        ),
        new(
            "Combo Burrito",
            145m,
            [new(6, 1m)],
            [new(204, 1m), new(211, 1m)]
        ),
        new(
            "Combo Quesadillas",
            125m,
            [new(7, 2m)],
            [new(205, 1m)]
        ),
        new(
            "Combo Ensalada",
            119m,
            [new(8, 1m)],
            [new(203, 1m)]
        ),
        new(
            "Combo Postre",
            119m,
            [new(9, 1m)],
            [new(209, 1m), new(215, 1m)]
        ),
        new(
            "Combo Familiar",
            349m,
            [new(1, 2m), new(3, 2m)],
            [new(201, 4m), new(210, 2m)]
        )
    ];
}
