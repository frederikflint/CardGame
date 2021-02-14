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
        public string GetRoomFromUserGuid(string guid);
        public User GetUserFromUserGuid(string guid);
        public List<User> GetUsersInRoom(string roomId);
        public User AddUserToRoom(string username, string roomId);
        public void RemoveClientFromRoom(string clientId, string roomId);
        public bool RoomExists(string roomId);
        public void CleanupUser(string guid);
    }

    public class RoomService : IRoomService
    {
        private readonly List<string> _rooms;
        private readonly Dictionary<string, HashSet<string>> _roomClientDictionary;
        private readonly Dictionary<string, User> _guidUserDictionary;

        public RoomService()
        {
            _rooms = new List<string>();
            _roomClientDictionary = new Dictionary<string, HashSet<string>>();
            _guidUserDictionary = new Dictionary<string, User>();
        }

        public string AddRoom(string roomId)
        {
            if (RoomExists(roomId))
            {
                return roomId;
            }

            _rooms.Add(roomId);
            _roomClientDictionary.Add(roomId, new HashSet<string>());

            return roomId;
        }

        public User AddUserToRoom(string username, string roomId)
        {
            if (!RoomExists(roomId))
            {
                throw new Exception($"Room with id: {roomId} does not exist");
            }

            var user = new User() {Username = username, Guid = Guid.NewGuid().ToString(), Admin = false};

            _roomClientDictionary[roomId].Add(user.Guid);

            if (_roomClientDictionary[roomId].Count == 1)
            {
                user.Admin = true;
            }

            _guidUserDictionary.Add(user.Guid, user);

            return user;
        }

        public void RemoveClientFromRoom(string clientId, string roomId)
        {
            if (!RoomExists(roomId))
            {
                throw new Exception($"Room with id: {roomId} does not exist");
            }

            _roomClientDictionary[roomId].Remove(clientId);
        }

        public List<string> GetRooms()
        {
            return _rooms;
        }

        public string GetRoomFromUserGuid(string guid)
        {
            return _rooms.Find(room => _roomClientDictionary[room].Contains(guid));
        }

        public User GetUserFromUserGuid(string guid)
        {
            if (!_guidUserDictionary.ContainsKey(guid))
            {
                return null;
            }

            return _guidUserDictionary[guid];
        }

        public List<User> GetUsersInRoom(string roomId)
        {
            if (!_roomClientDictionary.ContainsKey(roomId))
            {
                return new List<User>();
            }

            return _roomClientDictionary[roomId].ToList().Select(guid => _guidUserDictionary[guid]).ToList();
        }

        public bool RoomExists(string roomId)
        {
            return _rooms.Contains(roomId);
        }

        public void CleanupUser(string guid)
        {
            if (!_guidUserDictionary.ContainsKey(guid))
            {
                return;
            }

            var user = _guidUserDictionary[guid];
            _guidUserDictionary.Remove(guid);

            var room = GetRoomFromUserGuid(guid);
            if (room == null || !_roomClientDictionary.ContainsKey(room))
            {
                return;
            }

            _roomClientDictionary[room].Remove(guid);

            if (_roomClientDictionary[room].Count == 0)
            {
                _roomClientDictionary.Remove(room);
                _rooms.Remove(room);
            }
        }
    }
}