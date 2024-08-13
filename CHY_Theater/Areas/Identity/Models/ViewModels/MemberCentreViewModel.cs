namespace CHY_Theater.Areas.Identity.Models.ViewModels
{
    // BookingViewModel.cs
    public class MemberCentreViewModel
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public bool EmailConfirmed { get; set; }
        public string MembershipLevel { get; set; }
        public int MemberPoints { get; set; }
        public string Id { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? LastTicketPurchase { get; set; }
        public int TotalSpent { get; set; }
        public DateTime? LastLoginTime { get; set; }

    }
}
