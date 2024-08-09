using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CHY_Theater_Models.Models
{
    public partial class CarouselItem
    {
        public int CarouselItemId { get; set; }

        public int MovieId { get; set; }
        public string ImagePath { get; set; } = null!;
        public virtual Movie Movie { get; set; } = null!;

    }
}
