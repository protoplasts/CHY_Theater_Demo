using CHY_Theater.Areas.Customer.Models.ViewModels;
using CHY_Theater_DataAcess.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CHY_Theater.Areas.Customer.Controllers
{
    [Area("Customer")]

    public class MoviesController : Controller
	{
		private readonly Theater_ProjectDbContext _context;

		public MoviesController(Theater_ProjectDbContext context)
		{
			_context = context;
		}
		public IActionResult Index()
		{
			var onShowMovies = _context.Movies.Where(m => m.MovieState == 1).ToList();
			var comingSoonMovies = _context.Movies.Where(m => m.MovieState == 0).ToList();

			var viewModel = new MovieViewModel
			{
				OnShowMovies = onShowMovies,
				ComingSoonMovies = comingSoonMovies
			};
			return View(viewModel);

		}
		public IActionResult Details(int id)
		{
			var movie = _context.Movies
				.Include(m => m.MovieClasses).ThenInclude(mc => mc.Class)
				.Include(m => m.MovieActors).ThenInclude(ma => ma.Actor)
				.Include(m => m.Comments).ThenInclude(c => c.User)
				.Include(m => m.Shows)
					.ThenInclude(s => s.Auditorium)
						.ThenInclude(a => a.Theater)
				.FirstOrDefault(m => m.MovieId == id);

			if (movie == null)
			{
				return NotFound();
			}

			return View(movie);
		}
	}
}
