using System.Collections.Generic;

namespace CardGameServer.Entities
{
    public class GameState
    {
        public List<GameUser> Users { get; set; }
        public GameUser ActiveUser { get; set; }
        public GameUser Dealer { get; set; }
        public List<Card> Deck { get; set; }
        public int Round { get; set; }
    }

    public class DavoserJazzGameState : GameState
    {
        public RoundTypeEnum RoundType { get; set; }
        
        public Suit SuitToMatch { get; set; }
        
        public int IndexOfFirstPlayerInRound { get; set; }

        public static Dictionary<RoundTypeEnum, bool> AceHighestByRoundType = new Dictionary<RoundTypeEnum, bool>()
        {
            {RoundTypeEnum.AVOID_CLUB, false},
            {RoundTypeEnum.AVOID_TAKE_A_TRICK, false},
            {RoundTypeEnum.AVOID_FIVES_AND_LADIES, true},
            {RoundTypeEnum.AVOID_BLACK_KINGS, true},
            {RoundTypeEnum.AVOID_FIRST_AND_LAST_TRICK, false},
            {RoundTypeEnum.GET_MOST_TRICKS, true},
            {RoundTypeEnum.AVOID_ACES, true}
        };
        
        
        // TODO: Determine whether this is needed or not
        public static Dictionary<RoundTypeEnum, bool> ScoreIncrementByRoundType = new Dictionary<RoundTypeEnum, bool>()
        {
            {RoundTypeEnum.AVOID_CLUB, true},
            {RoundTypeEnum.AVOID_TAKE_A_TRICK, true},
            {RoundTypeEnum.AVOID_FIVES_AND_LADIES, true},
            {RoundTypeEnum.AVOID_BLACK_KINGS, true},
            {RoundTypeEnum.AVOID_FIRST_AND_LAST_TRICK, true},
            {RoundTypeEnum.GET_MOST_TRICKS, false},
            {RoundTypeEnum.AVOID_ACES, true}
        };
    }

    public enum RoundTypeEnum
    {
        AVOID_CLUB = 0,
        AVOID_TAKE_A_TRICK = 1,
        AVOID_FIVES_AND_LADIES = 2,
        AVOID_BLACK_KINGS = 3,
        AVOID_FIRST_AND_LAST_TRICK = 4,
        GET_MOST_TRICKS = 5,
        AVOID_ACES = 6
    }
}