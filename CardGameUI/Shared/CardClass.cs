namespace CardGameUI.Shared
{
    public class CardClass
    {
        public Suit Suit { get; set; }
        public Number Number { get; set; }
    }

    public enum Suit
    {
        DIAMOND = 1,
        CLUB = 2,
        HEART = 3,
        SPADE = 4,
    }

    public enum Number
    {
        ACE = 1,
        TWO = 2,
        THREE = 3,
        FOUR = 4,
        FIVE = 5,
        SIX = 6,
        SEVEN = 7,
        EIGHT = 8,
        NINE = 9,
        TEN = 10,
        JACK = 11,
        QUEEN = 12,
        KING = 13,
        JOKER = 14
    }
}