using System;
using System.Collections.Generic;

namespace CHY_Theater_Models.Models;

public partial class BookingTicketTypesDetail
{
    public int TicketTypesDetailId { get; set; }

    public int? BookingId { get; set; }

    public int? TicketTypeId { get; set; }
    public int Quantity { get; set; }


    public virtual Booking? Booking { get; set; }

    public virtual TicketType? TicketType { get; set; }
}
