using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CardGameServer.Dtos;
using CardGameServer.Hubs;
using CardGameServer.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace CardGameServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RoomController : ControllerBase
    {
        private readonly IRoomService _roomService;
        private readonly DavoserjazzGameService _davoserjazzGameService;
        private readonly ILogger<RoomController> _logger;

        public RoomController(ILogger<RoomController> logger, IRoomService roomService, DavoserjazzGameService  davoserjazzGameService)
        {
            _logger = logger;
            _roomService = roomService;
            _davoserjazzGameService = davoserjazzGameService;
        }

        [HttpGet]
        public IEnumerable<string> Get()
        {
            return _roomService.GetRooms();
        }

        [HttpGet("find")]
        public IActionResult GetRoomFromGuid([FromQuery(Name = "guid")] string guid)
        {
             var room = _roomService.GetRoomFromUserGuid(guid);
             var gameRoom = _davoserjazzGameService.FindRoomContainingUser(guid);

             if (room == null && gameRoom == null)
             {
                 return NotFound("Not Found");
             }

             return Ok(room ?? gameRoom);
        }
    }
}