using System;
using System.Collections.Generic;
using System.Linq;
using Take.TakeChat.Domain;

namespace Take.TakeChat.Repository
{
    public class RoomRepository : IRoomRepository
    {
        private static IDictionary<string, Room> rooms;

        public RoomRepository()
        {
            if (rooms == null)
                rooms = new Dictionary<string, Room>();
        }

        public IEnumerable<Room> GetRooms()
        {
            return rooms.Values;
        }

        public void CreateRoom(Room room)
        {
            rooms.Add(room.Id, room);
        }

        public IEnumerable<Message> GetMessagesForUser(string roomId, string userId)
        {
            var room = GetRoom(roomId);
            var user = room.Users.Where(u => u.Id == userId).SingleOrDefault();
            
            if (user == null)
                return new List<Message>();

            var messages = user.Messages;
            user.Messages = new List<Message>();

            return messages;
        }

        public void SaveMessage(Message message)
        {
            if (message == null)
                return;

            var room = GetRoom(message.RoomId);
            
            //If the message is not private, copy it to all users in the room
            if (message.IsPrivate == false)
            {
                foreach (var user in room.Users)
                {
                    //Don't copy to who sent it
                    if (user.Id == message.FromUserId)
                        continue;

                    user.Messages.Add(message);
                }
            }
            else
            {
                var user = room.Users.Where(u => u.Id == message.ToUserId).SingleOrDefault();

                if (user == null || user.Id == message.FromUserId)
                    return;

                user.Messages.Add(message);
            }
        }

        private Room GetRoom(string roomId)
        {
            if (string.IsNullOrWhiteSpace(roomId) || rooms.ContainsKey(roomId) == false)
                throw new Exception("Room not found");

            return rooms[roomId];
        }

        public IEnumerable<User> GetRoomUsers(string roomId)
        {
            var room = GetRoom(roomId);

            return room.Users;
        }

        public void RegisterUserInRoom(string roomId, User user)
        {
            var room = GetRoom(roomId);

            room.Users.Add(user);
        }

        public void RemoveUserFromRoom(string roomId, string userId)
        {
            var room = GetRoom(roomId);
            room.Users = room.Users.Where(u=> u.Id != userId).ToList();
        }
    }
}
