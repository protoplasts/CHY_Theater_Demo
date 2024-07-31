using System;
using System.Collections.Generic;

namespace CHY_Theater_Models.Models;

public partial class ShowSeat
{
    public int ShowSeatId { get; set; }

    public int? ShowId { get; set; }

    public int? SeatId { get; set; }

    public string ShowSeatStatus { get; set; } = null!;

    public virtual ICollection<BookingSeatsDetail> BookingSeatsDetails { get; set; } = new List<BookingSeatsDetail>();

    public virtual Seat? Seat { get; set; }

    public virtual Show? Show { get; set; }
}
