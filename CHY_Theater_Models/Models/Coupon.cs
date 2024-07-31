using System;
using System.Collections.Generic;

namespace CHY_Theater_Models.Models;

public partial class Coupon
{
    public int CouponId { get; set; }

    public string CouponCode { get; set; } = null!;

    public string DiscountType { get; set; } = null!;

    public decimal DiscountValue { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly ExpiryDate { get; set; }

    public int? MaxUsageCount { get; set; }

    public int? CurrentUsageCount { get; set; }

    public string? Description { get; set; }

    public virtual ICollection<BookingCoupon> BookingCoupons { get; set; } = new List<BookingCoupon>();
}
