using CHY_Theater.Areas.Identity.Models.ViewModels;
using CHY_Theater.Areas.Identity.Services;
using CHY_Theater_DataAcess.Data;
using CHY_Theater_Models.Models;
using FUEN104_2_FinalProject.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using static System.Runtime.CompilerServices.RuntimeHelpers;

namespace CHY_Theater.Areas.Booking.Controllers
{
    [Area("Booking")]

    public class BookingController : Controller
    {
        private readonly Theater_ProjectDbContext _context;
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly IRewardPointService _rewardPointService;

		public BookingController(UserManager<ApplicationUser> userManager, Theater_ProjectDbContext context, IRewardPointService rewardPointService)
        {
            _context = context; 
            _userManager = userManager;
			_rewardPointService = rewardPointService;

		}
		[HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> ProcessPayment([FromBody] ConfirmSelectionViewModel model)
        {
            try
            {
                // Get the current user's ID
                var userId = GetCurrentUserId();
                var booking = new CHY_Theater_Models.Models.Booking
                {
                    UserId = userId,
                    ShowingId = model.Showid,
                    BookingDate = DateTime.Now,
                    BookingStatus = "Pending", // Or whatever initial status you want
                    MerchantTradeNo = model.MerchantTradeNo

                };
                _context.Bookings.Add(booking);
                await _context.SaveChangesAsync();


                //add ShowSeats first
                foreach (var seat in model.SelectedSeats)
                {
                    var showSeat = new ShowSeat
                    {
                        ShowId = model.Showid,
                        ShowSeatStatus = "Booked",
                        SeatId = int.Parse(seat.SeatID)
                    };
                    _context.ShowSeats.Add(showSeat);
                }
                await _context.SaveChangesAsync();


                // Add BookingTicketTypes_Detail

                foreach (var ticket in model.SelectedTickets)
                {
                    var bookingTicketTypesDetail = new BookingTicketTypesDetail
                    {
                        BookingId = booking.BookingId,
                        TicketTypeId = ticket.TicketTypeId
                    };
                    _context.BookingTicketTypesDetails.Add(bookingTicketTypesDetail);
                    await _context.SaveChangesAsync();

                }

                // Add BookingSeats_Detail
                foreach (var seat in model.SelectedSeats)
                {
                    var seatIdString = seat.SeatID; // assuming seat.SeatID is a string
                    int seatId = int.Parse(seatIdString);
                    // You need to get the ShowSeatId based on the seat information

                    var ShowSeatId = _context.ShowSeats.FirstOrDefault(a => a.SeatId == seatId);

                    var bookingSeatDetail = new BookingSeatsDetail
                    {
                        BookingId = booking.BookingId,
                        ShowSeatId = ShowSeatId.ShowSeatId
                    };
                    _context.BookingSeatsDetails.Add(bookingSeatDetail);
                    await _context.SaveChangesAsync();

                }
                //Add PaymentTransaction
                var paymentTransaction = new PaymentTransaction
                {
                    BookingId = booking.BookingId,
                    MerchantTradeNo= model.MerchantTradeNo,
                    RtnMsg="付款進行中",
                    TradeAmt=model.MovieTotalPrice,
                    MemberID= userId,
                    PaymentType="現場取票付款"           
                 };
                _context.PaymentTransactions.Add(paymentTransaction);
                await _context.SaveChangesAsync();
                // Add BookingSnacks if any
                if (model.SelectedSnacks != null)
                {
                    foreach (var snack in model.SelectedSnacks)
                    {
                        var bookingSnack = new BookingSnack
                        {
                            BookingId = booking.BookingId,
                            SnackId = snack.SnackId,
                            Quantity = snack.Quantity
                        };
                        _context.BookingSnacks.Add(bookingSnack);
                        await _context.SaveChangesAsync();
                    }
                }
                // Apply points discount
                if (model.AppliedPoints > 0)
                {
                    var success = await _rewardPointService.UsePointsAsync(userId, model.AppliedPoints);

                    if (success)
                    {
                        var discount = model.AppliedPoints / 100;
                        model.MovieTotalPrice -= discount;
                    }
                    else
                    {
                        return Json(new { success = false, message = "Failed to apply points. Please try again." });
                    }
                }
                // If a coupon was applied, update its usage count
                if (!string.IsNullOrEmpty(model.CouponCode))
                {
                    var coupon = await _context.Coupons.FirstOrDefaultAsync(c => c.CouponCode == model.CouponCode);
                    if (coupon != null)
                    {
                        if (coupon.CurrentUsageCount.HasValue)
                        {
                            coupon.CurrentUsageCount++;
                        }
                        else
                        {
                            coupon.CurrentUsageCount = 1;
                        }

                        await _context.SaveChangesAsync();
                    }
                }

                return Json(new { success = true, bookingId = booking.BookingId });
            }
            catch (Exception ex)
            {
                // Log the exception
                return Json(new { success = false, message = "An error occurred while processing the booking." });
            }


        }
        [HttpPost]
        public async Task<IActionResult> UsePoints([FromBody] UsePointsViewModel model)
        {
            if (model == null)
            {
                return Json(new { success = false, message = "Invalid data received. Model is null." });
            }

            var debugMessage = $"Received PointsToUse: {model.PointsToUse}, CurrentTotal: {model.CurrentTotal}";

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var pointsToUse = model.PointsToUse;
            var currentTotal = model.CurrentTotal;

            var success = await _rewardPointService.UsePointsAsync(userId, pointsToUse);

            if (success)
            {
                var discount = pointsToUse ; // Assuming 100 points = $1 discount
                var newTotal = currentTotal - discount;

                return Json(new
                {
                    success = true,
                    newTotal = newTotal,
                    newTotalFormatted = newTotal.ToString("C"),
                    message = $"Successfully used {pointsToUse} points for a ${discount} discount.",
                    remainingPoints = await _rewardPointService.GetTotalPointsAsync(userId)
                });
            }
            else
            {
                return Json(new
                {
                    success = false,
                    message = $"Not enough points available. Debug: {debugMessage}"
                });
            }
        }
        // Method to get the current user's ID
        private string GetCurrentUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier);
        }
    }
}
