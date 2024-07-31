using CHY_Theater_Models.Models;

namespace CHY_Theater.Areas.Customer.Models.ViewModels
{
	public class MovieViewModel
	{
		public List<Movie> OnShowMovies { get; set; }
		public List<Movie> ComingSoonMovies { get; set; }
	}
}
