using System;
using System.Collections.Generic;

namespace CHY_Theater_Models.Models;

public partial class Rating
{
    public int MovieId { get; set; }

    public int UserId { get; set; }

    public int Rating1 { get; set; }

    public virtual Movie Movie { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
