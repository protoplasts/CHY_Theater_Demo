using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CHY_Theater_Models.Models
{
    public class RewardPoint
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int Points { get; set; }
        public DateTime EarnedDate { get; set; }
        public DateTime ExpiryDate { get; set; }
        public bool IsUsed { get; set; }
        public int? TransactionId { get; set; }

        public ApplicationUser User { get; set; }
        public PaymentTransaction Transaction { get; set; }
    }
}
