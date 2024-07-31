using System;
using System.Collections.Generic;

namespace CHY_Theater_Models.Models;

public partial class User
{
    public int UserId { get; set; }

    public string UserName { get; set; } = null!;

    public DateOnly Birthday { get; set; }

    public string Phone { get; set; } = null!;

    public string Sex { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Address { get; set; } = null!;

    public string? Mbti { get; set; }

    public int? Points { get; set; }

    public bool? EmailConfirm { get; set; }

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual ICollection<Contact> Contacts { get; set; } = new List<Contact>();

    public virtual ICollection<Rating> Ratings { get; set; } = new List<Rating>();

    public virtual ICollection<Twin> TwinInvitees { get; set; } = new List<Twin>();

    public virtual ICollection<Twin> TwinUsers { get; set; } = new List<Twin>();

    public virtual ICollection<UserHabbit> UserHabbits { get; set; } = new List<UserHabbit>();

    public virtual ICollection<UserPassword> UserPasswords { get; set; } = new List<UserPassword>();
}
