using System;
using System.Collections.Generic;

namespace Game_Vision.Models;

public partial class Genre
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Slug { get; set; } = null!;

    public string? Description { get; set; }

    public string? IconUrl { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<Game> Games { get; set; } = new List<Game>();
}
