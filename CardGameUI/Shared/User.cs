using System.Collections.Generic;

namespace CardGameUI.Shared
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
        public List<CardClass> Hand { get; set; }
        public List<CardClass> Table { get; set; }
        public bool YourTurn { get; set; }
        public bool IsDealer { get; set; }
        
    }

    public class DavoserJazzGameUser : GameUser
    {
        public CardClass ActiveCard { get; set; }
    }
}