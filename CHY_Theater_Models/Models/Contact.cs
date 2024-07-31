using System;
using System.Collections.Generic;

namespace CHY_Theater_Models.Models;

public partial class Contact
{
    public int ContactId { get; set; }

    public int UserId { get; set; }

    public string IssueType { get; set; } = null!;

    public string ContactDescription { get; set; } = null!;

    public string ContactStatus { get; set; } = null!;

    public DateTime CreateAt { get; set; }

    public DateTime? UpdateAt { get; set; }

    public virtual User User { get; set; } = null!;
}
