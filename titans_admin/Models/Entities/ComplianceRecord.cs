using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using titans_admin.Models.Enums;

namespace titans_admin.Models.Entities;

public class ComplianceRecord
{
    [Key]
    public int ComplianceRecordId { get; set; }

    [Required]
    public int TradeProgramId { get; set; }

    [ForeignKey(nameof(TradeProgramId))]
    public TradeProgram TradeProgram { get; set; } = null!;

    public int? ReviewedByUserId { get; set; }

    [ForeignKey(nameof(ReviewedByUserId))]
    public User? ReviewedBy { get; set; }

    [Required]
    public ComplianceResult Result { get; set; } = ComplianceResult.UnderReview;

    [Required]
    public DateTime ReviewDate { get; set; } = DateTime.UtcNow;

    [MaxLength(1000)]
    public string? Findings { get; set; }

    [MaxLength(1000)]
    public string? Recommendations { get; set; }

    public DateTime? FollowUpDate { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}
