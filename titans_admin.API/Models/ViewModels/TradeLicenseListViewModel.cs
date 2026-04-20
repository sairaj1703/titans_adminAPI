namespace titans_admin.Models.ViewModels
{
    public class TradeLicenseListViewModel
    {
        public int TradeLicenseId { get; set; }
        public string LicenseNumber { get; set; } = string.Empty;
        public string BusinessName { get; set; } = string.Empty;
        public string? BusinessType { get; set; }
        public string OwnerName { get; set; } = string.Empty;
        public DateTime IssueDate { get; set; }
        public DateTime ExpiryDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public bool IsExpiringSoon => ExpiryDate <= DateTime.UtcNow.AddMonths(3);
        public bool IsExpired => ExpiryDate < DateTime.UtcNow;
    }
}
