using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CHY_Theater_Models
{
    public partial class News
    {
        public int NewsId { get; set; }

        public string NewsType { get; set; } = null!;

        public string NewsTitle { get; set; } = null!;

        public string? NewsDescription { get; set; }
        public string? NewsNotice { get; set; }
        public string? NewsPoster { get; set; }
        public DateOnly StartDate { get; set; }

        public DateOnly ExpiryDate { get; set; }





    }
}
