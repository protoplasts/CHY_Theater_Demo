using CHY_Theater.Areas.Booking.Models.ViewModels;
using CHY_Theater_DataAcess.Data;
using CHY_Theater_Models.Models;
using CHY_Theater_Utitly;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using static CHY_Theater.Areas.Booking.Models.ViewModels.TicketTpyeSnacksViewModel;

namespace CHY_Theater.Areas.Booking.Controllers
{
    [Area("Booking")]

    public class TicketTypeAndSnackController : Controller
    {

        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly Theater_ProjectDbContext _context;
        public TicketTypeAndSnackController(IWebHostEnvironment webHostEnvironment, Theater_ProjectDbContext context)
        {
            _webHostEnvironment = webHostEnvironment;
            _context = context;
        }
        [Authorize(Roles =$"{SD.Admin},{SD.User}")]

        [HttpGet]
        public IActionResult Index(int showId)
        {
            var show = _context.Shows.Where(a => a.ShowId == showId).FirstOrDefault();
            var snacks = _context.Snacks.ToList();
            var ticketTypes = _context.TicketTypes.ToList();
            //var movie = _context.Movies.Where(a => a.MovieId == movieId).FirstOrDefault();
            //直接使用showId查詢
            var movie2 = _context.Movies
                        .FirstOrDefault(m => m.MovieId ==
                            _context.Shows.Where(s => s.ShowId == showId)
                                          .Select(s => s.MovieId)
                                          .FirstOrDefault());
            var showWithAuditoriumAndTheater = _context.Shows
            .Include(s => s.Auditorium)
            .ThenInclude(a => a.Theater)
            .FirstOrDefault(s => s.ShowId == showId);

            if (showWithAuditoriumAndTheater != null)
            {
                int auditoriumId = showWithAuditoriumAndTheater.AuditoriumId;
                int? theaterId2 = showWithAuditoriumAndTheater.Auditorium?.TheaterId;

                // Use auditoriumId and theaterId as needed



                var viewModel = new TicketTpyeSnacksViewModel
                {
                    Movie = movie2,
                    Show = show,
                    //這部分是上部分選好的data
                    BookingSelection = new BookingSelectionViewModel
                    {
                        ShowId = showId,
                        MovieId = movie2.MovieId,
                        TheaterId = theaterId2,
                        Auditoriumid = auditoriumId,
                    },
                    Snacks = snacks.Select(s => new SnacksInfo
                    {
                        SnackName = s.SnackName,
                        Price = s.Price,
                        SnackId = s.SnackId,
                        SnackSize = s.SnackSize,
                       
                    }).ToList(),
                    //將資料庫中的點心票種資訊丟到前台

                    TicketTypes = ticketTypes.Select(s => new TicketTypeInfo
                    {
                        TicketTypeId = s.TicketTypeId,
                        TypeName = s.TypeName,
                        HowManySeatForType = s.HowManySeatForType,
                        TicketDescription = s.TicketDescription,
                        Price = s.Price
                    }).ToList(),


                };
                return View(viewModel);
            }
            return View();
        }
        [HttpPost]
        public IActionResult ProcessSelection(int ShowId, int MovieId, int TheaterId, int Auditoriumid, string SelectedTickets, string SelectedSnacks, int GrandTotal, int GrandSeatTotal)
        {
            if (ModelState.IsValid)
            {
                var bookingSelection = new BookingSelectionViewModel
                {
                    ShowId = ShowId,
                    MovieId = MovieId,
                    TheaterId = TheaterId,
                    Auditoriumid = Auditoriumid,
                    TotalPrice = GrandTotal,
                    TotalHowManySeat = GrandSeatTotal,

                };

                // Handle selected tickets
                if (!string.IsNullOrEmpty(SelectedTickets))
                {
                    bookingSelection.SelectedTickets = JsonConvert.DeserializeObject<List<BookingSelectionViewModel.TicketSelection>>(SelectedTickets);
                }
                else
                {
                    bookingSelection.SelectedTickets = new List<BookingSelectionViewModel.TicketSelection>();
                }

                // Handle selected snacks
                if (!string.IsNullOrEmpty(SelectedSnacks))
                {
                    bookingSelection.SelectedSnacks = JsonConvert.DeserializeObject<List<BookingSelectionViewModel.SnackSelection>>(SelectedSnacks);
                }
                else
                {
                    bookingSelection.SelectedSnacks = new List<BookingSelectionViewModel.SnackSelection>();
                }

                // Validate that at least one ticket is selected
                if (bookingSelection.SelectedTickets.Count == 0)
                {
                    ModelState.AddModelError("", "Please select at least one ticket.");
                    return RedirectToAction("Index", new { theaterId = TheaterId, movieId = MovieId, showId = ShowId });
                }

                //var selectedTicketsList = System.Text.Json.JsonSerializer.Deserialize<List<TicketSelection>>(SelectedTickets);
                //var firstTicket = selectedTicketsList[0];
                //bookingSelection.SelectedTicketTypeId = firstTicket.TicketTypeId;
                //bookingSelection.Quantity = firstTicket.Quantity;
                // Store bookingSelection in TempData
                TempData["BookingSelection"] = JsonConvert.SerializeObject(bookingSelection);
                return RedirectToAction("ChooseSeat", "ChooseSeat");
            }

            // If not valid, return to the same view with errors
            return RedirectToAction("Index", new { theaterId = TheaterId, movieId = MovieId, showId = ShowId });
        }
    }
}
