

namespace Game_Vision.Domain
{
    // 1. GameGenre (واسط بازی و ژانر)
    public class GameGenre
    {
        public int GameId { get; set; }
        public int GenreId { get; set; }

        // Navigation properties
        public Game Game { get; set; } = null!;
        public Genre Genre { get; set; } = null!;
    }
}
