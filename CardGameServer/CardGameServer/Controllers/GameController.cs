using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CardGameServer.Dtos;
using CardGameServer.Entities;
using CardGameServer.Hubs;
using CardGameServer.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace CardGameServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GameController : ControllerBase
    {
        private readonly MouselGameService _gameService;
        private readonly ILogger<GameController> _logger;

        public GameController(ILogger<GameController> logger, MouselGameService gameService)
        {
            _logger = logger;
            _gameService = gameService;
        }

        [HttpGet]
        public string Get()
        {
            var users = new List<User>
            {
                new User() {Admin = true, Guid = "asdfa", Username = "Frede"},
                new User() {Admin = false, Guid = "a123123dfa", Username = "Papa"}
            };
            
            _gameService.InitializeGame("room123", users);


            return "Hello World";
        }
    }
}