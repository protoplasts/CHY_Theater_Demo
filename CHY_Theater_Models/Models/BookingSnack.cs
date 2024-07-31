using System;
using System.Collections.Generic;

namespace CHY_Theater_Models.Models;

public partial class BookingSnack
{
    public int BookingSnackId { get; set; }

    public int? BookingId { get; set; }

    public int? SnackId { get; set; }

    public int Quantity { get; set; }

    public virtual Booking? Booking { get; set; }

    public virtual Snack? Snack { get; set; }
}
