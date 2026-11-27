using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Mandys.Domain;

namespace Mandys.Infrastructure.Persistence.Records;

public class OrderRecord : Record
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    [Required]
    public int BranchId { get; set; }
    public BranchRecord Branch { get; set; } = null!;
    [Required]
    public int CreatedByUserId { get; set; }
    public UserRecord CreatedByUser { get; set; } = null!;
    public int? CustomerId { get; set; }
    public UserRecord? Customer { get; set; }
    [Required]
    [MaxLength(20)]
    public string PaymentMethod { get; set; } = string.Empty;
    [MaxLength(4)]
    public string? CardLast4 { get; set; }
    [Required]
    [MaxLength(30)]
    public string Status { get; set; } = OrderStatuses.InPreparation;
    public DateTime? CompletedAt { get; set; }
    [Required]
    public decimal SubtotalAmount { get; set; }
    [Required]
    public decimal PointsSpent { get; set; } = 0m;
    [Required]
    public decimal TotalAmount { get; set; }
    [Required]
    public decimal PointsEarned { get; set; } = 0m;
    public ICollection<OrderProductRecord> OrderProducts { get; set; } = new List<OrderProductRecord>();
    public ICollection<OrderDishRecord> OrderDishes { get; set; } = new List<OrderDishRecord>();
    public ICollection<OrderComboRecord> OrderCombos { get; set; } = new List<OrderComboRecord>();
}

public class OrderProductRecord : Record
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    [Required]
    public int OrderId { get; set; }
    public OrderRecord Order { get; set; } = null!;
    [Required]
    public int ProductId { get; set; }
    public ProductRecord Product { get; set; } = null!;
    [Required]
    public decimal Quantity { get; set; }
    [Required]
    public decimal UnitPrice { get; set; }
}

public class OrderDishRecord : Record
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    [Required]
    public int OrderId { get; set; }
    public OrderRecord Order { get; set; } = null!;
    [Required]
    public int DishId { get; set; }
    public DishRecord Dish { get; set; } = null!;
    [Required]
    public decimal Quantity { get; set; }
    [Required]
    public decimal UnitPrice { get; set; }
}

public class OrderComboRecord : Record
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    [Required]
    public int OrderId { get; set; }
    public OrderRecord Order { get; set; } = null!;
    [Required]
    public int ComboId { get; set; }
    public ComboRecord Combo { get; set; } = null!;
    [Required]
    public decimal Quantity { get; set; }
    [Required]
    public decimal UnitPrice { get; set; }
}
