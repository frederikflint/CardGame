using System.Collections.Generic;
using CardGameServer.Entities;

namespace CardGameServer.Dtos
{
    public class RoundInformation
    {
        public string ActivePlayerGuid { get; set; }
        public Suit SuitToMatch { get; set; }
        
        public RoundTypeEnum Round { get; set; }
        public List<GameUserInfo> UserInformation { get; set; }
    }
}