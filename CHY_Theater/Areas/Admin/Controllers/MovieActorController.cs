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
        [HttpGet]
        public async Task<IActionResult> SearchActors(string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                return Json(new { actors = new List<Actor>(), exactMatch = false });
            }

            var actors = await _context.Actors
                .Where(a => a.ActorName.Contains(query))
                .Select(a => new { a.ActorId, a.ActorName })
                .Take(10)
                .ToListAsync();

            var exactMatch = actors.Any(a => a.ActorName.Equals(query, StringComparison.OrdinalIgnoreCase));

            return Json(new { actors, exactMatch });
        }
        [HttpPost]
        public async Task<IActionResult> AddNewActor(string actorName)
        {
            // Check if the actor already exists (case-insensitive)
            var existingActor = await _context.Actors
                .FirstOrDefaultAsync(a => a.ActorName.ToLower() == actorName.ToLower());

            if (existingActor != null)
            {
                // Actor already exists, return the existing actor's info
                return Json(new
                {
                    actorId = existingActor.ActorId,
                    actorName = existingActor.ActorName,
                    message = "Actor already exists"
                });
            }

            // Actor doesn't exist, create a new one
            var newActor = new Actor { ActorName = actorName };
            _context.Actors.Add(newActor);
            await _context.SaveChangesAsync();

            return Json(new
            {
                actorId = newActor.ActorId,
                actorName = newActor.ActorName,
                message = "New actor added successfully"
            });
        }
        [HttpPost]
        public async Task<IActionResult> AddActorToMovie(int movieId, int actorId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var movieActor = await _context.MovieActors
                    .FirstOrDefaultAsync(ma => ma.MovieId == movieId && ma.ActorId == actorId);

                if (movieActor != null)
                {
                    return BadRequest("Actor already added to this movie.");
                }

                var newMovieActor = new MovieActor
                {
                    MovieId = movieId,
                    ActorId = actorId,
                    MainLevel = 1 // Default value, can be changed later
                };

                _context.MovieActors.Add(newMovieActor);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                var actor = await _context.Actors.FindAsync(actorId);
                return Json(new { actorId = actor.ActorId, actorName = actor.ActorName });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                // Log the exception
                Console.WriteLine($"Error in AddActorToMovie: {ex.Message}");
                return StatusCode(500, "An error occurred while adding the actor to the movie.");
            }
        }
    }
}
