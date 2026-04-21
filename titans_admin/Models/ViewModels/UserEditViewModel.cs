using System.ComponentModel.DataAnnotations;
using titans_admin.Models.Enums;

namespace titans_admin.Models.ViewModels
{
    public class UserEditViewModel
    {
        public int UserId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        public string LastName { get; set; } = string.Empty;

        [Required]
        public UserRole Role { get; set; }

        [Required]
        public UserStatus Status { get; set; }

        [Phone]
        public string? PhoneNumber { get; set; }

        public string? Address { get; set; }
    }
}
