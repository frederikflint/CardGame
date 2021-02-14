using System;
using System.Collections.Generic;
using CardGameServer.Entities;

namespace CardGameServer.Services
{
    public interface IMessageService
    {
        public Message AddMessage(string user, string text, string roomId);
        public List<Message> GetMessages(string roomId);
    }

    public class MessageService : IMessageService
    {
        private readonly IRoomService _roomService;
        private readonly Dictionary<string, List<Message>> _messages;

        public MessageService(IRoomService roomService)
        {
            _roomService = roomService;
            _messages = new Dictionary<string, List<Message>>();
        }

        public Message AddMessage(string user, string text, string roomId)
        {
            if (!_roomService.RoomExists(roomId))
            {
                throw new Exception($"Room with id: {roomId} does not exist");
            }

            var message = new Message() {Text = text, User = user};

            if (!_messages.ContainsKey(roomId))
            {
                _messages.Add(roomId, new List<Message>());
            }

            _messages[roomId].Add(message);

            return message;
        }

        public List<Message> GetMessages(string roomId)
        {
            if (! _messages.ContainsKey(roomId))
            {
                return new List<Message>();
            }
            
            return _messages[roomId] == null ? new List<Message>() : _messages[roomId];
        }
    }
}