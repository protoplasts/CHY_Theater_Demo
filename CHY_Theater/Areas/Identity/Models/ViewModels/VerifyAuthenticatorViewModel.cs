using System.ComponentModel.DataAnnotations;

namespace CHY_Theater.Areas.Identity.Models.ViewModels
{
	public class VerifyAuthenticatorViewModel
    {
        [Required]
        public string Code { get; set; }
        public string? ReturnUrl { get; set; }
        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
        [Display(Name = "Remember Client?")]
        public bool RememberClient { get; set; }
    }
}
