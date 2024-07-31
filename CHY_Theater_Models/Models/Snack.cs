using System;
using System.Collections.Generic;

namespace CHY_Theater_Models.Models;

public partial class Snack
{
    public int SnackId { get; set; }

    public string SnackName { get; set; } = null!;

    public string SnackSize { get; set; } = null!;

    public string? SnackImages { get; set; }

    public int Price { get; set; }

    public virtual ICollection<BookingSnack> BookingSnacks { get; set; } = new List<BookingSnack>();
}
