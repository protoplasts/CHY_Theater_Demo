﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using CHY_Theater_Models.Models;
using CHY_Theater_DataAcess.Data;
using CHY_Theater.Areas.Identity.Models.ViewModels;
using CHY_Theater.Models;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using ZXing;
using ZXing.QrCode;
using CHY_Theater.Areas.Identity.Services;
// Make sure to include the correct namespace for ApplicationUser

namespace CHY_Theater.Areas.Identity.Controllers
{
    [Authorize]
    [Area("Identity")]
    public class MemberCenterController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly Theater_ProjectDbContext _context;
        private readonly SignInManager<ApplicationUser> _signInManager;
		private readonly BarcodeService _barcodeService;

		public MemberCenterController(UserManager<ApplicationUser> userManager, Theater_ProjectDbContext context, SignInManager<ApplicationUser> signInManager, BarcodeService barcodeService)
        {
            _userManager = userManager;
            _context = context;
            _signInManager = signInManager;
			_barcodeService = barcodeService;

		}

		public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }
        public async Task<IActionResult> OrderInquiry()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            // Parse user.Id to int, or use a default value if parsing fails

            var bookings = await _context.Bookings
            .Where(b => b.UserId == user.Id)
            .Include(b => b.Showing)
                .ThenInclude(s => s.Movie)
            .Include(b => b.Showing)
                .ThenInclude(s => s.Auditorium)
                    .ThenInclude(a => a.Theater)
            .Include(b => b.BookingSeatsDetails)
                .ThenInclude(bsd => bsd.ShowSeat)
                    .ThenInclude(ss => ss.Seat)
            .Include(b => b.BookingTicketTypesDetails)
                .ThenInclude(bttd => bttd.TicketType)
            .Include(b => b.BookingSnacks)
                .ThenInclude(bs => bs.Snack)
            .Include(b => b.PaymentTransactions)
            .OrderByDescending(b => b.BookingDate)
            .ToListAsync();


            var viewModels = bookings.Select(b => new BookingViewModel
            {
                BookingId = b.BookingId,
                BookingDate = b.BookingDate,
                MerchantTradeNo = b.MerchantTradeNo,
                BookingStatus = b.PaymentTransactions.Select(pt => pt.RtnMsg).FirstOrDefault(),
                MovieTitle = b.Showing.Movie.MovieName,
                ShowTime = b.Showing.ShowDateTime,
                TheaterName = b.Showing.Auditorium.Theater.TheaterName,
                AuditoriumName = b.Showing.Auditorium.AuditoriumName,
                SeatDetails = b.BookingSeatsDetails
                .Select(bsd => bsd.ShowSeat?.Seat != null
                    ? $"{bsd.ShowSeat.Seat.SeatRow}{bsd.ShowSeat.Seat.SeatNumber}"
                    : "N/A")
                .ToList(),
                TicketTypes = b.BookingTicketTypesDetails.Select(bttd => $"{bttd.TicketType.TypeName}").ToList(),
                Snacks = b.BookingSnacks.Select(bs => $"{bs.Snack.SnackName} x{bs.Quantity}").ToList(),
                TotalAmount = b.PaymentTransactions.Sum(pt => pt.TradeAmt),
                PaymentTypes=b.PaymentTransactions.Select(pt =>pt.PaymentType).FirstOrDefault()
            }).ToList();

            return View(viewModels);
        }

        [HttpGet]
        public async Task<IActionResult> UpdatePersonalInfo()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdatePersonalInfo(ApplicationUser model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            user.Name = model.Name;
            user.Email = model.Email;
            user.Address = model.Address;
            user.PhoneNumber = model.PhoneNumber;
            user.Birthday = model.Birthday;

            // Check if all detailed info is filled
            if (!string.IsNullOrEmpty(user.Address) &&
                !string.IsNullOrEmpty(user.PhoneNumber) &&
                user.Birthday.HasValue)
            {
                user.MembershipLevel = "Advanced";
            }

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                TempData["StatusMessage"] = "Your profile has been updated successfully.";
                return RedirectToAction("Index", "MemberCenter");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            // Check if the new password is the same as the old one
            var isSamePassword = await _userManager.CheckPasswordAsync(user, model.NewPassword);
            if (isSamePassword)
            {
                ModelState.AddModelError(string.Empty, "新密碼不能與目前密碼相同。");
                return View(model);
            }

            var changePasswordResult = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
            if (!changePasswordResult.Succeeded)
            {
                foreach (var error in changePasswordResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(model);
            }

            await _signInManager.RefreshSignInAsync(user);
            TempData["StatusMessage"] = "您的密碼已成功更改。";

            return RedirectToAction("Index", "MemberCenter");
        }
        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }
		public async Task<IActionResult> GetUserBarcode()
		{
			var user = await _userManager.GetUserAsync(User);
			if (user == null)
			{
				return NotFound();
			}

			var barcodeImage = _barcodeService.GenerateUserIdBarcode(user.Id);
			return File(barcodeImage, "image/png");
		}
	}
}