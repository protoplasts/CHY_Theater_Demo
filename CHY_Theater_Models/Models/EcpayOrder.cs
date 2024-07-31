using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CHY_Theater_Models.Models
{
    public partial class EcpayOrder
    {
        [Key]
        public int EcpayOrdersID { get; set; }

        [ForeignKey("Booking")]
        public int BookingId { get; set; }  // Change this to match Booking's primary key

        [Required]
        [StringLength(50)]
        public string MerchantTradeNo { get; set; }

        [StringLength(50)]
        public string MemberID { get; set; }

        public int? RtnCode { get; set; }

        [StringLength(50)]
        public string RtnMsg { get; set; }

        [StringLength(50)]
        public string TradeNo { get; set; }

        public int? TradeAmt { get; set; }

        public DateTime? PaymentDate { get; set; }

        [StringLength(50)]
        public string PaymentType { get; set; }

        [StringLength(50)]
        public string PaymentTypeChargeFee { get; set; }

        [StringLength(50)]
        public string TradeDate { get; set; }

        public int? SimulatePaid { get; set; }

        // Navigation property
        public virtual Booking Booking { get; set; }
    }
}
