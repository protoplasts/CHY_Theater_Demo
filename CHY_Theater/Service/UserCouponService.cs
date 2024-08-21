using CHY_Theater.Service.IService;
using CHY_Theater_DataAcess.Data;
using CHY_Theater_Models.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CHY_Theater.Service
{
    public class UserCouponService : IUserCouponService
    {
        private readonly Theater_ProjectDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public UserCouponService(UserManager<ApplicationUser> userManager, Theater_ProjectDbContext context, SignInManager<ApplicationUser> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task CreateNewUserCoupon(string userId)
        {
           
            var newUserCoupons = await _context.Coupons
         .Where(c => c.IsUserSpecific && c.Description == "新用戶禮")
         .ToListAsync();

            if (newUserCoupons != null)
            {
                foreach (var newUserCoupon in newUserCoupons)
                {
                    var userCoupon = new UserCoupon
                    {
                        UserId = userId,
                        CouponId = newUserCoupon.CouponId,
                        CreatedAt = DateTime.UtcNow,
                        ExpiredAt = DateTime.UtcNow.AddMonths(1)
                    };
                    _context.UserCoupons.Add(userCoupon);
                }
                // Check if it's the user's birth month
               
                await _context.SaveChangesAsync();
            }
        }
        public async Task CreateBirthdayCouponForNewUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user?.Birthday != null)
            {
                var today = DateOnly.FromDateTime(DateTime.Today);
                // Check if it's the first day of the user's birth month
                if (today.Month == user.Birthday.Value.Month )
                {
                    var birthdayCoupon = await _context.Coupons
                        .FirstOrDefaultAsync(c => c.IsUserSpecific && c.Description == "生日禮");

                    if (birthdayCoupon != null)
                    {
                        var userCoupon = new UserCoupon
                        {
                            UserId = userId,
                            CouponId = birthdayCoupon.CouponId,
                            CreatedAt = DateTime.UtcNow,
                            ExpiredAt = DateTime.UtcNow.AddMonths(1)

                        };

                        _context.UserCoupons.Add(userCoupon);
                        await _context.SaveChangesAsync();
                    }
                }
            }

        }
        public async Task CreateBirthdayCoupon(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user?.Birthday != null)
            {
                var today = DateOnly.FromDateTime(DateTime.Today);
                // Check if it's the first day of the user's birth month
                if (today.Month == user.Birthday.Value.Month && today.Day == 1)
                {
                    var birthdayCoupon = await _context.Coupons
                        .FirstOrDefaultAsync(c => c.IsUserSpecific && c.DiscountType == "Birthday");

                    if (birthdayCoupon != null)
                    {
                        var userCoupon = new UserCoupon
                        {
                            UserId = userId,
                            CouponId = birthdayCoupon.CouponId,
                            CreatedAt = DateTime.UtcNow,
                            ExpiredAt = DateTime.UtcNow.AddMonths(1)

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
