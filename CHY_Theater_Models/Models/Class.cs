using System;
using System.Collections.Generic;

namespace CHY_Theater_Models.Models;

public partial class Class
{
    public int ClassId { get; set; }

    public string ClassName { get; set; } = null!;

    public virtual ICollection<MovieClass> MovieClasses { get; set; } = new List<MovieClass>();
}
