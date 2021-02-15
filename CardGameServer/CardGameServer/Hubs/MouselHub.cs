using System.Security.Cryptography.Xml;
using System.Threading.Tasks;
using CardGameServer.Services;
using Microsoft.AspNetCore.SignalR;
using Hub = Microsoft.AspNetCore.SignalR.Hub;

namespace CardGameServer.Hubs
{
    public class MouselHub : Hub
    {
        private readonly MouselGameService _mouselGameService;

        public MouselHub(MouselGameService mouselGameService)
        {
            _mouselGameService = mouselGameService;
        }

        public async Task EnterGameAsPlayer(string guid, string roomId)
        {
            var playerCards = _mouselGameService.GetPlayerCards(roomId, guid);

            // foreach (var playerCard in playerCards)
            // {
            //     await Clients.Caller.SendAsync("PlayerHand", playerCard);
            // }
            await Clients.Caller.SendAsync("PlayerHand", playerCards);
        }
    }
}