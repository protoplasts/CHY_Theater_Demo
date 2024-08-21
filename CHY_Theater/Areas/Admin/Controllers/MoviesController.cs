using CHY_Theater_DataAcess.Data;
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
    }
}
