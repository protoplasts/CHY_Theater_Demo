using CHY_Theater_Models.Models;

namespace CHY_Theater.Areas.Booking.Models.ViewModels
{
    public class TheaterAuditoriumsViewModel
    {
        public int TheaterId { get; set; }
        public List<Theater> Theaters { get; set; }
        public List<AuditoriumInfo> Auditoriums { get; set; }


        public class AuditoriumInfo
        {
            public string AuditoriumName { get; set; }
            public string AuditoriumType { get; set; }

            public int AuditoriumId { get; set; }
            public List<ShowInfo> Shows { get; set; }
        }
        
        public class ShowInfo
        {
            public int ShowId { get; set; }
            public int MovieId { get; set; }
            public string MovieName { get; set; }
            public int Level { get; set; }
            public DateTime ShowDateTime { get; set; }
            // Add any other Show properties you need to display
        }

    }
}
