using CHY_Theater.Service.IService;
using CHY_Theater_Models.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CHY_Theater.Service
{
    public class BirthdayCouponService : BackgroundService
    {
        private readonly IServiceProvider _services;
        private readonly ILogger<BirthdayCouponService> _logger;

        public BirthdayCouponService(IServiceProvider services,
                                     ILogger<BirthdayCouponService> logger)
        {
            _services = services;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await ProcessBirthdayCoupons();

                // Wait until next day
                await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
            }
        }

        private async Task ProcessBirthdayCoupons()
        {
            using var scope = _services.CreateScope();
            var userManager = scope.ServiceProvider
                .GetRequiredService<UserManager<ApplicationUser>>();
            var userCouponService = scope.ServiceProvider
                .GetRequiredService<IUserCouponService>();

            var today = DateOnly.FromDateTime(DateTime.Today);
            if (today.Day == 1)  // Only process on the first day of each month
            {
                var users = await userManager.Users
                    .Where(u => u.Birthday.HasValue && u.Birthday.Value.Month == today.Month)
                    .ToListAsync();

                foreach (var user in users)
                {
                    try
                    {
                        await userCouponService.CreateBirthdayCoupon(user.Id);
                        _logger.LogInformation($"Created birthday coupon for user {user.Id}");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Error creating birthday coupon for user {user.Id}");
                    }
                }
            }
        }
    }
}
