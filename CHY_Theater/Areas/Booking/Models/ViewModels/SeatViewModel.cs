
using CHY_Theater.Areas.Booking.Models.ViewModels;
using CHY_Theater_Models.Models;
using static FUEN104_2_FinalProject.Models.ViewModels.ConfirmSelectionViewModel;

namespace FUEN104_2_FinalProject.Models.ViewModels
{
    public class SeatViewModel
    {
        public string MovieName { get; set; }
        public string MovieEnglishName { get; set; }
        public string MovieImag { get; set; }

        public DateTime ShowDateTime { get; set; }
        public int Level { get; set; }
        public int TotalSeats { get; set; }

        public string AuditoriumName { get; set; }
        public string Auditoriumtype { get; set; }
        public string selectedTicketName { get; set; }
        public string selectedTicketDescription { get; set; }
        public Movie Movie { get; set; }
        public List<SeatInfo> SeatList { get; set; }
        public List<TicketTypeInfo> TicketTypes { get; set; }
        public BookingSelectionViewModel BookingSelection { get; set; }
        public List<SelectedSnack> SelectedSnacks { get; set; }

        public class SeatInfo
        {
            public int SeatID { get; set; }
            public int SeatNumber { get; set; }
            public string SeatRow { get; set; }
            public string SeatType { get; set; }
            public string SeatStatus { get; set; }
        }

        public class TicketTypeInfo
        {
            public int TicketTypeId { get; set; }
            public string TypeName { get; set; } = null!;
            public int Quantity { get; set; }
            public string? TicketDescription { get; set; }
            public int Price { get; set; }
			public string GetFormattedDescription()
			{
				if (string.IsNullOrEmpty(TicketDescription))
					return $"{TypeName} X {Quantity}";

				var items = TicketDescription.Split(';')
					.Select(item => {
						var parts = item.Split(':');
						return new
						{
							Description = parts[0],
							Quantity = int.Parse(parts[1])
						};
					});

				return string.Join(", ", items.Select(i => $"{i.Description} X {i.Quantity}"));
			}


		}
        public string GetCombinedTicketDescription()
        {
            var combinedItems = new Dictionary<string, int>();

            foreach (var tt in TicketTypes)
            {
                if (!string.IsNullOrEmpty(tt.TicketDescription))
                {
                    var items = tt.TicketDescription.Split(';');
                    foreach (var item in items)
                    {
                        var parts = item.Split(':');
                        var description = parts[0].Trim();
                        var quantity = int.Parse(parts[1].Trim());
                        quantity *= tt.Quantity;

                        if (combinedItems.ContainsKey(description))
                            combinedItems[description] += quantity;
                        else
                            combinedItems[description] = quantity;
                    }
                }
                else
                {
                    var description = tt.TypeName.Trim();
                    if (combinedItems.ContainsKey(description))
                        combinedItems[description] += tt.Quantity;
                    else
                        combinedItems[description] = tt.Quantity;
                }
            }

            return string.Join(", ", combinedItems.Select(kv => $"{kv.Key} X {kv.Value}"));
        }

    }
}
