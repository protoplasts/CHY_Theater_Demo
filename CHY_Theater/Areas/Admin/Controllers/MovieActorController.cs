using CHY_Theater.Areas.Admin.Models.ViewModels;
using CHY_Theater_DataAcess.Data;
using CHY_Theater_Models.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CHY_Theater.Areas.Admin.Controllers
{
    [Area("Admin")]

    public class MovieActorController : Controller
    {
        private readonly Theater_ProjectDbContext _context;

        public MovieActorController(Theater_ProjectDbContext context)
        {
            _context = context;

        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> GetMovieActors(int movieId)
        {
            var movieActors = await _context.MovieActors
                .Where(ma => ma.MovieId == movieId)
                .Select(ma => new MovieActorViewModel
                {
                    ActorId = ma.ActorId,
                    ActorName = ma.Actor.ActorName,
                    MainLevel = ma.MainLevel,
                    IsSelected = true
                })
                .ToListAsync();

            var allActors = await _context.Actors
                .Where(a => !movieActors.Any(ma => ma.ActorId == a.ActorId))
                .Select(a => new MovieActorViewModel
                {
                    ActorId = a.ActorId,
                    ActorName = a.ActorName,
                    MainLevel = 0,
                    IsSelected = false
                })
                .ToListAsync();

            movieActors.AddRange(allActors);

            return Json(movieActors);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateMovieActors(int movieId, List<MovieActorViewModel> actors)
        {
            var movie = await _context.Movies
                .Include(m => m.MovieActors)
                .FirstOrDefaultAsync(m => m.MovieId == movieId);

            if (movie == null)
            {
                return NotFound();
            }

            movie.MovieActors.Clear();
            foreach (var actor in actors.Where(a => a.IsSelected))
            {
                movie.MovieActors.Add(new MovieActor
                {
                    ActorId = actor.ActorId,
                    MainLevel = actor.MainLevel
                });
            }

            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}
