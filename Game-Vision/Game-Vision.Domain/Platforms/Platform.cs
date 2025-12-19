

namespace Game_Vision.Domain;

public partial class Platform
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Slug { get; set; } = null!;

    public string? IconUrl { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<Game> Games { get; set; } = new List<Game>();
    public virtual ICollection<GamePlatform> GamePlatforms { get; set; } = new List<GamePlatform>();
}
