
namespace Game_Vision.Domain;

public partial class GameImage
{
    public int Id { get; set; }

    public int GameId { get; set; }

    public string ImageUrl { get; set; } = null!;

    public string? AltText { get; set; }

    public bool IsCover { get; set; }

    public int DisplayOrder { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Game Game { get; set; } = null!;
}
