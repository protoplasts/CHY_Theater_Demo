using System;
using System.Collections.Generic;

namespace CHY_Theater_Models.Models;

public partial class MovieActor
{
    public int MovieId { get; set; }

    public int ActorId { get; set; }

    public int MainLevel { get; set; }

    public virtual Actor Actor { get; set; } = null!;

    public virtual Movie Movie { get; set; } = null!;
}
