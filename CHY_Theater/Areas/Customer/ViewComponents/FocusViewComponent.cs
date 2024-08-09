using CHY_Theater.Areas.Customer.Models.ViewModels;
using CHY_Theater_DataAcess.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CHY_Theater.Areas.Customer.ViewComponents
{
    public class FocusViewComponent : ViewComponent
    {
        private readonly Theater_ProjectDbContext _context;

        public FocusViewComponent(Theater_ProjectDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var onFocusMovies = await _context.Movies.Where(m => m.OnFous == 1).ToListAsync();

            var viewModel = new MovieViewModel
            {
                OnFocusMovies = onFocusMovies
            };

            return View(viewModel);
        }
    }
}
