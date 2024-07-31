using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CHY_Theater_Models.Models
{
    public partial class Event
    {
        public int EventId { get; set; }

        public string EventType { get; set; } = null!;

        public string EventTitle { get; set; } = null!;

        public string? EventDescription { get; set; }
        public string? EventNotice { get; set; }
        public string? EventImage { get; set; }
        public DateOnly StartDate { get; set; }

        public DateOnly ExpiryDate { get; set; }


    }
}
