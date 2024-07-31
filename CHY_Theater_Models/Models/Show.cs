using System;
using System.Collections.Generic;

namespace CHY_Theater_Models.Models;

public partial class Show
{
    public int ShowId { get; set; }

    public int MovieId { get; set; }

    public int AuditoriumId { get; set; }

    public DateTime ShowDateTime { get; set; }

    public virtual Auditorium Auditorium { get; set; } = null!;

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual Movie Movie { get; set; } = null!;

    public virtual ICollection<ShowSeat> ShowSeats { get; set; } = new List<ShowSeat>();

    public virtual ICollection<Twin> Twins { get; set; } = new List<Twin>();
}
