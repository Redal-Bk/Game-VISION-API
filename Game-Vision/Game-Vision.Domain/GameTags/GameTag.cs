
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Game_Vision.Domain;


public class GameTag
{

    public int GameId { get; set; }

    public int TagId { get; set; }

    public Game Game { get; set; } = null!;
    public Tag Tag { get; set; } = null!;
}
