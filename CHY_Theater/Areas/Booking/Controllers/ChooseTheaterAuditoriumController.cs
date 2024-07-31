using CHY_Theater.Areas.Booking.Models.ViewModels;
using CHY_Theater_DataAcess.Data;
using CHY_Theater_Models.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static CHY_Theater.Areas.Booking.Models.ViewModels.TheaterAuditoriumsViewModel;

namespace CHY_Theater.Areas.Booking.Controllers
{
    [Area("Booking")]

    public class ChooseTheaterAuditoriumController : Controller
    {
        private readonly Theater_ProjectDbContext _context;

        public ChooseTheaterAuditoriumController(Theater_ProjectDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var theater = await _context.Theaters.ToListAsync();
            var viewModel = new TheaterAuditoriumsViewModel { Theaters = theater };
            return View(viewModel);
        }

        public IActionResult ChooseAuditorium(int theaterID)
        {
            var theater = _context.Theaters.FirstOrDefault(a => a.TheaterId == theaterID);
            var auditoriums = _context.Auditoriums
                .Where(a => a.TheaterId == theaterID)
                .Include(a => a.Shows)
                .ThenInclude(s => s.Movie);

            var viewModel = new TheaterAuditoriumsViewModel
            {
                Auditoriums = auditoriums.Select(a => new AuditoriumInfo
                {
                    AuditoriumId = a.AuditoriumId,
                    AuditoriumName = a.AuditoriumName,
                    AuditoriumType = a.AuditoriumType ?? "Not specified",
                    Shows = a.Shows.Select(s => new ShowInfo
                    {
                        ShowId = s.ShowId,
                        MovieId = s.MovieId,
                        Level = s.Movie.Level,
                        MovieName = s.Movie.MovieName ?? "Unknown Movie",
                        ShowDateTime = s.ShowDateTime
                    }).ToList()
                }).ToList(),
                Theaters = new List<Theater> { theater },
                TheaterId = theaterID  // Add this line to include TheaterId in the viewModel
            };
            return PartialView("~/Areas/Booking/Views/PartialView/_AuditoriumPartial.cshtml", viewModel);
        }
    }
}
