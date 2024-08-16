using CHY_Theater.Models;
using CHY_Theater_DataAcess.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CHY_Theater.Areas.Customer.ViewComponents
{
    public class CarouselViewComponent : ViewComponent
    {
        private readonly Theater_ProjectDbContext _context;

        public CarouselViewComponent(Theater_ProjectDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var carouselItems = new List<CarouselItemViewModel>();

            // Fetch movies
            var movies = await _context.Movies
                .Where(m => m.OnFous == 1)
                .OrderByDescending(m => m.ReleaseDate)
                .Take(2) // Limit to 2 movies
                .Select(m => new CarouselItemViewModel
                {
                    IsMovie = true,
                    Id = m.MovieId,
                    Title = m.MovieName,
                    SubTitle = m.MovieEnglishName,
                    Description = m.MovieDescription,
                    ImageUrl = m.MovieImageHorizontal,
                    ButtonLink = m.Movievideo,
                    ButtonText = "觀賞預告",
                    Tags = m.MovieClasses.Select(mc => mc.Class.ClassName).ToList(),
                    Rating = m.IMDb,
                    Year = m.ReleaseDate.Year
                })
                .ToListAsync();

            carouselItems.AddRange(movies);

            // Fetch the most recent event
            var eventItem = await _context.Events
                .OrderByDescending(e => e.StartDate)
                .Select(e => new CarouselItemViewModel
                {
                    IsMovie = false,
                    Id = e.EventId,
                    Title = e.EventTitle,
                    SubTitle = e.EventType,
                    Description = e.EventDescription,
                    ImageUrl = e.EventImage,
                    ButtonLink = $"/Event/Details/{e.EventId}", // Adjust this route as needed
                    ButtonText = "查看活動詳情",
                    Tags = new List<string> { e.EventType },
                    Year = e.StartDate.Year
                })
                .FirstOrDefaultAsync();

            if (eventItem != null)
            {
                carouselItems.Add(eventItem);
            }

            return View(carouselItems);
        }
    }
}
