using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace titans_admin.Models.Entities;

public class AuditLog
{
    [Key]
    public int AuditLogId { get; set; }

    public int? UserId { get; set; }

    [ForeignKey(nameof(UserId))]
    public User? User { get; set; }

    [Required, MaxLength(100)]
    public string Action { get; set; } = string.Empty;

    [Required, MaxLength(255)]
    public string Resource { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Details { get; set; }

    [Required]
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    [MaxLength(45)]
    public string? IpAddress { get; set; }

    [MaxLength(255)]
    public string? UserAgent { get; set; }
}
