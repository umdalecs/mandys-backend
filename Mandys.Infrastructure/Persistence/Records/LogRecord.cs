using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mandys.Infrastructure.Persistence.Records;

public class LogRecord : Record
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    [Required]
    [MaxLength(200)]
    public string Verb { get; set; } = string.Empty;
    [Required]
    [MaxLength(200)]
    public string Description { get; set; } = string.Empty;
}
