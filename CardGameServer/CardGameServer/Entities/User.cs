using System.Collections.Generic;

namespace CardGameServer.Entities
{
    public class User
    {
        public string Username { get; set; }
        public string Guid { get; set; }
        public bool Admin { get; set; }
    }

    public class GameUser
    {
        public User User { get; set; }
        public int Score { get; set; }
        public List<Card> Hand { get; set; }
        public List<Card> Table { get; set; }
        public bool YourTurn { get; set; }
        public bool IsDealer { get; set; }
        
    }

    public class DavoserJazzGameUserUser : GameUser
    {
        public Card ActiveCard { get; set; }
    }
}