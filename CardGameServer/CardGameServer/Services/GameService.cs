using System;
using System.Collections.Generic;
using System.Linq;
using CardGameServer.Entities;

namespace CardGameServer.Services
{
    public abstract class GameService
    {
        public abstract void InitializeGame(string roomId, List<User> users);

        public List<Card> GenerateDeck()
        {
            var cards = new List<Card>();
            for (int i = 1; i < 5; i++)
            {
                for (int j = 1; j < 14; j++)
                {
                    cards.Add(new Card() {Number = (Number)j, Suit = (Suit)i });
                }
            }

            return cards;
        }

        public List<Card> ShuffleCards(List<Card> cards)
        {
            var shuffledCards = cards.OrderBy(card => Guid.NewGuid()).ToList();

            return shuffledCards;
        }

        public void DealCards(List<Card> deck, List<GameUser> users, int amount)
        {
            for (int i = 0; i < users.Count * amount; i++)
            {
                var cardToDeal = deck[0];
                deck.Remove(cardToDeal);
                
                users[i % users.Count].Hand.Add(cardToDeal);
            }
        }
    }
}