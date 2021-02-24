using System;
using System.Collections.Generic;
using System.Linq;
using CardGameServer.Entities;

namespace CardGameServer.Services
{
    public class DavoserjazzGameService : GameService
    {
        private readonly Dictionary<string, DavoserJazzGameState> _roomToGameStates;

        public DavoserjazzGameService()
        {
            _roomToGameStates = new Dictionary<string, DavoserJazzGameState>();
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

            var davoserJazzGameUsers = gameUsers.Select(gUser => new DavoserJazzGameUser
            {
                Hand = gUser.Hand,
                User = gUser.User,
                Score = 0,
                Table = new List<Card>(),
                ActiveCard = null
            }).OrderBy(_ => Guid.NewGuid()).ToList();

            var gameState = new DavoserJazzGameState();
            gameState.Deck = deck;
            gameState.Round = 0;
            gameState.Users = davoserJazzGameUsers;
            gameState.RoundType = 0;
            gameState.Dealer = gameState.Users[0];
            gameState.Users[0].IsDealer = true;
            
            gameState.ActiveUser = gameState.Users[1];
            gameState.Users[1].YourTurn = true;

            _roomToGameStates.Add(roomId, gameState);
        }

        public string GetGuidOfActiveUser(string roomId)
        {
            return _roomToGameStates[roomId]?.ActiveUser.User.Guid;
        }

        public DavoserJazzGameUser HandlePlayerTurn(string roomId, string playerGuid, Suit suit, Number number)
        {
            var gameState = _roomToGameStates[roomId];
            var playerIsEldestHand = false;

            // If game state doesn't exist, throw error
            if (gameState == null)
            {
                throw new Exception("Room doesn't contain a game state for DavoserJazz");
            }
            
            var user = gameState.Users.Find(u => u.User.Guid == playerGuid);

            // If user doesn't exist in game room, throw error
            if (user == null)
            {
                throw new Exception($"Player with guid: {playerGuid} doesn't exist in game room with id: {roomId}");
            }

            // Make sure that it is the users turn, otherwise throw error
            if (user != gameState.ActiveUser)
            {
                throw new Exception(
                    $"Player with guid {playerGuid} is not the active player, and must wait for it their turn");
            }
            
            // If user is right after dealer, this user is eldest hand
            if (gameState.Users[1].User.Guid == user.User.Guid)
            {
                playerIsEldestHand = true;
            }
            
            // Make sure that the user is in possession of the card wished to play, otherwise throw error
            if (!user.Hand.Any(card => card.Suit == suit && card.Number == number))
            {
                throw new Exception(
                    $"Player tries to play card {number.ToString()} of {suit.ToString()}, but is not in possession of the card");
            }

            // User wishes to play a card not matching the first played suit of the round
            if (!playerIsEldestHand && suit != gameState.SuitToMatch)
            {
                // This is only allowed if user doesn't have a card matching that suit, otherwise throw error
                if (user.Hand.Any(card => card.Suit == gameState.SuitToMatch))
                {
                    throw new Exception(
                        $"Player trying to play a card of suit: {suit} when the suit firstly played this round was {gameState.SuitToMatch}");
                }
            }

            // If player is first to play a card, set the SuitToMatch for the rest of the round
            if (playerIsEldestHand)
            {
                gameState.SuitToMatch = suit;
            }

            // Set active card of a user & remove card from hand
            user.ActiveCard = user.Hand.Find(c => c.Suit == suit && c.Number == number);
            user.Hand.Remove(user.ActiveCard);
            user.YourTurn = false;
            
            // Set next players turn
            var indexOfNextPlayer = (gameState.Users.IndexOf(user) + 1) % gameState.Users.Count;
            gameState.Users[indexOfNextPlayer].YourTurn = true;
            gameState.ActiveUser = gameState.Users[indexOfNextPlayer];

            return user;
        }

        public DavoserJazzGameUser GetPlayerInformation(string roomId, string playerGuid)
        {
            if (!_roomToGameStates.ContainsKey(roomId))
            {
                return null;
            }

            var gameState = _roomToGameStates[roomId];

            var gameUser = gameState.Users.Find(user => user.User.Guid == playerGuid);

            // Return users hand if user exists. Otherwise returns null
            return gameUser;
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