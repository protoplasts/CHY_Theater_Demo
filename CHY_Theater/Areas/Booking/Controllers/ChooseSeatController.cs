using CHY_Theater.Areas.Booking.Models.ViewModels;
using CHY_Theater.Areas.Identity.Services;
using CHY_Theater.Service.IService;
using CHY_Theater_DataAcess.Data;
using CHY_Theater_Models.Models;
using FUEN104_2_FinalProject.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Security.Claims;
using static FUEN104_2_FinalProject.Models.ViewModels.ConfirmSelectionViewModel;
using static FUEN104_2_FinalProject.Models.ViewModels.SeatViewModel;

namespace CHY_Theater.Areas.Booking.Controllers
{
    [Area("Booking")]

    public class ChooseSeatController : Controller
    {
        private readonly Theater_ProjectDbContext _context;
        private readonly IRewardPointService _rewardPointService;
        private readonly IUserCouponService _userCouponService;
        public ChooseSeatController(Theater_ProjectDbContext context, IRewardPointService rewardPointService, IUserCouponService userCouponService)
        {
            _context = context; 
            _rewardPointService = rewardPointService;
            _userCouponService = userCouponService;
        }
        public IActionResult ChooseSeat()
        {
            var bookingSelection = JsonConvert.DeserializeObject<BookingSelectionViewModel>(TempData["BookingSelection"] as string);
            var auditoriumid = bookingSelection.Auditoriumid;
            var showid = bookingSelection.ShowId;
            // Retrieve selected snacks
            var selectedSnacks = bookingSelection.SelectedSnacks;
            var selectedTickets = bookingSelection.SelectedTickets;

            // Now you can access the SnackId for each selected snack
            foreach (var snack in selectedSnacks)
            {
                int snackId = snack.SnackId;
                string snackName = snack.SnackName;
                int price = snack.Price;
                int quantity = snack.Quantity;
            }
            foreach (var ticket in selectedTickets)
            {
                int ticketId = ticket.TicketTypeId;
                string ticketName = ticket.TypeName;
                int ticketprice = ticket.Price;
                int ticketquantity = ticket.Quantity;
                string ticketDescription = ticket.TicketDescription;
            }
            int totalSeats = bookingSelection.TotalHowManySeat;

            var showDeatal = _context.Shows.Where(a => a.ShowId == showid).FirstOrDefault();
            var movieInfor = _context.Movies.Where(a => a.MovieId == bookingSelection.MovieId).FirstOrDefault();
            var auditoriumDetail = _context.Auditoriums.Where(a => a.AuditoriumId == auditoriumid).FirstOrDefault();
            // Fetch all seats for the show
            var availableSeats = _context.Seats.Where(a => a.AuditoriumId == auditoriumid).ToList();
            // Fetch booked seats for the show
            var bookSeats = _context.ShowSeats.Where(a => a.ShowId == showid).ToList();

            var viewModel = new SeatViewModel
            {
                TotalSeats = totalSeats,
                BookingSelection = bookingSelection,
                ShowDateTime = showDeatal.ShowDateTime,
                MovieName = movieInfor.MovieName,
                MovieEnglishName = movieInfor.MovieEnglishName,
                Level = movieInfor.Level,
                MovieImag = movieInfor.MovieImage,
                AuditoriumName = auditoriumDetail.AuditoriumName,
                Auditoriumtype = auditoriumDetail.AuditoriumType,
                //selectedTicketName=selectedTicketInfor.TypeName,
                //selectedTicketDescription=selectedTicketInfor.TicketDescription,
                SeatList = availableSeats.Select(s => new SeatInfo
                {
                    SeatID = s.SeatId,
                    SeatRow = s.SeatRow,
                    SeatNumber = s.SeatNumber,
                    SeatType = s.SeatType,
                    ShowSeatStatus = bookSeats.FirstOrDefault(ss => ss.SeatId == s.SeatId)?.ShowSeatStatus ?? "Available",
                    SeatStatus = s.SeatStatus,

                }).ToList(),
                SelectedSnacks = selectedSnacks.Select(s => new SelectedSnack
                {
                    SnackId = s.SnackId,
                    SnackName = s.SnackName,
                    Quantity = s.Quantity,
                    Price = s.Price,
                }
                    ).ToList(),
                TicketTypes = selectedTickets.Select(st => new TicketTypeInfo
                {
                    TicketTypeId = st.TicketTypeId,
                    TypeName = st.TypeName,
                    Quantity = st.Quantity,
                    TicketDescription = st.TicketDescription,
                    Price = st.Price,
                }
                ).ToList()

            };
            //// Store the ticket types in TempData
            //TempData["TicketTypes"] = JsonConvert.SerializeObject(viewModel.TicketTypes);
            return View(viewModel);

        }

