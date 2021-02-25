using System;
using System.Collections.Generic;
using System.Linq;
using CardGameServer.Dtos;
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

            gameState.IndexOfFirstPlayerInRound = 1;

            _roomToGameStates.Add(roomId, gameState);
        }

        public RoundInformation GetRoundInformation(string roomId)
        {
            var gameState = _roomToGameStates[roomId];

            var roundInformation = new RoundInformation()
            {
                ActivePlayerGuid = gameState.ActiveUser.User.Guid,
                SuitToMatch = gameState.SuitToMatch,
            };
            
            return roundInformation;
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
            if (gameState.Users[gameState.IndexOfFirstPlayerInRound].User.Guid == user.User.Guid)
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
            if (!playerIsEldestHand && gameState.SuitToMatch != 0 && suit != gameState.SuitToMatch && number != Number.JOKER)
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
            
            // If everyone played a card, wrap up this round
            if (gameState.Users.TrueForAll(gU => gU.ActiveCard != null))
            {
                WrapUpTurn(gameState);
            }

            return user;
        }

        private void WrapUpTurn(DavoserJazzGameState gameState)
        {
            // Find user who won this round.
            DavoserJazzGameUser user = gameState.Users[gameState.IndexOfFirstPlayerInRound];
            // If user did not play joker initially, calculate winner of the round
            if (gameState.SuitToMatch != 0)
            {
                Card lastUsersCard = gameState.Users[gameState.IndexOfFirstPlayerInRound].ActiveCard;
                for (int i = gameState.IndexOfFirstPlayerInRound + 1;
                    i < gameState.Users.Count + gameState.IndexOfFirstPlayerInRound;
                    i++)
                {
                    var aceIsHighest = DavoserJazzGameState.AceHighestByRoundType[gameState.RoundType];
                
                    var currentUsersCard = gameState.Users[i % gameState.Users.Count].ActiveCard; 
                    // If current player beats currently played card
                    if ((int) currentUsersCard.Number > (int) lastUsersCard.Number)
                    {
                        // If player who did not play first card, played a joker, skip this player
                        if (currentUsersCard.Number == Number.JOKER)
                        {
                            continue;
                        }

                        
                        // If current users card doesn't match the suit of this round, skip this player
                        if (currentUsersCard.Suit != gameState.SuitToMatch)
                        {
                            continue;
                        }

                        // Ace is considered as number 13. Therefore making it programatically larger than any other thing.
                        // If ace should be considered as the lowest card, skip this player
                        if (currentUsersCard.Number == Number.ACE && !aceIsHighest)
                        {
                            continue;
                        }

                        user = gameState.Users[i % gameState.Users.Count];
                        lastUsersCard = user.ActiveCard;
                    }
                }
            }

            var cardsThisRound = gameState.Users.Select(u => u.ActiveCard).ToList();
            user.Table.AddRange(cardsThisRound);

            CalculateScoreForUser(user, cardsThisRound, gameState);

            gameState.Users.ForEach(u =>
            {
                u.ActiveCard = null;
                u.YourTurn = false;
            });

            gameState.SuitToMatch = 0;
            gameState.IndexOfFirstPlayerInRound = gameState.Users.IndexOf(user);

            gameState.ActiveUser = user;
            user.YourTurn = true;

        }

        private void CalculateScoreForUser(DavoserJazzGameUser user, List<Card> cards, DavoserJazzGameState gameState)
        {
            int score = 0;
            switch (gameState.RoundType)
            {
                case RoundTypeEnum.AVOID_CLUB:
                    cards.ForEach(c =>
                    {
                        if (c.Suit == Suit.CLUB)
                        {
                            score += 10;
                        }
                    });
                    break;
                case RoundTypeEnum.AVOID_TAKE_A_TRICK:
                    score += 10;
                    break;
                case RoundTypeEnum.AVOID_FIVES_AND_LADIES:
                    cards.ForEach(c =>
                    {
                        if (c.Number == Number.FIVE ||c.Number == Number.QUEEN)
                        {
                            score += 20;
                        }
                    });
                    break;
                case RoundTypeEnum.AVOID_BLACK_KINGS:
                    cards.ForEach(c =>
                    {
                        if (c.Number == Number.KING && (c.Suit == Suit.CLUB || c.Suit == Suit.SPADE))
                        {
                            score += 40;
                        }
                    });
                    break;
                case RoundTypeEnum.AVOID_FIRST_AND_LAST_TRICK:
                    break;
                case RoundTypeEnum.GET_MOST_TRICKS:
                    break;
                case RoundTypeEnum.AVOID_ACES:
                    break;
                default:
                    break;
            }

            user.Score += score;
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