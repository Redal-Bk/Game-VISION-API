namespace Game_Vision.Domain;

public partial class UserPcspec
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string? Os { get; set; }

    public string? Cpu { get; set; }

    public int? Ram { get; set; }

    public string? Gpu { get; set; }

    public string? DirectX { get; set; }

    public int? StorageAvailable { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool IsPublic { get; set; }

    public virtual User User { get; set; } = null!;
}
