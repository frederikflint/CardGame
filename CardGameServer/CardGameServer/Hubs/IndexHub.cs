using System.Collections.Generic;
using System.Threading.Tasks;
using CardGameServer.Entities;
using CardGameServer.Services;
using Microsoft.AspNetCore.SignalR;

namespace CardGameServer.Hubs
{
    public class IndexHub : Hub
    {
        private readonly IRoomService _roomService;

        public IndexHub(IRoomService roomService)
        {
            _roomService = roomService;
        }

        public async Task CreateRoom(string roomId)
        {
            _roomService.AddRoom(roomId);

            await Clients.All.SendAsync("RoomUpdated", roomId);
        }
    }
}