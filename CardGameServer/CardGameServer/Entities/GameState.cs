using System.Collections.Generic;

namespace CardGameServer.Entities
{
    public class GameState
    {
        public List<GameUser> Users { get; set; }
        public List<Card> Deck { get; set; }
        public int Round { get; set; }
    }
}