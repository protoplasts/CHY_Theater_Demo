using System;
using System.Collections.Generic;

namespace CHY_Theater_Models.Models;

public partial class Twin
{
    public int UserId { get; set; }

    public int InviteeId { get; set; }

    public int ShowId { get; set; }

    public int TwinupState { get; set; }

    public virtual User Invitee { get; set; } = null!;

    public virtual Show Show { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
