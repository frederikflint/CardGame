using System.Linq;
using System.Security.Cryptography.Xml;
using System.Threading.Tasks;
using CardGameServer.Entities;
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
            var gameUser = _davoserjazzGameService.GetPlayerInformation(roomId, guid);
            await Groups.AddToGroupAsync(Context.ConnectionId, roomId);

            await Clients.Caller.SendAsync("PlayerInformation", gameUser);
        }

        public async Task UserTakeTurn(string roomId, string playerGuid, Suit suit, Number number)
        {
            var gameUser = _davoserjazzGameService.HandlePlayerTurn(roomId, playerGuid, suit, number);

            await Clients.Caller.SendAsync("PlayerInformation", gameUser);
            await Clients.Group(roomId).SendAsync("ActivePlayer", _davoserjazzGameService.GetGuidOfActiveUser(roomId));
        }
    }
}