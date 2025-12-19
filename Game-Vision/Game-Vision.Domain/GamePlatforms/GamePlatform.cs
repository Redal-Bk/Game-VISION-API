using Game_Vision.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Game_Vision.Domain.GamePlatforms
{
    
    public class GamePlatform
    {
        public int GameId { get; set; }
        public int PlatformId { get; set; }

        public Game Game { get; set; } = null!;
        public Platform Platform { get; set; } = null!;
    }
}
