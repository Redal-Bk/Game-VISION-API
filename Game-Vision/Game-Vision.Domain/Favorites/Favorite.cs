using System;
using System.Collections.Generic;

namespace Game_Vision.Models;

public partial class Favorite
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int GameId { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Game Game { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
