

namespace Game_Vision.Domain;

public partial class Genre
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Slug { get; set; } = null!;

    public string? Description { get; set; }

    public string? IconUrl { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<Game> Games { get; set; } = new List<Game>();
    public virtual ICollection<GameGenre> GameGenres { get; set; } = new List<GameGenre>();
    public virtual ICollection<GamePlatform> GamePlatforms { get; set; } = new List<GamePlatform>();
    public virtual ICollection<GameTag> GameTags { get; set; } = new List<GameTag>();
  
}
