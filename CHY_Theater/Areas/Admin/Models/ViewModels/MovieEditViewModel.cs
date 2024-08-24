using CHY_Theater_Models.Models;

namespace CHY_Theater.Areas.Admin.Models.ViewModels
{
    public class MovieEditViewModel
    {
        public Movie? Movie { get; set; }
        public List<MovieActorViewModel>? MovieActors { get; set; }
        public List<Actor>? AllActors { get; set; }
    }
}
