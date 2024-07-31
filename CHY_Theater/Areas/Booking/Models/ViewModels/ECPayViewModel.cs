namespace CHY_Theater.Areas.Booking.Models.ViewModels
{
    public class ECPayViewModel
    {
        public int TotalAmount { get; set; }
        public string? MerchantTradeNo { get; set; }

        public int ECPID { get; set; }
        public string ChannelID { get; set; }
        public string? MerchantID { get; set; }
        public string ItemDesc { get; set; }
        public string Amt { get; set; }
        public string ExpireDate { get; set; }
        public string? ReturnURL { get; set; }
        public string? CustomerURL { get; set; }
        public string? NotifyURL { get; set; }
        public string? ClientBackURL { get; set; }
        public string Email { get; set; }
        public string PayOption { get; set; }
    }
}
