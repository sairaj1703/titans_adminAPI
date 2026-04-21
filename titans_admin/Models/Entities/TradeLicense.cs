using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace titans_admin.Models.Entities;

public class TradeLicense
{
    [Key]
    public int TradeLicenseId { get; set; }

    [Required]
    public int UserId { get; set; }

    [ForeignKey(nameof(UserId))]
    public User User { get; set; } = null!;

    [Required, MaxLength(50)]
    public string LicenseNumber { get; set; } = string.Empty;

    [Required, MaxLength(200)]
    public string BusinessName { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? BusinessType { get; set; }

    [Required]
    public DateTime IssueDate { get; set; }

    [Required]
    public DateTime ExpiryDate { get; set; }

    [Required, MaxLength(50)]
    public string Status { get; set; } = "Pending";

    [MaxLength(500)]
    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}
