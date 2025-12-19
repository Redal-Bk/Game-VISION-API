namespace Game_Vision.Domain;

public partial class PccompatibilityResult
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int GameId { get; set; }

    public bool CanRunMinimum { get; set; }

    public bool CanRunRecommended { get; set; }

    public string? Bottleneck { get; set; }

    public decimal? Score { get; set; }

    public DateTime CheckedAt { get; set; }

    public virtual Game Game { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
