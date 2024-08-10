using System.ComponentModel.DataAnnotations;

namespace CHY_Theater.Areas.Identity.Models.ViewModels
{
    public class ChangePasswordViewModel
    {
		[Required(ErrorMessage = "請輸入密碼")]
		[DataType(DataType.Password)]
        [Display(Name = "密碼")]
        public string OldPassword { get; set; }
        
        [Required(ErrorMessage = "請輸入新密碼")]

		[StringLength(100, ErrorMessage = "{0} 必須至少包含 {2} 個字符。", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "新密碼")]
        public string NewPassword { get; set; }
		[Required(ErrorMessage = "請確認新密碼")]
		[DataType(DataType.Password)]
        [Display(Name = "確認新密碼")]
        [Compare("NewPassword", ErrorMessage = "新密碼和確認密碼不匹配。")]
        public string ConfirmPassword { get; set; }
    }
}
