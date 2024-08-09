using CHY_Theater_DataAcess.Data;
using Microsoft.AspNetCore.Mvc;

namespace CHY_Theater.Areas.Customer.ViewComponents
{
    
        public class NewsViewComponent : ViewComponent
        {
            private readonly Theater_ProjectDbContext _context;

            public NewsViewComponent(Theater_ProjectDbContext context)
            {
                _context = context;
            }

            public IViewComponentResult Invoke(int count = 7)
            {
                var events = _context.News
                    .OrderByDescending(e => e.StartDate)
                    .Take(count)
                    .ToList();

                return View(events);
            }
        }
    
}
