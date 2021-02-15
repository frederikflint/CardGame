using System.Security.Cryptography.Xml;
using System.Threading.Tasks;
using CardGameServer.Services;
using Microsoft.AspNetCore.SignalR;
using Hub = Microsoft.AspNetCore.SignalR.Hub;

namespace CardGameServer.Hubs
{
    public class DavoserjazzHub : Hub
    {
        private readonly DavoserjazzGameService _davoserjazzGameService;

        public DavoserjazzHub(DavoserjazzGameService davoserjazzGameService)
        {
            _davoserjazzGameService = davoserjazzGameService;
        }

        public async Task EnterGameAsPlayer(string guid, string roomId)
        {
            var playerCards = _davoserjazzGameService.GetPlayerCards(roomId, guid);

            await Clients.Caller.SendAsync("PlayerHand", playerCards);
        }
    }
}