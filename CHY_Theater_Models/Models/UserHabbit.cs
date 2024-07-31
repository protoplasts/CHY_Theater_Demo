using System;
using System.Collections.Generic;

namespace CHY_Theater_Models.Models;

public partial class UserHabbit
{
    public int HabbitId { get; set; }

    public int UserId { get; set; }

    public int? Other { get; set; }

    public virtual Habbit Habbit { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
