namespace Game_Vision.Domain;

public partial class Game
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string Slug { get; set; } = null!;

    public string? Description { get; set; }

    public DateOnly? ReleaseDate { get; set; }

    public string? CoverImageUrl { get; set; }

    public string? BannerImageUrl { get; set; }

    public string? TrailerUrl { get; set; }

    public int? PublisherId { get; set; }

    public int? DeveloperId { get; set; }

    public decimal? AverageRating { get; set; }

    public int? TotalReviews { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool IsFeatured { get; set; }

    public virtual Developer? Developer { get; set; }

    public virtual ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();

    public virtual ICollection<GameImage> GameImages { get; set; } = new List<GameImage>();

    public virtual ICollection<PccompatibilityResult> PccompatibilityResults { get; set; } = new List<PccompatibilityResult>();

    public virtual Publisher? Publisher { get; set; }

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    public virtual SystemRequirement? SystemRequirement { get; set; }

    public virtual ICollection<Genre> Genres { get; set; } = new List<Genre>();

    public virtual ICollection<Platform> Platforms { get; set; } = new List<Platform>();

    public virtual ICollection<Tag> Tags { get; set; } = new List<Tag>();
    public virtual ICollection<GameGenre> GameGenres { get; set; } = new List<GameGenre>();
    public virtual ICollection<GamePlatform> GamePlatforms { get; set; } = new List<GamePlatform>();
    public virtual ICollection<GameTag> GameTags { get; set; } = new List<GameTag>();

}
