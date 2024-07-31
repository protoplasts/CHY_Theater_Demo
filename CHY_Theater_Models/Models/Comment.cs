using System;
using System.Collections.Generic;

namespace CHY_Theater_Models.Models;

public partial class Comment
{
    public int CommentsId { get; set; }

    public int MovieId { get; set; }

    public int UserId { get; set; }

    public string CommentMessage { get; set; } = null!;

    public DateTime? CommentTime { get; set; }

    public virtual Movie Movie { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
