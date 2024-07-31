namespace CHY_Theater.Areas.Identity.Models.ViewModels
{
    // BookingViewModel.cs
    public class BookingViewModel
    {
        public int BookingId { get; set; }
        public DateTime BookingDate { get; set; }
        public string MerchantTradeNo { get; set; }
        public string BookingStatus { get; set; }
        public string MovieTitle { get; set; }
        public DateTime ShowTime { get; set; }
        public string TheaterName { get; set; }
        public string AuditoriumName { get; set; }
        public string PaymentTypes { get; set; }

        public List<string> SeatDetails { get; set; }
        public List<string>? TicketTypes { get; set; }
        public List<string>? Snacks { get; set; }
        public decimal? TotalAmount { get; set; }
    }
}
