using CHY_Theater_DataAcess.Data;
using Microsoft.AspNetCore.Mvc;

namespace CHY_Theater.Areas.Customer.ViewComponents
{
    public class EventsViewComponent : ViewComponent
    {
        private readonly Theater_ProjectDbContext _context;

        public EventsViewComponent(Theater_ProjectDbContext context)
        {
            _context = context;
        }

        public IViewComponentResult Invoke(int count = 4)
        {
            var events = _context.Events
                .OrderByDescending(e => e.StartDate)
                .Take(count)
                .ToList();

            return View(events);
        }
    }
}
