using System;
using System.Collections.Generic;
using System.Linq;
using CardGameServer.Entities;

namespace CardGameServer.Services
{
    public class DavoserjazzGameService : GameService
    {
        private readonly Dictionary<string, GameState> _roomToGameStates;

        public DavoserjazzGameService()
        {
            _roomToGameStates = new Dictionary<string, GameState>();
        }

        public override void InitializeGame(string roomId, List<User> users)
        {
            List<GameUser> gameUsers = users.Select(user => new GameUser() {User = user}).ToList();

            foreach (var user in gameUsers)
            {
                user.Hand = new List<Card>();
                user.Table = new List<Card>();
            }

            var deck = GenerateDeck(CalculateAmountOfJokers(gameUsers.Count));
            deck = ShuffleCards(deck);

            DealCards(deck, gameUsers);

            var gameState = new GameState();
            gameState.Deck = deck;
            gameState.Round = 0;
            gameState.Users = gameUsers;

            _roomToGameStates.Add(roomId, gameState);
        }

        public List<Card> GetPlayerCards(string roomId, string guid)
        {
            if (!_roomToGameStates.ContainsKey(roomId))
            {
                return null;
            }

            var gameState = _roomToGameStates[roomId];

            var gameUser = gameState.Users.Find(user => user.User.Guid == guid);
            if (gameUser == null)
            {
                return null;
            }

            return gameUser.Hand;
        }

        // DavozerJazz has different amount of jokers based on player count
        public int CalculateAmountOfJokers(int playerCount)
        {
            switch (playerCount)
            {
                case 3:
                case 6:
                    return 2;
                case 5:
                    return 3;
                case 4:
                case 7:
                    return 4;
                default:
                    return -1;
            }
        }
    }
}