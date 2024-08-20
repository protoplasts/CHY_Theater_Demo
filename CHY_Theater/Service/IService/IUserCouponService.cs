using CHY_Theater_Models.Models;

namespace CHY_Theater.Service.IService
{
    public interface IUserCouponService
    {
        Task CreateNewUserCoupon(string userId);
        Task CreateBirthdayCoupon(string userId);

        Task CreateBirthdayCouponForNewUser(string userId);
        Task<List<UserCoupon>> GetUserCoupons(string userId);
        Task<List<UserCoupon>> GetAllCoupons(string userId);

        Task UseUserCoupon(int userCouponId);
    }
}
