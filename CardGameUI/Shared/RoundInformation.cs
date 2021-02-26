using System.Collections.Generic;

namespace CardGameUI.Shared
{
    public class RoundInformation
    {
        public string ActivePlayerGuid { get; set; }
        public Suit SuitToMatch { get; set; }

        public List<GameUserInfo> UserInformation { get; set; }

        public RoundTypeEnum Round { get; set; }
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