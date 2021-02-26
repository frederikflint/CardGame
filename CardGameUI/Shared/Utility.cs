using System;

namespace CardGameUI.Shared
{
    public class Utility
    {
        public static bool AceHighest(RoundTypeEnum type)
        {
            return type switch
            {
                RoundTypeEnum.AVOID_CLUB => false,
                RoundTypeEnum.AVOID_TAKE_A_TRICK => false,
                RoundTypeEnum.AVOID_FIVES_AND_LADIES => true,
                RoundTypeEnum.AVOID_BLACK_KINGS => true,
                RoundTypeEnum.AVOID_FIRST_AND_LAST_TRICK => false,
                RoundTypeEnum.GET_MOST_TRICKS => true,
                RoundTypeEnum.AVOID_ACES => true,
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };
        }
        public static string GetRoundNameByType(RoundTypeEnum type)
        {
            switch (type)
            {
                case RoundTypeEnum.AVOID_CLUB:
                    return "Avoid clubs";
                case RoundTypeEnum.AVOID_TAKE_A_TRICK:
                    return "Avoid tricks";
                case RoundTypeEnum.AVOID_FIVES_AND_LADIES:
                    return "Avoid fives and ladies";
                case RoundTypeEnum.AVOID_BLACK_KINGS:
                    return "Avoid black kings";
                case RoundTypeEnum.AVOID_FIRST_AND_LAST_TRICK:
                    return "Avoid first and last trick";
                case RoundTypeEnum.GET_MOST_TRICKS:
                    return "Get as many tricks as you can";
                case RoundTypeEnum.AVOID_ACES:
                    return "Avoid aces";
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
        public static string GetFileName(CardClass card)
        {
            var fileName = "/cards/";

            switch (card.Suit)
            {
                case Suit.DIAMOND:
                    fileName += "diamond/";
                    break;
                case Suit.CLUB:
                    fileName += "club/";
                    break;
                case Suit.HEART:
                    fileName += "heart/";
                    break;
                case Suit.SPADE:
                    fileName += "spade/";
                    break;
                default:
                    break;
            }

            switch (card.Number)
            {
                case Number.ACE:
                    fileName += "ace";
                    break;
                case Number.TWO:
                    fileName += "2";
                    break;
                case Number.THREE:
                    fileName += "3";
                    break;
                case Number.FOUR:
                    fileName += "4";
                    break;
                case Number.FIVE:
                    fileName += "5";
                    break;
                case Number.SIX:
                    fileName += "6";
                    break;
                case Number.SEVEN:
                    fileName += "7";
                    break;
                case Number.EIGHT:
                    fileName += "8";
                    break;
                case Number.NINE:
                    fileName += "9";
                    break;
                case Number.TEN:
                    fileName += "10";
                    break;
                case Number.JACK:
                    fileName += "jack";
                    break;
                case Number.QUEEN:
                    fileName += "queen";
                    break;
                case Number.KING:
                    fileName += "king";
                    break;
                case Number.JOKER:
                    fileName += "joker";
                    break;
                default:
                    break;
            }

            return fileName + ".png";
        }
    }
}