
namespace Game_Vision.Domain;

public partial class Comment
{
    public int Id { get; set; }

    public int? ParentId { get; set; }

    public int? NewsId { get; set; }

    public int? ReviewId { get; set; }

    public int UserId { get; set; }

    public string Content { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public bool IsApproved { get; set; }

    public virtual ICollection<Comment> InverseParent { get; set; } = new List<Comment>();

    public virtual News? News { get; set; }

    public virtual Comment? Parent { get; set; }

    public virtual Review? Review { get; set; }

    public virtual User User { get; set; } = null!;
}
