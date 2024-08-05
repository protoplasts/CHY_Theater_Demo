using CHY_Theater.Areas.Identity.Authorize;
using CHY_Theater.Areas.Identity.Models.ViewModels;
using CHY_Theater_Models.Models;
using CHY_Theater_Utitly;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text.Encodings.Web;

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

		public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager,
			 UrlEncoder urlEncoder, RoleManager<IdentityRole> roleManager, IEmailSender emailSender, JwtTokenService jwtTokenService)
		{
			_urlEncoder = urlEncoder;
			_signInManager = signInManager;
			_userManager = userManager;
			_roleManager = roleManager;
			_emailSender = emailSender;
			_jwtTokenService = jwtTokenService;
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
                RoleList = _roleManager.Roles.Select(x => x.Name).Select(i => new SelectListItem
                {
                    Text = i,
                    Value = i
                })
            };
            return View(registerViewModel);
        }

		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Register(RegisterViewModel model, string returnurl = null)
		{
			ViewData["ReturnUrl"] = returnurl;
			returnurl = returnurl ?? Url.Content("~/");
			if (ModelState.IsValid)
			{
				var user = new ApplicationUser
				{
					UserName = model.Email,
					Email = model.Email,
					Name = model.Name,
				};
				var result = await _userManager.CreateAsync(user, model.Password);
				if (result.Succeeded)
				{
					//將選取的角色塞入資料庫

					if (model.RoleSelected != null)
					{
						await _userManager.AddToRoleAsync(user, model.RoleSelected);
					}
					else
					{
						await _userManager.AddToRoleAsync(user, SD.User);
					}
					//Email Confirmation
					//var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
					//var callbackurl = Url.Action("ConfirmEmail", "Account", new
					//{
					//    userid = user.Id,
					//    code
					//}, protocol: HttpContext.Request.Scheme);

					//await _emailSender.SendEmailAsync(model.Email, "Confirm Email - Identity Manager",
					//                       $"Please confirm your email by clicking here: <a href='{callbackurl}'>link</a>");

					// Sign in the user immediately
					//await _signInManager.SignInAsync(user, isPersistent: false);
					//return LocalRedirect(returnurl);


					// Generate fake email confirmation token
					var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
					var callbackurl = Url.Action("FakeConfirmEmail", "Account", new
					{
						userId = user.Id,
						code,
						returnurl
					}, protocol: HttpContext.Request.Scheme);

					//// Generate JWT token
					//var token = await _jwtTokenService.GenerateJwtToken(user);

					//// Store the token in session
					//HttpContext.Session.SetString("JWTToken", token);

					// Simulate sending the email (you can log this or just display it in the view for testing)
					// For example:
					//ViewBag.Message = $"Please confirm your email by clicking here: <a href='{callbackurl}'>link</a>";
					//// Do not sign in the user immediately, wait for confirmation
					//// await _signInManager.SignInAsync(user, isPersistent: false);
					//return View("RegisterConfirmation"); // Return a view to show the confirmation link

                    // Instead of returning a view, return a JSON result
                    return Json(new
                    {
                        success = true,
                        message = $"Please confirm your email by clicking here: <a href='{callbackurl}'>link</a>"
                    });

                }

				AddErrors(result);

			}
			//失敗導回原介面
			model.RoleList = _roleManager.Roles.Select(x => x.Name).Select(i => new SelectListItem
			{
				Text = i,
				Value = i
			});
			return View(model);
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
                        // Additional logic for admin users if needed
                        return LocalRedirect(returnurl); // Or wherever you want admins to go
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
