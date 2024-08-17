using System.ComponentModel.DataAnnotations;

namespace CHY_Theater.Areas.Identity.Models.ViewModels
{
	public class ExternalLoginConfirmationViewModel
    {
        [Required]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }
        public string? Address { get; set; }
        public string? PhoneNumber { get; set; }
        public DateTime? Birthday { get; set; }
        public string MembershipLevel { get; set; } = "basic"; // Default to "basic"

    }
}
