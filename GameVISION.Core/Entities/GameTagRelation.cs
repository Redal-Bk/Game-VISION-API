using System.ComponentModel.DataAnnotations;

namespace GameVISION.Core.Entities
{
    public class GameTagRelation
    {
        [Key]
        public int GameId { get; set; }
        public int? TagId { get; set; }

        public Game? Game { get; set; } = null!;
        public GameTag? Tag { get; set; } = null!;
    }
}
