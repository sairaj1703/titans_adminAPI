using System.ComponentModel.DataAnnotations;

namespace titans_admin.Models.Entities;

public class TradeProgram
{
    [Key]
    public int TradeProgramId { get; set; }

    [Required, MaxLength(200)]
    public string ProgramName { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Description { get; set; }

    [Required]
    public DateTime StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    [Required, MaxLength(50)]
    public string Status { get; set; } = "Active";

    [MaxLength(100)]
    public string? ProgramType { get; set; }

    public decimal? Budget { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public ICollection<ComplianceRecord> ComplianceRecords { get; set; } = [];
}
