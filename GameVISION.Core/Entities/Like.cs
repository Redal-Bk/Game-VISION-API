
namespace GameVISION.Core.Entities
{
    public class Like
    {
        public int LikeId { get; set; }
        public int UserId { get; set; }
        public int PostId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // روابط
        public User User { get; set; } = null!;
        public Post Post { get; set; } = null!;
    }
}
