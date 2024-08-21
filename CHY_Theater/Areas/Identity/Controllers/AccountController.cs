using CHY_Theater.Areas.Identity.Authorize;
using CHY_Theater.Areas.Identity.Models.ViewModels;
using CHY_Theater.Service;
using CHY_Theater.Service.IService;
using CHY_Theater_Models.Models;
using CHY_Theater_Utitly;
using MailKit.Security;
using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using MimeKit.Text;
using System.Net;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Web;

namespace CHY_Theater.Areas.Identity.Controllers
{
	[Area("Identity")]
	public class AccountController : Controller
	{
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly RoleManager<IdentityRole> _roleManager;
		private readonly SignInManager<ApplicationUser> _signInManager;
		private readonly IEmailSender _emailSender;
		private readonly UrlEncoder _urlEncoder;
		private readonly JwtTokenService _jwtTokenService;
        private readonly ILogger<AccountController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IUserCouponService _userCouponService;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager,
			 UrlEncoder urlEncoder, RoleManager<IdentityRole> roleManager, IEmailSender emailSender, JwtTokenService jwtTokenService,
        ILogger<AccountController> logger, IConfiguration configuration, IUserCouponService userCouponService)
		{
			_urlEncoder = urlEncoder;
			_signInManager = signInManager;
			_userManager = userManager;
			_roleManager = roleManager;
			_emailSender = emailSender;
			_jwtTokenService = jwtTokenService; _logger = logger; _configuration = configuration; _userCouponService = userCouponService;


        }
        [AllowAnonymous]
        public IActionResult Index()
        {
            return View();
        }
        [AllowAnonymous]
		public async Task<IActionResult> Register(string returnurl = null)
		{
            //checks if the "Admin" role exists. If not, it creates both "Admin" and "User" roles.
            if (!_roleManager.RoleExistsAsync(SD.Admin).GetAwaiter().GetResult())
            {
                await _roleManager.CreateAsync(new IdentityRole(SD.Admin));
                await _roleManager.CreateAsync(new IdentityRole(SD.User));
            }
            ViewData["ReturnUrl"] = returnurl;
            RegisterViewModel registerViewModel = new()
            {
                //對每個角色使用 Select(x => x.Name) 取得其名稱。接著，這些名稱被轉換為 SelectListItem 物件，這些物件將用於在下拉選單中顯示角色名稱。
                RoleList = _roleManager.Roles.Select(x => x.Name).Select(i => new SelectListItem
                {
                    Text = i,
                    Value = i
                })
            };
            return View(registerViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model, string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    Name = model.Name,
                    DateCreated = DateTime.UtcNow,
                    MembershipLevel = "普通",
                    MemberPoints = 0,
                    TotalSpent = 0,
                    LastLoginTime = DateTime.UtcNow
                };

                try
                {
                    var result = await _userManager.CreateAsync(user, model.Password);
                    if (result.Succeeded)
                    {
                        _logger.LogInformation("User created a new account with password.");
                        
                        if (model.RoleSelected == "管理員")
                        {
                            await _userManager.AddToRoleAsync(user, model.RoleSelected);
                        }
                        else
                        {
                            await _userManager.AddToRoleAsync(user, SD.User);                        
                            // Create new user coupon
                            await _userCouponService.CreateNewUserCoupon(user.Id);
                            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                            code = WebUtility.UrlEncode(code);
                            var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: HttpContext.Request.Scheme);

                            await SendEmailAsync(model.Email, "帳號驗證信-如影隨形",
                                $"請點擊此處確認您的電子郵件：<a href='{callbackUrl}'>link</a>");
                        }
                        //用於確定在用戶登錄之前是否需要確認帳戶。我在Program.cs設為false所以不需要驗證即可登入)
                        if (_userManager.Options.SignIn.RequireConfirmedAccount)
                        {
                            return Json(new { success = true, message = "註冊成功。請檢查您的電子郵件以進行驗證。" });
                        }
                        else
                        {
                            await _signInManager.SignInAsync(user, isPersistent: false);
                            return Json(new { success = true, message = "註冊成功。系統將跳轉回首頁並自動登入，同時請檢查您的電子信箱進行驗證。" });
                            //轉回首頁
                        }
                    }
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error during user registration");
                    return Json(new { success = false, message = "註冊發生錯誤。請稍後重試。" });
                }
            }
            
            return Json(new { success = false, message = "註冊失敗。請檢查您的輸入欄位並重試。" });
        }

        private async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {    
            // 從配置中讀取電子郵件設置
            var emailSettings = _configuration.GetSection("EmailSettings");
            var smtpServer = emailSettings["SmtpServer"];
            var smtpPort = int.Parse(emailSettings["SmtpPort"]);
            var senderEmail = emailSettings["SenderEmail"];
            var password = emailSettings["Password"];
            var senderName = emailSettings["SenderName"];
            // 建立 MimeMessage 對象
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress(senderName, senderEmail));
            emailMessage.To.Add(new MailboxAddress("", email));
            emailMessage.Subject = subject;
            emailMessage.Body = new TextPart(TextFormat.Html)
            {
                Text = htmlMessage
            };

            // 使用 MailKit 的 SmtpClient 發送郵件
            using (var client = new SmtpClient())
            {
                try
                {
                    await client.ConnectAsync(smtpServer, smtpPort, MailKit.Security.SecureSocketOptions.StartTls);
                    await client.AuthenticateAsync(senderEmail, password);
                    await client.SendAsync(emailMessage);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to send email");
                    throw;
                }
                finally
                {
                    await client.DisconnectAsync(true);
                }
            }
            //var client = new SmtpClient(smtpServer, smtpPort)
            //{
            //    Credentials = new NetworkCredential(senderEmail, password),
            //    EnableSsl = true
            //};

            //var mailMessage = new MailMessage
            //{
            //    From = new MailAddress(senderEmail, senderName),
            //    Subject = subject,
            //    Body = htmlMessage,
            //    IsBodyHtml = true
            //};
            //mailMessage.To.Add(email);

            //try
            //{
            //    await client.SendMailAsync(mailMessage);
            //}
            //catch (Exception ex)
            //{
            //    _logger.LogError(ex, "Failed to send email");
            //    throw;
            //}
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return RedirectToAction(nameof(ForgotPasswordConfirmation));
                }

                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                //code = WebUtility.UrlEncode(code);
                var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: HttpContext.Request.Scheme);

                try
                {
                    await SendEmailAsync(
                        model.Email,
                        "Reset Password - Identity Manager",
                        $"Please reset your password by clicking here: <a href='{callbackUrl}'>link</a>"
                    );
                    return RedirectToAction(nameof(ForgotPasswordConfirmation));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to send password reset email");
                    ModelState.AddModelError("", "Error sending email. Please try again later.");
                    return View(model);
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string code = null)
        {
            return code == null ? View("Error") : View();
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    return RedirectToAction(nameof(ResetPasswordConfirmation));
                }

                var result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);
                if (result.Succeeded)
                {
                    return RedirectToAction(nameof(ResetPasswordConfirmation));
                }
                AddErrors(result);
            }

            return View();
        }
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }
        [AllowAnonymous]
        public IActionResult FakeConfirmEmail(string userId, string code)
        {
            // Simulate the email confirmation process
            ViewData["UserId"] = userId;
            ViewData["Code"] = code;
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> FakeConfirmEmailConfirmed(string userId, string code, string returnurl = null)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return View("Error");
            }

            var result = await _userManager.ConfirmEmailAsync(user, code);
            if (result.Succeeded)
            {
                // Sign in the user after successful confirmation
                await _signInManager.SignInAsync(user, isPersistent: false);
                return LocalRedirect(returnurl ?? Url.Content("~/"));
            }

            return View("Error");
        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return View("Error");
            }

            code = WebUtility.UrlDecode(code);
            var result = await _userManager.ConfirmEmailAsync(user, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogOff()
        {
            Response.Cookies.Delete("AdminJwtToken");
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home", new { area = "Customer" });
        }
        [AllowAnonymous]
        public IActionResult Login(string returnurl = null)
        {
            //used to redirect users back to the page they were trying to access before being redirected to the login page. 
            ViewData["ReturnUrl"] = returnurl;
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnurl = null)
        {
            //如果是被別的頁面導轉過來登入成功之後會返回當初的頁面
            ViewData["ReturnUrl"] = returnurl;
            //If returnurl is null, it defaults to the home page URL(~/).
            returnurl = returnurl ?? Url.Content("~/");
            if (ModelState.IsValid)
            {
                //第一/二個:The user name and password to sign in. 第三個:whether the sign-in cookie should persist after the browser is closed.
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe,
                    lockoutOnFailure: true);
                if (result.Succeeded)
                {
                    var user = await _userManager.FindByEmailAsync(model.Email);

                    // Update the LastLoginTime property
                    // 取得伺服器的本地時間
                    DateTime localTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.Local);

                    // 更新 LastLoginTime 屬性
                    user.LastLoginTime = localTime;
                    await _userManager.UpdateAsync(user);


                    // Generate JWT token for all authenticated users
                    var jwtToken = await _jwtTokenService.GenerateJwtToken(user);

                    // Store JWT in HttpOnly cookie
                    Response.Cookies.Append("AdminJwtToken", jwtToken, new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true, // Use only if your site uses HTTPS
                        SameSite = SameSiteMode.Strict
                    });

                    if (await _userManager.IsInRoleAsync(user, "管理員"))
                    {                        
                        return LocalRedirect("~/Admin"); //管理員之後要挑到Admin page
                    }
                    else if (await _userManager.IsInRoleAsync(user, "使用者"))
                    {
                        // Additional logic for regular users if needed
                        return LocalRedirect(returnurl);
                    }
                    //The User property is a member of the Controller class, and it provides access to the current authenticated user.
                    //When a user logs in successfully, the ClaimsPrincipal (User) is set up by the authentication middleware. 
                    //var user = await _userManager.GetUserAsync(User);
                    //// Holds a list of all claims for the user. A claim is a key-value pair that represents information about the user (e.g., "FirstName" -> "John").
                    //var claim = await _userManager.GetClaimsAsync(user);

                    //if (claim.Count > 0)
                    //{
                    //    await _userManager.RemoveClaimAsync(user, claim.FirstOrDefault(u => u.Type == "FirstName"));
                    //}
                    //await _userManager.AddClaimAsync(user, new System.Security.Claims.Claim("FirstName",user.Name));

                    return LocalRedirect(returnurl);
                }
                if (result.RequiresTwoFactor)
                {
                    return RedirectToAction(nameof(VerifyAuthenticatorCode), new { returnurl, model.RememberMe });
                }
                if (result.IsLockedOut)
                {
                    return View("Lockout");
                }

                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return View(model);
                }
            }
            return View(model);
        }

		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		//This method is part of the process that allows users to log in using external authentication providers (e.g., Google, Facebook).
		//When the user clicks on an external login button, this method is called, which redirects them to the chosen provider's login page.
		//After successful authentication, the provider redirects the user back to the specified callback URL (ExternalLoginCallback).
		public IActionResult ExternalLogin(string provider, string returnurl = null)
		{
			var redirectUrl = Url.Action("ExternalLoginCallback", "Account", new { returnurl });
			var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
			return Challenge(properties, provider);
		}


		[HttpGet]
		[AllowAnonymous]
		public async Task<IActionResult> ExternalLoginCallback(string returnurl = null, string remoteError = null)
		{
			returnurl = returnurl ?? Url.Content("~/");
			if (remoteError != null)
			{
				ModelState.AddModelError(string.Empty, $"Error from external provider: {remoteError}");
				return View(nameof(Login));
			}

			var info = await _signInManager.GetExternalLoginInfoAsync();
			if (info == null)
			{
				return RedirectToAction(nameof(Login));
			}

			//Attempts to sign in the user with the external login provider using the provider key. 
			var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey,
							   isPersistent: false, bypassTwoFactor: true);
			//之前有使用此外部帳號註冊過result=Succeeded
			if (result.Succeeded)
			{
				//Updates the external authentication tokens.
				// It ensures that the application has the latest tokens provided by the external authentication service.
				await _signInManager.UpdateExternalAuthenticationTokensAsync(info);
				return LocalRedirect(returnurl);
			}
			//如果此帳號有設定二階段登入
			if (result.RequiresTwoFactor)
			{
				//跳轉至VerifyAuthenticatorCode 介面
				return RedirectToAction(nameof(VerifyAuthenticatorCode), new { returnurl });
			}
			//沒有註冊過就跳轉到註冊頁面，在這邊可以填入客製化的會員資料
			else
			{
				//that means user account is not create and we will display a view to create an account

				ViewData["ReturnUrl"] = returnurl;
				ViewData["ProviderDisplayName"] = info.ProviderDisplayName;

				//Returns the ExternalLoginConfirmation view with a model containing the user's email and name retrieved from the external login information.
				return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel
				{
					Email = info.Principal.FindFirstValue(ClaimTypes.Email),
					Name = info.Principal.FindFirstValue(ClaimTypes.Name)
				});
			}

		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[AllowAnonymous]
		public async Task<IActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model,string returnurl = null)
		{
			returnurl = returnurl ?? Url.Content("~/");

			if (ModelState.IsValid)
			{
				var info = await _signInManager.GetExternalLoginInfoAsync();
				if (info == null)
				{
					return View("Error");
				}

				//Creates a new ApplicationUser object with the email, username, and other properties populated from the ExternalLoginConfirmationViewModel.
				var user = new ApplicationUser
				{
                    UserName = model.Email,
                    Email = model.Email,
                    Name = model.Name,
                    NormalizedEmail = model.Email.ToUpper(),
                    DateCreated = DateTime.Now,
                    Address = model.Address,
                    PhoneNumber = model.PhoneNumber,
                    Birthday = model.Birthday,
                };
                // Determine MembershipLevel based on whether all optional fields are filled
                if (!string.IsNullOrEmpty(model.Address) &&
                    !string.IsNullOrEmpty(model.PhoneNumber) &&
                    model.Birthday.HasValue)
                {
                    user.MembershipLevel = "白金會員";
                }
                else
                {
                    user.MembershipLevel = "basic";
                }
                //create the new user in the database.
                var result = await _userManager.CreateAsync(user);
				if (result.Succeeded)
				{
					//If user creation is successful, assigns the user a default role (e.g., SD.User).
					await _userManager.AddToRoleAsync(user, SD.User);
					//把外部登入的資訊加到剛剛的user裡面
					result = await _userManager.AddLoginAsync(user, info);
					if (result.Succeeded)
					{
						//signs in the user and updates the external authentication tokens.
						await _signInManager.SignInAsync(user, isPersistent: false);
						await _signInManager.UpdateExternalAuthenticationTokensAsync(info);
						return LocalRedirect(returnurl);
					}
				}
				AddErrors(result);
			}
			ViewData["ReturnUrl"] = returnurl;
			return View(model);
		}


		private void AddErrors(IdentityResult result)
		{
			foreach (var error in result.Errors)
			{
				ModelState.AddModelError(string.Empty, error.Description);
			}
		}
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> VerifyAuthenticatorCode(bool rememberMe, string returnUrl = null)
        {
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                return View("Error");
            }
            ViewData["ReturnUrl"] = returnUrl;
            return View(new VerifyAuthenticatorViewModel { ReturnUrl = returnUrl, RememberMe = rememberMe });

        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VerifyAuthenticatorCode(VerifyAuthenticatorViewModel model)
        {
            model.ReturnUrl = model.ReturnUrl ?? Url.Content("~/");
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            //rememberClient is used to indicate whether the client's two-factor authentication (2FA) session should be remembered on the device being used.
            //類似在使用apple ID時，如果在第一次登入的裝置都會跳出驗證，並會詢問使否要記憶此裝置
            var result = await _signInManager.TwoFactorAuthenticatorSignInAsync(model.Code, model.RememberMe,
               model.RememberClient);
            if (result.Succeeded)
            {
                return LocalRedirect(model.ReturnUrl);
            }

            if (result.IsLockedOut)
            {
                return View("Lockout");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return View(model);
            }
        }
    }
}
