using System.ComponentModel.DataAnnotations;

namespace CHY_Theater.Areas.Identity.Models.ViewModels
{
	public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
