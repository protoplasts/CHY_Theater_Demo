using CHY_Theater.Areas.Identity.Models.ViewModels;
using CHY_Theater_DataAcess.Data;
using CHY_Theater_Models.Models;
using Microsoft.EntityFrameworkCore;

namespace CHY_Theater.Areas.Identity.Services
{
    public class RewardPointService : IRewardPointService
    {
        private readonly Theater_ProjectDbContext _context;

        public RewardPointService(Theater_ProjectDbContext context)
        {
            _context = context;
        }
        public async Task<int> GetTotalPointsAsync(string userId)
        {
            var now = DateTime.Now;
            return await _context.RewardPoints
                .Where(rp => rp.UserId == userId && !rp.IsUsed && rp.ExpiryDate > now)
                .SumAsync(rp => rp.Points);
        }
        public async Task<IQueryable<RewardPointDetails>> GetPointDetailsAsync(string userId, bool isAvailable)
        {
            var now = DateTime.Now;
            return _context.RewardPoints
                .Where(rp => rp.UserId == userId)
                .Select(rp => new RewardPointDetails
                {
                    Points = rp.Points,
                    EarnedDate = rp.EarnedDate,
                    ExpiryDate = rp.ExpiryDate,
                    IsAvailable = !rp.IsUsed && rp.ExpiryDate > now,
                    UnavailableReason = rp.IsUsed ? "已使用" : (rp.ExpiryDate <= now ? "已過期" : null)
                })
                .Where(rpd => rpd.IsAvailable == isAvailable)
                .OrderBy(rpd => rpd.ExpiryDate);
        }
        public async Task<bool> UsePointsAsync(string userId, int pointsToUse)
        {
            var now = DateTime.Now;
            var availablePoints = await _context.RewardPoints
                .Where(rp => rp.UserId == userId && !rp.IsUsed && rp.ExpiryDate > now)
                .OrderBy(rp => rp.ExpiryDate)
                .ToListAsync();

            if (availablePoints.Sum(rp => rp.Points) < pointsToUse)
            {
                return false;
            }

            int remainingPointsToUse = pointsToUse;
            foreach (var point in availablePoints)
            {
                if (remainingPointsToUse <= 0) break;

                if (point.Points <= remainingPointsToUse)
                {
                    point.IsUsed = true;
                    remainingPointsToUse -= point.Points;
                }
                else
                {
                    var newPoint = new RewardPoint
                    {
                        UserId = userId,
                        Points = point.Points - remainingPointsToUse,
                        EarnedDate = point.EarnedDate,
                        ExpiryDate = point.ExpiryDate,
                        IsUsed = false,
                        TransactionId = point.TransactionId
                    };
                    point.Points = remainingPointsToUse;
                    point.IsUsed = true;
                    _context.RewardPoints.Add(newPoint);
                    remainingPointsToUse = 0;
                }
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task AddPointsAsync(string userId, int points, int transactionId)
        {
            var rewardPoint = new RewardPoint
            {
                UserId = userId,
                Points = points,
                EarnedDate = DateTime.Now,
                ExpiryDate = DateTime.Now.AddYears(1),
                IsUsed = false,
                TransactionId = transactionId
            };

            _context.RewardPoints.Add(rewardPoint);
            await _context.SaveChangesAsync();
        }
    }

}
