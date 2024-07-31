using System;
using System.Collections.Generic;

namespace CHY_Theater_Models.Models;

public partial class UserPassword
{
    public int UserPasswordId { get; set; }

    public int? UserId { get; set; }

    public string Password { get; set; } = null!;

    public bool? Status { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual User? User { get; set; }
}
