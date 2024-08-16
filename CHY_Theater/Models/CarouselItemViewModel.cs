namespace CHY_Theater.Models
{
    public class CarouselItemViewModel
    {
        public bool IsMovie { get; set; }
        public int Id { get; set; }
        public string Title { get; set; }
        public string SubTitle { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public string ButtonLink { get; set; }
        public string ButtonText { get; set; }
        public List<string> Tags { get; set; }
        public int? Rating { get; set; }
        public int? Year { get; set; }
    }
}
