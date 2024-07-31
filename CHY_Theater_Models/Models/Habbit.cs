using System;
using System.Collections.Generic;

namespace CHY_Theater_Models.Models;

public partial class Habbit
{
    public int HabbitId { get; set; }

    public string Habbit1 { get; set; } = null!;

    public virtual ICollection<UserHabbit> UserHabbits { get; set; } = new List<UserHabbit>();
}
