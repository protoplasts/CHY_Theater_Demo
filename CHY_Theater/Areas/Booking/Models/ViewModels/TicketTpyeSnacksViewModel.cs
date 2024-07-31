using CHY_Theater_Models.Models;

namespace CHY_Theater.Areas.Booking.Models.ViewModels
{
    
        public class TicketTpyeSnacksViewModel
        {
            public Movie Movie { get; set; }
            public Show Show { get; set; }

            public List<SnacksInfo> Snacks { get; set; }
            public List<TicketTypeInfo> TicketTypes { get; set; }
            public class SnacksInfo
            {
                public int SnackId { get; set; }

                public string SnackName { get; set; }
                public string SnackSize { get; set; }
                public int Price { get; set; }
            }
            public class TicketTypeInfo
            {
                public int TicketTypeId { get; set; }

                public string TypeName { get; set; } = null!;

                public int Quantity { get; set; }

                public string? TicketDescription { get; set; }

                public int Price { get; set; }
                public int HowManySeatForType { get; set; }

        }
            public BookingSelectionViewModel BookingSelection { get; set; }

        }
    }
