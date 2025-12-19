using System.ComponentModel.DataAnnotations;

namespace GameVISION.Core.Entities
{
    public class Post
    {
        [Key]
        public int PostId { get; set; }
        public int? UserId { get; set; }
        public int? GameId { get; set; }
        public string? Content { get; set; } = null!;
        public string? ImageUrl { get; set; }
        public DateTime? CreatedAt { get; set; } = DateTime.Now;

        // روابط
        public User? User { get; set; } = null!;
        public Game? Game { get; set; }
        public ICollection<Comment>? Comments { get; set; } = new List<Comment>();
        public ICollection<Like>? Likes { get; set; } = new List<Like>();
    }
}
