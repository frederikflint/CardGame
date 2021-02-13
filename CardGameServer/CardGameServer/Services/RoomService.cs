using System;
using System.Collections.Generic;
using System.Linq;
using CardGameServer.Entities;

namespace CardGameServer.Services
{
    public interface IRoomService
    {
        public string AddRoom(string roomId);
        public List<string> GetRooms();
        public List<string> GetUsersInRoom(string roomId);
        public void AddClientToRoom(string clientId, string roomId);
        public void RemoveClientFromRoom(string clientId, string roomId);
        public bool RoomExists(string roomId);
    }

    public class RoomService : IRoomService
    {
        private readonly List<string> _rooms;
        private readonly Dictionary<string, HashSet<string>> _clientRoomDictionary;

        public RoomService()
        {
            _rooms = new List<string>();
            _clientRoomDictionary = new Dictionary<string, HashSet<string>>();
        }

        public string AddRoom(string roomId)
        {
            if (RoomExists(roomId))
            {
                return roomId;
            }

            _rooms.Add(roomId);
            _clientRoomDictionary.Add(roomId, new HashSet<string>());

            return roomId;
        }

        public void AddClientToRoom(string clientId, string roomId)
        {
            if (!RoomExists(roomId))
            {
                throw new Exception($"Room with id: {roomId} does not exist");
            }

            _clientRoomDictionary[roomId].Add(clientId);
        }

        public void RemoveClientFromRoom(string clientId, string roomId)
        {
            if (!RoomExists(roomId))
            {
                throw new Exception($"Room with id: {roomId} does not exist");
            }

            _clientRoomDictionary[roomId].Remove(clientId);
        }

        public List<string> GetRooms()
        {
            return _rooms;
        }

        public List<string> GetUsersInRoom(string roomId)
        {
            return _clientRoomDictionary[roomId].ToList();
        }

        public bool RoomExists(string roomId)
        {
            return _rooms.Contains(roomId);
        }
    }
}