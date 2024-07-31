using System;
using System.Collections.Generic;

namespace CHY_Theater_Models.Models;

public partial class BookingCoupon
{
    public int BookingCouponId { get; set; }

    public int? BookingId { get; set; }

    public int? CouponId { get; set; }

    public virtual Booking? Booking { get; set; }

    public virtual Coupon? Coupon { get; set; }
}
