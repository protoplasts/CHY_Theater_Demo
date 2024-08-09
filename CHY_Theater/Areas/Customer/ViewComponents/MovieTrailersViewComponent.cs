using CHY_Theater.Areas.Customer.Models.ViewModels;
using CHY_Theater_DataAcess.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CHY_Theater.Areas.Customer.ViewComponents
{
    public class MovieTrailersViewComponent : ViewComponent
    {
        private readonly Theater_ProjectDbContext _context;

        public MovieTrailersViewComponent(Theater_ProjectDbContext context)
        {
            _context = context;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var onShowMovies = await _context.Movies.Where(m => m.MovieState == 1).ToListAsync();
            var comingSoonMovies = await _context.Movies.Where(m => m.MovieState == 2).ToListAsync();

            var viewModel = new MovieViewModel
            {
                OnShowMovies = onShowMovies,
                ComingSoonMovies = comingSoonMovies
            };

            return View(viewModel);
        }
    }
}
