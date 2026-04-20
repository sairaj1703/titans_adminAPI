namespace titans_admin.Models.ViewModels
{
    public class TradeProgramListViewModel
    {
        public int TradeProgramId { get; set; }
        public string ProgramName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? ProgramType { get; set; }
        public decimal? Budget { get; set; }
        public int ComplianceRecordCount { get; set; }
        public bool IsActive => Status == "Active";
    }
}
