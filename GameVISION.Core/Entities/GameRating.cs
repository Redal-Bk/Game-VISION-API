namespace GameVISION.Core.Entities
{
    public class GameRating
    {
        public int RatingId { get; set; }
        public int UserId { get; set; }
        public int GameId { get; set; }
        public int Rating { get; set; }
        public string? Review { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // روابط
        public User User { get; set; } = null!;
        public Game Game { get; set; } = null!;
    }
}
