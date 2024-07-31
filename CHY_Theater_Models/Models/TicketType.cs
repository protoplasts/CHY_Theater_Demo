using System;
using System.Collections.Generic;

namespace CHY_Theater_Models.Models;

public partial class TicketType
{
    public int TicketTypeId { get; set; }

    public string TypeName { get; set; } = null!;

    public int HowManySeatForType { get; set; }

    public string? TicketDescription { get; set; }

    public int Price { get; set; }

    public virtual ICollection<BookingTicketTypesDetail> BookingTicketTypesDetails { get; set; } = new List<BookingTicketTypesDetail>();
}
