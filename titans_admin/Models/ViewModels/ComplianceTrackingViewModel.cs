using titans_admin.Models.Enums;

namespace titans_admin.Models.ViewModels
{
    public class ComplianceTrackingViewModel
    {
        public int TotalRecords { get; set; }
        public int CompliantRecords { get; set; }
        public int NonCompliantRecords { get; set; }
        public int UnderReviewRecords { get; set; }
        public decimal ComplianceRate { get; set; }
        public List<ComplianceRecordItemViewModel> RecentRecords { get; set; } = new List<ComplianceRecordItemViewModel>();
    }

    public class ComplianceRecordItemViewModel
    {
        public int ComplianceRecordId { get; set; }
        public string ProgramName { get; set; } = string.Empty;
        public ComplianceResult Result { get; set; }
        public DateTime ReviewDate { get; set; }
        public string? ReviewedBy { get; set; }
        public string? Findings { get; set; }
    }
}
