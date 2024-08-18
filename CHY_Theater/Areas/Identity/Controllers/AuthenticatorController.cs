using CHY_Theater.Areas.Identity.Models.ViewModels;
using CHY_Theater_Models.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Text.Encodings.Web;

namespace CHY_Theater.Areas.Identity.Controllers
{
    [Authorize]
    [Area("Identity")]
    public class AuthenticatorController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly UrlEncoder _urlEncoder;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AuthenticatorController(UserManager<ApplicationUser> userManager, UrlEncoder urlEncoder, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _urlEncoder = urlEncoder;
            _signInManager = signInManager;


        }
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> TwoFactorLogin()
        {  //The User property is a member of the Controller class, and it provides access to the current authenticated user.
            //When a user logs in successfully, the ClaimsPrincipal (User) is set up by the authentication middleware. 
            var user = await _userManager.GetUserAsync(User);
            // Check if the current client is remembered for 2FA
            var isTwoFactorClientRemembered = await _signInManager.IsTwoFactorClientRememberedAsync(user);

            if (user == null)
            {
                ViewData["TwoFactorEnabled"] = false;
            }
            else
            {
                ViewData["TwoFactorEnabled"] = user.TwoFactorEnabled;
                ViewData["IsTwoFactorClientRemembered"] = isTwoFactorClientRemembered;
            }
            return View();
        }
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> EnableAuthenticator()
        {
            //A format string for generating the URL that the authenticator app will use (簡單點就是隨機碼).
            string AuthenticatorUriFormat = "otpauth://totp/{0}:{1}?secret={2}&issuer={0}&digits=6";
            var user = await _userManager.GetUserAsync(User);
            await _userManager.ResetAuthenticatorKeyAsync(user);
            var token = await _userManager.GetAuthenticatorKeyAsync(user);
            //Creates the URL for the QR code by encoding the issuer and the user's email and inserting them along with the token into the format string.
            string AuthUri = string.Format(AuthenticatorUriFormat, _urlEncoder.Encode("IdentityManager"),
                _urlEncoder.Encode(user.Email), token);

            var model = new TwoFactorAuthenticationViewModel() { Token = token, QRCodeUrl = AuthUri };
            return PartialView("~/Areas/Identity/Views/PartialView/_EnableAuthenticator.cshtml", model);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EnableAuthenticator(TwoFactorAuthenticationViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                var succeeded = await _userManager.VerifyTwoFactorTokenAsync(user, _userManager.Options.Tokens.AuthenticatorTokenProvider, model.Code);

                if (succeeded)
                {
                    await _userManager.SetTwoFactorEnabledAsync(user, true);
                    return Json(new { success = true });
                }
                else
                {
                    ModelState.AddModelError("Verify", "Your two factor auth code could not be validated.");
                }
            }

            return PartialView("_EnableAuthenticator", model);
        }
        [HttpGet]
        [AllowAnonymous]
        public IActionResult AuthenticatorConfirmation()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> RemoveAuthenticator()
        {
            //Retrieves the currently logged-in user
            var user = await _userManager.GetUserAsync(User);
            //The authenticator key is used to generate 2FA codes, so resetting it effectively invalidates any previously configured authenticator app.
            await _userManager.ResetAuthenticatorKeyAsync(user);
            await _userManager.SetTwoFactorEnabledAsync(user, false);
            //Why Both Steps Are Needed
            //If only the key is reset, the user might still be prompted to set up a new authenticator during the next login attempt, which might be confusing.
            //Explicitly disabling 2FA updates the user's security settings in the system, ensuring that no 2FA code is requested at all.
            return RedirectToAction(nameof(Index), "MemberCenter");
        }

        //移除信任此裝置
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveRememberClient()
        {
            await _signInManager.ForgetTwoFactorClientAsync(); // Remove the RememberClient cookie
            /*await _signInManager.SignOutAsync();*/ // Sign out the user

            return RedirectToAction(nameof(Index), "MemberCenter");
        }
    }
}
