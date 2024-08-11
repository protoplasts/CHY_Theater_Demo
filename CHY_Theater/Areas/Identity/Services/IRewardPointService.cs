using CHY_Theater.Areas.Identity.Models.ViewModels;
using CHY_Theater_Models.Models;

namespace CHY_Theater.Areas.Identity.Services
{
    public interface IRewardPointService
    {
        Task<int> GetTotalPointsAsync(string userId);
        Task<bool> UsePointsAsync(string userId, int pointsToUse);
        Task AddPointsAsync(string userId, int points, int transactionId);
        Task<IQueryable<RewardPointDetails>> GetPointDetailsAsync(string userId, bool isAvailable);
    }
}
