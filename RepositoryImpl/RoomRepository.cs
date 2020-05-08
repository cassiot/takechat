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

        public IEnumerable<Message> GetMessagesForUser(string roomId, string userId)
        {
            var room = GetRoom(roomId);

            var messages = room.Messages.Where(m => m.ToUserId == userId);
            room.Messages = room.Messages.Where(m => m.ToUserId != userId).ToList();

            return messages;
        }

        public void SaveMessage(string roomId, Message message)
        {
            var room = GetRoom(roomId);

            room.Messages.Add(message);
        }

        private Room GetRoom(string roomId)
        {
            if (rooms.ContainsKey(roomId) == false)
                throw new Exception("Room not found");

            return rooms[roomId];
        }
    }
}
