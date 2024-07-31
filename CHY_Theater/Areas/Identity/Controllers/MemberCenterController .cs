using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using CHY_Theater_Models.Models;
using CHY_Theater_DataAcess.Data;
using CHY_Theater.Areas.Identity.Models.ViewModels;
using CHY_Theater.Models;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore; // Make sure to include the correct namespace for ApplicationUser

namespace CHY_Theater.Areas.Identity.Controllers
{
    [Authorize]
    [Area("Identity")]
    public class MemberCenterController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly Theater_ProjectDbContext _context;

        public MemberCenterController(UserManager<ApplicationUser> userManager, Theater_ProjectDbContext context)
        {
            _userManager = userManager;
            _context = context;
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
                TicketTypes = b.BookingTicketTypesDetails.Select(bttd => $"{bttd.TicketType.TypeName} 內函 {bttd.TicketType.TicketDescription}").ToList(),
                Snacks = b.BookingSnacks.Select(bs => $"{bs.Snack.SnackName} x{bs.Quantity}").ToList(),
                TotalAmount = b.PaymentTransactions.Sum(pt => pt.TradeAmt),
                PaymentTypes=b.PaymentTransactions.Select(pt =>pt.PaymentType).FirstOrDefault()
            }).ToList();

            return View(viewModels);
        }
    }
}