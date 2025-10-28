namespace GameVISION.Core.Entities
{
    public class Game
    {
        public int GameId { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public string? Genre { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public string? CoverImage { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // روابط
        public ICollection<Post> Posts { get; set; } = new List<Post>();
        public ICollection<GameRating> Ratings { get; set; } = new List<GameRating>();
        public ICollection<GameTagRelation> GameTags { get; set; } = new List<GameTagRelation>();
    }
}
