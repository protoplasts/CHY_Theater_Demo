using CHY_Theater_DataAcess.Data;
using CHY_Theater_Models.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CHY_Theater.Areas.Admin.Controllers
{
    [Area("Admin")]

    public class MoviesController : Controller
    {
        private readonly Theater_ProjectDbContext _context;

        public MoviesController(Theater_ProjectDbContext context)
        {
            _context = context;

        }
        public async Task<IActionResult> Index()
        {
            return View(await _context.Movies.ToListAsync());
        }
        // GET: Movies/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movies.FindAsync(id);
            if (movie == null)
            {
                return NotFound();
            }
            return View(movie);
        }

        // POST: Movies/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [FromForm] Movie movie, IFormFile? MovieImage)
        {
            if (id != movie.MovieId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (MovieImage != null && MovieImage.Length > 0)
                    {
                        // Generate a unique filename
                        string uniqueFileName = Guid.NewGuid().ToString() + "_" + MovieImage.FileName;

                        // Combine the filename with the path to save in wwwroot/images
                        string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", uniqueFileName);

                        // Ensure the directory exists
                        Directory.CreateDirectory(Path.GetDirectoryName(filePath));

                        // Save the file
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await MovieImage.CopyToAsync(stream);
                        }

                        // Update the movie's image URL
                        movie.MovieImage = "/images/" + uniqueFileName;
                    }
                    else
                    {
                        // No new image uploaded, keep the existing image URL
                        var existingMovie = await _context.Movies.AsNoTracking().FirstOrDefaultAsync(m => m.MovieId == id);
                        if (existingMovie != null)
                        {
                            movie.MovieImage = existingMovie.MovieImage;
                        }
                    }

                    _context.Update(movie);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MovieExists(movie.MovieId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(movie);
        }
        private bool MovieExists(int id)
        {
            return _context.Movies.Any(e => e.MovieId == id);
        }
        public IActionResult GetPicture(int id)
        {
            var movie = _context.Movies.FirstOrDefault(m => m.MovieId == id);
            if (movie == null || string.IsNullOrEmpty(movie.MovieImage))
            {
                return NotFound();
            }

            var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", movie.MovieImage.TrimStart('/'));
            return PhysicalFile(imagePath, "image/jpeg");
        }
    }
}
