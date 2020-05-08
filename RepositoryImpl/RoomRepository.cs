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

        public void SaveMessage(Message message)
        {
            if (message == null)
                return;

            var room = GetRoom(message.RoomId);

            room.Messages.Add(message);
        }

        private Room GetRoom(string roomId)
        {
            if (string.IsNullOrWhiteSpace(roomId) || rooms.ContainsKey(roomId) == false)
                throw new Exception("Room not found");

            return rooms[roomId];
        }
    }
}
