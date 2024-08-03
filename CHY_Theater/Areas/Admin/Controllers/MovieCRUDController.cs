using CHY_Theater.Areas.Admin.Service;
using CHY_Theater_Models.Models;
using Microsoft.AspNetCore.Mvc;

namespace CHY_Theater.Areas.Admin.Controllers
{
    [Area("Admin")]

    public class MovieCRUDController : Controller
    {
        private readonly MovieService _movieService;

        public MovieCRUDController(MovieService movieService)
        {
            _movieService = movieService;
        }

        // GET: Movie
        public async Task<IActionResult> Index()
        {
            var movies = await _movieService.GetMoviesAsync();
            return View(movies);
        }

        // GET: Movie/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var movie = await _movieService.GetMovieAsync(id);
            if (movie == null)
            {
                return NotFound();
            }
            return View(movie);
        }

        // GET: Movie/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Movie/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Genre,ReleaseDate")] Movie movie)
        {
            if (ModelState.IsValid)
            {
                await _movieService.CreateMovieAsync(movie);
                return RedirectToAction(nameof(Index));
            }
            return View(movie);
        }

        // GET: Movie/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var movie = await _movieService.GetMovieAsync(id);
            if (movie == null)
            {
                return NotFound();
            }
            return View(movie);
        }

        // POST: Movie/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Genre,ReleaseDate")] Movie movie)
        {
            if (id != movie.MovieId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                await _movieService.UpdateMovieAsync(id, movie);
                return RedirectToAction(nameof(Index));
            }
            return View(movie);
        }

        // POST: Movie/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _movieService.DeleteMovieAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
