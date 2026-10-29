using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mandys.Infrastructure.Persistence.Records;

public class ComboRecord : Record
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;
    [Required]
    public decimal Price { get; set; }
    public string? ImageUrl { get; set; }
    public ICollection<ComboDishRecord> ComboDishes { get; set; } = new List<ComboDishRecord>();
    public ICollection<ComboProductRecord> ComboProducts { get; set; } = new List<ComboProductRecord>();
}
