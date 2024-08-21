using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CHY_Theater_Models.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        public string Name { get; set; }
        [NotMapped]
        public string RoleId { get; set; }
        [NotMapped]
        public string Role { get; set; }
        [NotMapped]
        public string UserClaim { get; set; }
        public DateTime DateCreated { get; set; }
        // New properties
        public string MembershipLevel { get; set; }
        public int MemberPoints { get; set; }
        public DateTime? LastTicketPurchase { get; set; }
        public decimal TotalSpent { get; set; }
        public DateTime? LastLoginTime { get; set; }

        //detail infor

        public string? Address { get; set; }
        public DateTime? Birthday { get; set; }
        public virtual ICollection<UserCoupon> UserCoupons { get; set; } = new List<UserCoupon>();


    }

}
