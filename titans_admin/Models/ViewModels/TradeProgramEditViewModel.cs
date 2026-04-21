using System.ComponentModel.DataAnnotations;

namespace titans_admin.Models.ViewModels
{
    public class TradeProgramEditViewModel
    {
        public int TradeProgramId { get; set; }

        [Required]
        [MaxLength(200)]
        public string ProgramName { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Description { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; } = DateTime.UtcNow;

        [DataType(DataType.Date)]
        public DateTime? EndDate { get; set; }

        [Required]
        [MaxLength(50)]
        public string Status { get; set; } = "Active";

        [MaxLength(100)]
        public string? ProgramType { get; set; }

        [DataType(DataType.Currency)]
        public decimal? Budget { get; set; }
    }
}
