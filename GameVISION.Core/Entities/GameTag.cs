using System.ComponentModel.DataAnnotations;

namespace GameVISION.Core.Entities
{
    public class GameTag
    {
        [Key]
        public int TagId { get; set; }
        public string? TagName { get; set; } = null!;

        public ICollection<GameTagRelation>? GameTagRelations { get; set; } = new List<GameTagRelation>();
    }
}
