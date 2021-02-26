using System;
using System.Security.Cryptography.Xml;
using System.Threading.Tasks;
using CardGameServer.Services;
using Microsoft.AspNetCore.SignalR;
using Hub = Microsoft.AspNetCore.SignalR.Hub;

namespace CardGameServer.Hubs
{
    public class LobbyHub : Hub
    {
        private readonly IMessageService _messageService;
        private readonly IRoomService _roomService;
        private readonly DavoserjazzGameService _davoserjazzGameService;

        public LobbyHub(IMessageService messageService, IRoomService roomService,
            DavoserjazzGameService davoserjazzGameService)
        {
            _messageService = messageService;
            _roomService = roomService;
            _davoserjazzGameService = davoserjazzGameService;
        }

        public override async Task OnConnectedAsync()
        {
            await Clients.Caller.SendAsync("ActiveGameRooms", _davoserjazzGameService.GetActiveGameRooms());
        }

        public async Task SendMessage(string user, string message, string roomId)
        {
            _messageService.AddMessage(user, message, roomId);
            await Clients.Group(roomId).SendAsync("ReceiveMessage", user, message);
        }

        public async Task LeaveLobby(string guid)
        {
            if (_roomService.RoomExists(_roomService.GetRoomFromUserGuid(guid)))
            {
                await Clients.Group(_roomService.GetRoomFromUserGuid(guid))
                    .SendAsync("UserRemoved", _roomService.GetUserFromUserGuid(guid));
            }

            _roomService.CleanupUser(guid);
        }

        public async Task JoinRoomWithGuid(string guid, string roomId)
        {
            var user = _roomService.GetUserFromUserGuid(guid);

            await Clients.Caller.SendAsync("SessionInformation", user);

            if (user == null)
            {
                return;
            }

            await Groups.AddToGroupAsync(Context.ConnectionId, roomId);

            foreach (var message in _messageService.GetMessages(roomId))
            {
                await Clients.Caller.SendAsync("ReceiveMessage", message.User, message.Text);
            }

            foreach (var userInRoom in _roomService.GetUsersInRoom(roomId))
            {
                await Clients.Caller.SendAsync("UserAdded", userInRoom.Username);
            }
        }

        public async Task JoinRoom(string username, string roomId)
        {
            if (!_roomService.RoomExists(roomId))
            {
                await Clients.Caller.SendAsync("SessionInformation", null);
                return;
            }

            await Groups.AddToGroupAsync(Context.ConnectionId, roomId);

            var user = _roomService.AddUserToRoom(username, roomId);

            await Clients.Caller.SendAsync("SessionInformation", user);

            foreach (var message in _messageService.GetMessages(roomId))
            {
                await Clients.Caller.SendAsync("ReceiveMessage", message.User, message.Text);
            }

            foreach (var userInRoom in _roomService.GetUsersInRoom(roomId))
            {
                await Clients.Caller.SendAsync("UserAdded", userInRoom.Username);
            }

            await Clients.AllExcept(Context.ConnectionId).SendAsync("UserAdded", user.Username);
        }

        public async Task StartGame(string guid, string roomId)
        {
            var user = _roomService.GetUserFromUserGuid(guid);

            if (!user.Admin)
            {
                return;
            }

            // DavoserJazz requires 3 to 7 players
            var playerCount = _roomService.GetUsersInRoom(roomId).Count;
            if (playerCount < 3 || 7 < playerCount)
            {
                return;
            }

            _davoserjazzGameService.InitializeGame(roomId, _roomService.GetUsersInRoom(roomId));

            await Clients.Group(roomId).SendAsync("GameStarted");
            
            _roomService.CleanUpRoom(roomId);

            await Clients.All.SendAsync("ActiveGameRooms", _davoserjazzGameService.GetActiveGameRooms());
        }
    }
}