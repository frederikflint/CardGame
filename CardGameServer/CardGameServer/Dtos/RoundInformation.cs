using CardGameServer.Entities;

namespace CardGameServer.Dtos
{
    public class RoundInformation
    {
        public string ActivePlayerGuid { get; set; }
        public Suit SuitToMatch { get; set; }
    }
}