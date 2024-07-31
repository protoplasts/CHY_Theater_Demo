using System;
using System.Collections.Generic;

namespace CHY_Theater_Models.Models;

public partial class Seat
{
    public int SeatId { get; set; }

    public int? AuditoriumId { get; set; }

    public int SeatNumber { get; set; }

    public string SeatRow { get; set; } = null!;

    public string SeatType { get; set; } = null!;

    public string SeatStatus { get; set; } = null!;

    public virtual Auditorium? Auditorium { get; set; }

    public virtual ICollection<ShowSeat> ShowSeats { get; set; } = new List<ShowSeat>();
}
