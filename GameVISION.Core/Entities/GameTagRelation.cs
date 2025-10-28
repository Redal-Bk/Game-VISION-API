namespace GameVISION.Core.Entities
{
    public class GameTagRelation
    {
        public int GameId { get; set; }
        public int TagId { get; set; }

        public Game Game { get; set; } = null!;
        public GameTag Tag { get; set; } = null!;
    }
}
