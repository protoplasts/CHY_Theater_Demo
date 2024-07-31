using CHY_Theater_DataAcess.Data;
using CHY_Theater_Utitly;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CHY_Theater.Areas.Customer.Controllers
{
    [Area("Customer")]

    public class TheatersController : Controller
    {
        private readonly Theater_ProjectDbContext _context;

        public TheatersController(Theater_ProjectDbContext context)
        {
            _context = context;
        }
        [Authorize(Roles = $"{SD.Admin},{SD.User}")]
        public async Task<IActionResult> Index()
        {
            var theaters = await _context.Theaters.ToListAsync();
            return View(theaters);
        }
        public IActionResult Details(int id)
        {
            var theater = _context.Theaters
                .Include(t => t.Auditoria)
                    .ThenInclude(a => a.Shows)
                        .ThenInclude(s => s.Movie)
                .FirstOrDefault(t => t.TheaterId == id);

            if (theater == null)
            {
                return NotFound();
            }

            return View(theater);
        }
    }
}
