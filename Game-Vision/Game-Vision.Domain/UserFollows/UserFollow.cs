namespace Game_Vision.Domain;

public partial class UserFollow
{
    public int FollowerId { get; set; }

    public int FollowedId { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual User Followed { get; set; } = null!;

    public virtual User Follower { get; set; } = null!;
}
