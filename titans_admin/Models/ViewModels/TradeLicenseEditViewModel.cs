using System.ComponentModel.DataAnnotations;

namespace titans_admin.Models.ViewModels
{
    public class TradeLicenseEditViewModel
    {
        public int TradeLicenseId { get; set; }

        [Required]
        [Display(Name = "License Owner")]
        public int UserId { get; set; }

        // License number is auto-generated - not required for input
        [MaxLength(50)]
        [Display(Name = "License Number")]
        public string LicenseNumber { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        [Display(Name = "Business Name")]
        public string BusinessName { get; set; } = string.Empty;

        [MaxLength(100)]
        [Display(Name = "Business Type")]
        public string? BusinessType { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Issue Date")]
        public DateTime IssueDate { get; set; } = DateTime.Today;

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Expiry Date")]
        public DateTime ExpiryDate { get; set; } = DateTime.Today.AddYears(1);

        [Required]
        [MaxLength(50)]
        public string Status { get; set; } = "Pending";

        [MaxLength(500)]
        public string? Notes { get; set; }
    }
}
