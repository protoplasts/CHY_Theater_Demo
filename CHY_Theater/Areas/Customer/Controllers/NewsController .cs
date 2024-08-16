using CHY_Theater_DataAcess.Data;
using Microsoft.AspNetCore.Mvc;

namespace CHY_Theater.Areas.Customer.Controllers
{
    [Area("Customer")]

    public class NewsController : Controller
    {
        private readonly Theater_ProjectDbContext _context;

        public NewsController(Theater_ProjectDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var events = _context.News.ToList();
            return View(events);
        }
        public IActionResult Details(int id)
        {
            var eventItem = _context.News.FirstOrDefault(e => e.NewsId == id);
            if (eventItem == null)
            {
                return NotFound();
            }
            return View(eventItem);
        }
    }
}
