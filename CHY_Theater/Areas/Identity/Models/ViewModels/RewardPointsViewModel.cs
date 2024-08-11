using CHY_Theater_Models.Models;
using Microsoft.EntityFrameworkCore;

namespace CHY_Theater.Areas.Identity.Models.ViewModels
{
    public class RewardPointsViewModel
    {
        public int TotalPoints { get; set; }
        public int AvailablePoints { get; set; }
        public int LatestPoints { get; set; }
        public DateTime? LatestTransactionDate { get; set; }
        public List<PaymentTransaction> Transactions { get; set; }
        public PaginatedList<RewardPointDetails> PointDetails { get; set; }
        public string CurrentTab { get; set; } = "available";
    }

    public class RewardPointDetails
    {
        public int Points { get; set; }
        public DateTime EarnedDate { get; set; }
        public DateTime ExpiryDate { get; set; }
        public bool IsAvailable { get; set; }
        public string UnavailableReason { get; set; }
    }

    public class PaginatedList<T> : List<T>
    {
        public int PageIndex { get; private set; }
        public int TotalPages { get; private set; }

        public PaginatedList(List<T> items, int count, int pageIndex, int pageSize)
        {
            PageIndex = pageIndex;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            this.AddRange(items);
        }

        public bool HasPreviousPage => PageIndex > 1;
        public bool HasNextPage => PageIndex < TotalPages;

        public static async Task<PaginatedList<T>> CreateAsync(IQueryable<T> source, int pageIndex, int pageSize)
        {
            var count = await source.CountAsync();
            var items = await source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
            return new PaginatedList<T>(items, count, pageIndex, pageSize);
        }
    }
}
