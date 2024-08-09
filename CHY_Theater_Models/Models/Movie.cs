using System;
using System.Collections.Generic;

namespace CHY_Theater_Models.Models;

public partial class Movie
{
    public int MovieId { get; set; }

    public string MovieName { get; set; } = null!;

    public string MovieEnglishName { get; set; } = null!;

    public string MovieDescription { get; set; } = null!;

    public DateOnly ReleaseDate { get; set; }

    public int Runtime { get; set; }

    public int Level { get; set; }

    public string Language { get; set; } = null!;

    public string? MovieImage { get; set; }

    public string? Movievideo { get; set; }

    public string DirectorName { get; set; } = null!;

 
    public int? MovieState { get; set; }
    public int? OnFous { get; set; }
    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual ICollection<MovieActor> MovieActors { get; set; } = new List<MovieActor>();

    public virtual ICollection<MovieClass> MovieClasses { get; set; } = new List<MovieClass>();

    public virtual ICollection<Rating> Ratings { get; set; } = new List<Rating>();

    public virtual ICollection<Show> Shows { get; set; } = new List<Show>();
}
