using System;
using System.Collections.Generic;

namespace CHY_Theater_Models.Models;

public partial class Booking
{
    public int BookingId { get; set; }

    public string? UserId { get; set; }

    public int? ShowingId { get; set; }

    public DateTime BookingDate { get; set; }

    public string? MerchantTradeNo { get; set; }

    public string? BookingStatus { get; set; }

    public virtual ICollection<BookingCoupon> BookingCoupons { get; set; } = new List<BookingCoupon>();

    public virtual ICollection<BookingSeatsDetail> BookingSeatsDetails { get; set; } = new List<BookingSeatsDetail>();

    public virtual ICollection<BookingSnack> BookingSnacks { get; set; } = new List<BookingSnack>();

    public virtual ICollection<BookingTicketTypesDetail> BookingTicketTypesDetails { get; set; } = new List<BookingTicketTypesDetail>();

    public virtual ICollection<PaymentTransaction> PaymentTransactions { get; set; } = new List<PaymentTransaction>();

    public virtual Show? Showing { get; set; }
}
