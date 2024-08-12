namespace CHY_Theater.Areas.Booking.Models.ViewModels
{
    public class BookingSelectionViewModel
    {
        public int ShowId { get; set; }
        public int MovieId { get; set; }
        public int? TheaterId { get; set; }
        public int Auditoriumid { get; set; }
        public List<SnackSelection> SelectedSnacks { get; set; } = new List<SnackSelection>();
        public List<TicketSelection> SelectedTickets { get; set; } = new List<TicketSelection>();

        public int SelectedTicketTypeId { get; set; }
        public int Quantity { get; set; }
        public int TotalHowManySeat { get; set; }

        public int TotalPrice { get; set; }

        public class SnackSelection
        {
            public int SnackId { get; set; }
            public string? SnackName { get; set; }
            public int Price { get; set; }
            public int Quantity { get; set; }
        }
        public class TicketSelection
        {
            public int TicketTypeId { get; set; }

            public string TypeName { get; set; } = null!;

            public int Quantity { get; set; }

            public string? TicketDescription { get; set; }

            public int Price { get; set; }
            public int HowManySeat { get; set; }

        }
    }
}
