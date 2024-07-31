using System;
using System.Collections.Generic;

namespace CHY_Theater_Models.Models;

public partial class Theater
{
    public int TheaterId { get; set; }

    public string TheaterName { get; set; } = null!;

    public string TheaterPhone { get; set; } = null!;

    public string TheaterEmail { get; set; } = null!;

    public string TheaterLocation { get; set; } = null!;

    public string? TheaterDescription { get; set; }

    public TimeOnly TheaterStartTime { get; set; }

    public TimeOnly TheaterEndTime { get; set; }

    public string? TheaterImage { get; set; }

    public virtual ICollection<Auditorium> Auditoria { get; set; } = new List<Auditorium>();
}
