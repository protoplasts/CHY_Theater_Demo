using static CHY_Theater.Areas.Booking.Models.ViewModels.BookingSelectionViewModel;

namespace FUEN104_2_FinalProject.Models.ViewModels
{
    public class ConfirmSelectionViewModel
    {
        public string? MerchantTradeNo { get; set; }

        public int? MovieTotalPrice { get; set; }
        public string? CouponCode { get; set; }
     
        public int? MovieId { get; set; }
        public int Auditoriumid { get; set; }
        public int Showid { get; set; }
        public int? SelectedTicketTypeId { get; set; }
        public string MovieName { get; set; }
        public string MovieEnglishName { get; set; }
        public string Level { get; set; }
        public string MovieImg { get; set; }

        public int AppliedPoints { get; set; }

        public DateTime ShowDateTime { get; set; }
        public string AuditoriumName { get; set; }
        public List<SeatInfon> SelectedSeats { get; set; }
        public List<SelectedSnack>? SelectedSnacks { get; set; }
        public List<SelectedTicket> SelectedTickets { get; set; }

        public class SeatInfon
        {
            public string SeatID { get; set; }
            public string SeatRow { get; set; }
            public string SeatNumber { get; set; }
        }
        public class SelectedSnack
        {
            public int SnackId { get; set; }
            public string SnackName { get; set; }
            public int Price { get; set; }
            public int Quantity { get; set; }
        }
        public class SelectedTicket
        {
            public int TicketTypeId { get; set; }

            public string TypeName { get; set; } = null!;

            public int Quantity { get; set; }

            public string? TicketDescription { get; set; }

            public int Price { get; set; }
            public List<TicketDescriptionItem> ParsedTicketDescription
            {
                get
                {
                    if (string.IsNullOrEmpty(TicketDescription))
                        return new List<TicketDescriptionItem>();

                    return TicketDescription.Split(';')
                        .Select(item => {
                            var parts = item.Split(':');
                            return new TicketDescriptionItem
                            {
                                Description = parts[0],
                                Quantity = int.Parse(parts[1])
                            };
                        })
                        .ToList();
                }
            }
            public class TicketDescriptionItem
            {
                public string Description { get; set; }
                public int Quantity { get; set; }
            }
        }
    }
}