        public async Task<IActionResult> ConfirmSelection([FromForm] ConfirmSelectionViewModel model)
        {
            if (model == null)
            {
                return BadRequest("No data received");
            }
            if (model.MerchantTradeNo == null)
            {
                model.MerchantTradeNo = GenerateUniqueMerchantTradeNo();
            }
            
            if (model.SelectedSnacks == null)
            {
                model.SelectedSnacks = new List<ConfirmSelectionViewModel.SelectedSnack>();
                for (int i = 0; ; i++)
                {
                    var snackId = Request.Form[$"SelectedSnacks[{i}].SnackId"];
                    if (string.IsNullOrEmpty(snackId)) break;
                    model.SelectedSnacks.Add(new ConfirmSelectionViewModel.SelectedSnack
                    {
                        SnackId = int.Parse(snackId),
                        SnackName = Request.Form[$"SelectedSnacks[{i}].SnackName"],
                        Price = int.Parse(Request.Form[$"SelectedSnacks[{i}].Price"]),
                        Quantity = int.Parse(Request.Form[$"SelectedSnacks[{i}].Quantity"])
                    });
                }
            }
            // Get the current user's ID
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Fetch available points
            var availablePoints = await _rewardPointService.GetTotalPointsAsync(userId);
            // Pass the available points to the view using ViewBag
            ViewBag.AvailablePoints = availablePoints;

            // Fetch user-specific coupons
            var userCoupons = await _userCouponService.GetUserCoupons(userId);
            ViewBag.UserCoupons = userCoupons.Where(uc => !uc.IsUsed).ToList();
       
            return View(model);
        }
        private string GenerateUniqueMerchantTradeNo()
        {
            // Implement this method to generate a unique merchant trade number
            // This could be a combination of date, time, and a random number
            return $"MT{DateTime.Now:yyyyMMddHHmmss}{new Random().Next(1000, 9999)}";
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ValidateCoupon([FromBody] CouponValidationRequest request)
        {
            //var couponAll = _context.Coupons.ToListAsync();

            var coupon = _context.Coupons.FirstOrDefault(c => c.CouponCode == request.CouponCode);

            if (coupon == null)
            {
                return Json(new { isValid = false, message = "無效的優惠碼。" });
            }
            var currentDate = DateOnly.FromDateTime(DateTime.Now);
            if (currentDate < coupon.StartDate || currentDate > coupon.ExpiryDate)
            {
                return Json(new { isValid = false, message = "優惠碼不在有效期內。" });
            }

            if (coupon.MaxUsageCount.HasValue && coupon.CurrentUsageCount >= coupon.MaxUsageCount)
            {
                return Json(new { isValid = false, message = "優惠碼已達到使用上限。" });
            }

            // Check if the current total meets the minimum purchase amount
            if (coupon.MinimumPurchaseAmount.HasValue && request.CurrentTotal < coupon.MinimumPurchaseAmount.Value)
            {
                return Json(new { isValid = false, message = $"訂單金額須滿 {coupon.MinimumPurchaseAmount:C0} 才能使用此優惠碼。" });
            }
            // Calculate new total price based on the discount
            decimal newTotal = CalculateDiscountedTotal(coupon, request.CurrentTotal);
            string formattedTotal = newTotal.ToString("C0");

            return Json(new
            {
                isValid = true,
                newTotal = newTotal,
                newTotalFormatted = formattedTotal
            });
        }

        private decimal CalculateDiscountedTotal(Coupon coupon, decimal currentTotal)
        {
            if (coupon.DiscountType == "Percentage")
            {
                return currentTotal * (1 - (coupon.DiscountValue / 100));
            }
            else if (coupon.DiscountType == "FixedAmount")
            {
                return Math.Max(currentTotal - coupon.DiscountValue, 0);
            }

            return currentTotal;
        }

        public class CouponValidationRequest
        {
            public string CouponCode { get; set; }
            public decimal CurrentTotal { get; set; }
        }
    }
}
