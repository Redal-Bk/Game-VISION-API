using System;
using System.Collections.Generic;

namespace Game_Vision.Models;

public partial class SystemRequirement
{
    public int Id { get; set; }

    public int GameId { get; set; }

    public string? MinOs { get; set; }

    public string? MinCpu { get; set; }

    public int? MinRam { get; set; }

    public string? MinGpu { get; set; }

    public string? MinDirectX { get; set; }

    public int? MinStorage { get; set; }

    public string? RecOs { get; set; }

    public string? RecCpu { get; set; }

    public int? RecRam { get; set; }

    public string? RecGpu { get; set; }

    public string? RecDirectX { get; set; }

    public int? RecStorage { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Game Game { get; set; } = null!;
}
