using System.ComponentModel.DataAnnotations;
using titans_admin.Models.Enums;

namespace titans_admin.Models.ViewModels
{
    public class ComplianceRecordEditViewModel
    {
        public int ComplianceRecordId { get; set; }

        [Required]
        public int TradeProgramId { get; set; }

        public int? ReviewedByUserId { get; set; }

        [Required]
        public ComplianceResult Result { get; set; } = ComplianceResult.UnderReview;

        [Required]
        [DataType(DataType.Date)]
        public DateTime ReviewDate { get; set; } = DateTime.UtcNow;

        [MaxLength(1000)]
        public string? Findings { get; set; }

        [MaxLength(1000)]
        public string? Recommendations { get; set; }

        [DataType(DataType.Date)]
        public DateTime? FollowUpDate { get; set; }
    }
}
