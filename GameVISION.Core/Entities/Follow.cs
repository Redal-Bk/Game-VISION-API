namespace GameVISION.Core.Entities
{
    public class Follow
    {
        public int FollowId { get; set; }
        public int FollowerId { get; set; }
        public int FollowingId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // روابط
        public User Follower { get; set; } = null!;
        public User Following { get; set; } = null!;
    }
}
