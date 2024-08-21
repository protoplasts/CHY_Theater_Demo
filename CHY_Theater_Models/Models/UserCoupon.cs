using System;
using System.Collections.Generic;

namespace CHY_Theater_Models.Models;

public partial class UserCoupon
{
    public int UserCouponId { get; set; }
    public string UserId { get; set; }
    public int CouponId { get; set; }
    public bool IsUsed { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ExpiredAt { get; set; }

    public DateTime? UsedAt { get; set; }

    public virtual ApplicationUser User { get; set; }
    public virtual Coupon Coupon { get; set; }
}
