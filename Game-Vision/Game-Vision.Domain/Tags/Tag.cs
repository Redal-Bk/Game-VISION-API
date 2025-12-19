using System;
using System.Collections.Generic;

namespace Game_Vision.Models;

public partial class Tag
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Slug { get; set; } = null!;

    public string? Color { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<Game> Games { get; set; } = new List<Game>();
}
