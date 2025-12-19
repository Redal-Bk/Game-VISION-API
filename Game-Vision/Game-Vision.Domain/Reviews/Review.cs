using System;
using System.Collections.Generic;

namespace Game_Vision.Models;

public partial class Review
{
    public int Id { get; set; }

    public int GameId { get; set; }

    public int UserId { get; set; }

    public byte Rating { get; set; }

    public string Title { get; set; } = null!;

    public string Content { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool IsApproved { get; set; }

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual Game Game { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
