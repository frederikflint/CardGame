using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CardGameServer.Dtos;
using CardGameServer.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CardGameServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RoomController : ControllerBase
    {
        private readonly IRoomService _roomService;
        private readonly ILogger<RoomController> _logger;

        public RoomController(ILogger<RoomController> logger, IRoomService roomService)
        {
            _logger = logger;
            _roomService = roomService;
        }

        [HttpGet]
        public IEnumerable<string> Get()
        {
            return _roomService.GetRooms();
        }

        [HttpPost]
        public string CreateRoom([FromBody] CreateRoomDto createRoom)
        {
            return _roomService.AddRoom(createRoom.RoomId);
        }
    }
}