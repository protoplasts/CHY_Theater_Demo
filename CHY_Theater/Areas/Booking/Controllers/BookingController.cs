using CHY_Theater_DataAcess.Data;
using CHY_Theater_Models.Models;
using FUEN104_2_FinalProject.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CHY_Theater.Areas.Booking.Controllers
{
    [Area("Booking")]

    public class BookingController : Controller
    {
        private readonly Theater_ProjectDbContext _context;

        public BookingController(Theater_ProjectDbContext context)
        {
            _context = context;

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
                //
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

        // Method to get the current user's ID
        private string GetCurrentUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier);
        }
    }
}
