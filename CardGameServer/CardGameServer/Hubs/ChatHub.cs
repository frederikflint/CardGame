using System.Collections.Generic;
using System.Threading.Tasks;
using CardGameServer.Entities;
using CardGameServer.Services;
using Microsoft.AspNetCore.SignalR;

namespace CardGameServer.Hubs
{
    public class ChatHub : Hub
    {
        private readonly IMessageService _messageService;
        private readonly IRoomService _roomService;

        public ChatHub(IMessageService messageService, IRoomService roomService)
        {
            _messageService = messageService;
            _roomService = roomService;
        }

        public async Task SendMessage(string user, string message, string roomId)
        {
            _messageService.AddMessage(user, message, roomId);
            await Clients.Group(roomId).SendAsync("ReceiveMessage", user, message);
        }

        public async Task JoinRoom(string user, string roomId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, roomId);
            foreach (var message in _messageService.GetMessages(roomId))
            {
                await Clients.Caller.SendAsync("ReceiveMessage", message.User, message.Text);
            }
        }
    }
}