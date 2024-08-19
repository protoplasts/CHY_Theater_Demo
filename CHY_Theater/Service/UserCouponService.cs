using CHY_Theater.Service.IService;
using CHY_Theater_DataAcess.Data;
using CHY_Theater_Models.Models;
using Microsoft.EntityFrameworkCore;

namespace CHY_Theater.Service
{
    public class UserCouponService : IUserCouponService
    {
        private readonly Theater_ProjectDbContext _context;

        public UserCouponService(Theater_ProjectDbContext context)
        {
            _context = context;
        }

        public async Task CreateNewUserCoupon(string userId)
        {
            var newUserCoupon = await _context.Coupons
                .FirstOrDefaultAsync(c => c.IsUserSpecific && c.DiscountType == "NewUser");

            if (newUserCoupon != null)
            {
                var userCoupon = new UserCoupon
                {
                    UserId = userId,
                    CouponId = newUserCoupon.CouponId,
                    CreatedAt = DateTime.UtcNow
                };

                _context.UserCoupons.Add(userCoupon);
                await _context.SaveChangesAsync();
            }
        }

        public async Task CreateBirthdayCoupon(string userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user?.Birthday != null)
            {
                var today = DateOnly.FromDateTime(DateTime.Today);

                // Check if it's the first day of the user's birth month
                if (today.Month == user.Birthday.Month && today.Day == 1)
                {
                    var birthdayCoupon = await _context.Coupons
                        .FirstOrDefaultAsync(c => c.IsUserSpecific && c.DiscountType == "Birthday");

                    if (birthdayCoupon != null)
                    {
                        var userCoupon = new UserCoupon
                        {
                            UserId = userId,
                            CouponId = birthdayCoupon.CouponId,
                            CreatedAt = DateTime.UtcNow
                        };

                        _context.UserCoupons.Add(userCoupon);
                        await _context.SaveChangesAsync();
                    }
                }
            }
        }

        public async Task<List<UserCoupon>> GetUserCoupons(string userId)
        {
            return await _context.UserCoupons
                .Where(uc => uc.UserId == userId && !uc.IsUsed)
                .Include(uc => uc.Coupon)
                .ToListAsync();
        }
        public async Task<List<UserCoupon>> GetAllCoupons(string userId)
        {
            return await _context.UserCoupons
                .Where(uc => uc.UserId == userId)
                .Include(uc => uc.Coupon)
                .ToListAsync();
        }
        public async Task UseUserCoupon(int userCouponId)
        {
            var userCoupon = await _context.UserCoupons.FindAsync(userCouponId);
            if (userCoupon != null)
            {
                userCoupon.IsUsed = true;
                userCoupon.UsedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }
    }
}
