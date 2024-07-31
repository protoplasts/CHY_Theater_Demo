using System;
using System.Collections.Generic;

namespace CHY_Theater_Models.Models;

public partial class Auditorium
{
    public int AuditoriumId { get; set; }

    public int? TheaterId { get; set; }

    public string AuditoriumName { get; set; } = null!;

    public int TotalSeats { get; set; }

    public string? AuditoriumType { get; set; }

    public virtual ICollection<Seat> Seats { get; set; } = new List<Seat>();

    public virtual ICollection<Show> Shows { get; set; } = new List<Show>();

    public virtual Theater? Theater { get; set; }
}
