using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CHY_Theater_DataAcess.Data;
using CHY_Theater_Models.Models;
using Newtonsoft.Json;

namespace CHY_Theater.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class RevenueController : Controller
    {
        private readonly Theater_ProjectDbContext _context;

        public RevenueController(Theater_ProjectDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Revenue
        public async Task<IActionResult> Index()
        {
            var theater_ProjectDbContext = _context.PaymentTransactions.Include(p => p.Booking);
            return View(await theater_ProjectDbContext.ToListAsync());
        }
        public async Task<IActionResult> Revenue()
        {
            try
            {
                var transactions = await _context.PaymentTransactions
                    .Where(t => t.RtnCode == 1 && t.PaymentDate.HasValue)
                    .OrderBy(t => t.PaymentDate)
                    .ToListAsync();

                var totalRevenue = transactions.Sum(t => t.TradeAmt ?? 0);

                var yearlyRevenue = transactions
                    .GroupBy(t => t.PaymentDate.Value.Year)
                    .Select(g => new
                    {
                        Date = g.Key.ToString(),
                        Revenue = g.Sum(t => t.TradeAmt ?? 0)
                    })
                    .OrderBy(r => r.Date)
                    .ToList();

                var monthlyRevenue = transactions
                    .GroupBy(t => new { Year = t.PaymentDate.Value.Year, Month = t.PaymentDate.Value.Month })
                    .Select(g => new
                    {
                        Date = $"{g.Key.Year}-{g.Key.Month:D2}",
                        Revenue = g.Sum(t => t.TradeAmt ?? 0)
                    })
                    .OrderBy(r => r.Date)
                    .ToList();

                // 修改每日收入計算，確保過去30天的每一天都有數據
                var thirtyDaysAgo = DateTime.Now.Date.AddDays(-29); // 注意這裡改成 -29，以包含今天
                var dailyRevenue = Enumerable.Range(0, 30)
                    .Select(offset => thirtyDaysAgo.AddDays(offset))
                    .GroupJoin(
                        transactions.Where(t => t.PaymentDate >= thirtyDaysAgo),
                        date => date.Date,
                        trans => trans.PaymentDate.Value.Date,
                        (date, trans) => new
                        {
                            Date = date.ToString("yyyy-MM-dd"),
                            Revenue = trans.Sum(t => t.TradeAmt ?? 0)
                        }
                    )
                    .OrderBy(r => r.Date)
                    .ToList();

                ViewBag.TotalRevenue = totalRevenue;
                ViewBag.YearlyRevenue = JsonConvert.SerializeObject(yearlyRevenue);
                ViewBag.MonthlyRevenue = JsonConvert.SerializeObject(monthlyRevenue);
                ViewBag.DailyRevenue = JsonConvert.SerializeObject(dailyRevenue);

                return View();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return View("Error");
            }
        }
        private bool PaymentTransactionExists(int id)
        {
            return _context.PaymentTransactions.Any(e => e.TransactionId == id);
        }
    }
}
