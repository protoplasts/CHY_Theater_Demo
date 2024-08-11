using CHY_Theater.Areas.Identity.Models.ViewModels;
using CHY_Theater.Areas.Identity.Services;
using CHY_Theater_DataAcess.Data;
using CHY_Theater_Models.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CHY_Theater.Areas.Identity.Controllers
{
    [Area("Identity")]

    public class RewardPointsController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly Theater_ProjectDbContext _context;
        private readonly IRewardPointService _rewardPointService;

        public RewardPointsController(UserManager<ApplicationUser> userManager, IRewardPointService rewardPointService, Theater_ProjectDbContext context)
        {
            _userManager = userManager;
            _context = context; 
            _rewardPointService = rewardPointService;

        }

        //[Authorize]
        //public async Task<IActionResult> Index()
        //{
        //    var user = await _userManager.GetUserAsync(User);
        //    if (user == null)
        //    {
        //        return NotFound();
        //    }

        //    var oneYearAgo = DateTime.Now.AddYears(-1);

        //    var transactions = await _context.PaymentTransactions
        //        .Where(pt => pt.MemberID == user.Id
        //                  && pt.PaymentDate >= oneYearAgo
        //                  && pt.RtnMsg == "已付款")
        //        .OrderByDescending(pt => pt.PaymentDate)
        //        .ToListAsync();

        //    var totalPoints = transactions.Sum(t => t.Points);
        //    var latestTransaction = transactions.FirstOrDefault();

        //    var viewModel = new RewardPointsViewModel
        //    {
        //        TotalPoints = totalPoints,
        //        LatestPoints = latestTransaction?.Points ?? 0,
        //        LatestTransactionDate = latestTransaction?.PaymentDate,
        //        Transactions = transactions
        //    };

        //    return View(viewModel);
        //}
        [Authorize]
        public async Task<IActionResult> Index(string tab = "available", int pageIndex = 1)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }
            var oneYearAgo = DateTime.Now.AddYears(-1);
            var transactions = await _context.PaymentTransactions
                .Where(pt => pt.MemberID == user.Id
                          && pt.PaymentDate >= oneYearAgo
                          && pt.RtnMsg == "已付款")
                .OrderByDescending(pt => pt.PaymentDate)
                .ToListAsync();
            var oneYearPoints = await _context.RewardPoints
                .Where(pt => pt.UserId == user.Id && pt.EarnedDate >= oneYearAgo)
                .OrderByDescending(pt => pt.EarnedDate)
                .ToListAsync();

            var totalPoints = oneYearPoints.Sum(t => t.Points);
            var latestPoints = oneYearPoints.FirstOrDefault();

            // Get available points from the RewardPoint system
            var availablePoints = await _rewardPointService.GetTotalPointsAsync(user.Id);
            // Get point details
            var pointDetails = await _rewardPointService.GetPointDetailsAsync(user.Id, tab == "available");
            var viewModel = new RewardPointsViewModel
            {
                TotalPoints = totalPoints,
                AvailablePoints = availablePoints,
                LatestPoints = latestPoints?.Points ?? 0,
                LatestTransactionDate = latestPoints?.EarnedDate,
                PointDetails = await PaginatedList<RewardPointDetails>.CreateAsync(pointDetails, pageIndex, 10),
                CurrentTab = tab,

                Transactions = transactions
            };

            return View(viewModel);
        }


       
    }
}
