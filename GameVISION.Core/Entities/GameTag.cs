namespace GameVISION.Core.Entities
{
    public class GameTag
    {
        public int TagId { get; set; }
        public string TagName { get; set; } = null!;

        public ICollection<GameTagRelation> GameTagRelations { get; set; } = new List<GameTagRelation>();
    }
}
