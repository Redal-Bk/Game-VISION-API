using Game_Vision.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Game_Vision.Domain.GameTags
{
   
    public class GameTag
    {
        public int GameId { get; set; }
        public int TagId { get; set; }

        public Game Game { get; set; } = null!;
        public Tag Tag { get; set; } = null!;
    }
}
