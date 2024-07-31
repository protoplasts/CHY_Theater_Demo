using System;
using System.Collections.Generic;

namespace CHY_Theater_Models.Models;

public partial class MovieClass
{
    public int MovieId { get; set; }

    public int ClassId { get; set; }

    public int? Other { get; set; }

    public virtual Class Class { get; set; } = null!;

    public virtual Movie Movie { get; set; } = null!;
}
