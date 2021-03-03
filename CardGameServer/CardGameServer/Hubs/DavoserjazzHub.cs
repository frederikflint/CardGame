using System.Linq;
using System.Security.Cryptography.Xml;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CardGameServer.Dtos;
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

        public async Task EnterGameAsSpectator(string roomId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, roomId);

            await Clients.Caller.SendAsync("RoundInformation", _davoserjazzGameService.GetRoundInformation(roomId));
        }

        public async Task EnterGameAsPlayer(string guid, string roomId)
        {
            var gameUser = _davoserjazzGameService.GetPlayerInformation(roomId, guid);
            await Groups.AddToGroupAsync(Context.ConnectionId, roomId);

            _davoserjazzGameService.AddUserConnection(roomId, guid, Context.ConnectionId);

            await Clients.Caller.SendAsync("PlayerInformation", gameUser);
            await Clients.Group(roomId)
                .SendAsync("RoundInformation", _davoserjazzGameService.GetRoundInformation(roomId));
        }

        public async Task EnterGameAsPlayerWithGuid(string guid, string roomId)
        {
            var user = _davoserjazzGameService.GetPlayerInformation(roomId, guid);

            if (user == null)
            {
                return;
            }

            await Clients.Caller.SendAsync("PlayerInformation", user);

            await Groups.AddToGroupAsync(Context.ConnectionId, roomId);
            await Clients.Group(roomId)
                .SendAsync("RoundInformation", _davoserjazzGameService.GetRoundInformation(roomId));
        }

        public async Task UserTakeTurn(string roomId, string playerGuid, Suit suit, Number number)
        {
            var gameState = _davoserjazzGameService.HandlePlayerTurn(roomId, playerGuid, suit, number);

            await Clients.Group(roomId)
                .SendAsync("RoundInformation", _davoserjazzGameService.GetRoundInformation(roomId));

            foreach (var user in gameState.Users)
            {
                await Clients.Client(user.User.ConnectionId).SendAsync("PlayerInformation",
                    _davoserjazzGameService.GetPlayerInformation(roomId, user.User.Guid));
            }
        }
    }
}