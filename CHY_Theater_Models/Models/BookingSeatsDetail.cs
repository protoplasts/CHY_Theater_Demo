using System;
using System.Collections.Generic;

namespace CHY_Theater_Models.Models;

public partial class BookingSeatsDetail
{
    public int DetailId { get; set; }

    public int? BookingId { get; set; }

    public int? ShowSeatId { get; set; }

    public virtual Booking? Booking { get; set; }

    public virtual ShowSeat? ShowSeat { get; set; }
}
