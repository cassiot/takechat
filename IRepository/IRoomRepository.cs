using System.Collections.Generic;
using Take.TakeChat.Domain;

namespace Take.TakeChat.Repository
{
    public interface IRoomRepository
    {
        IEnumerable<Message> GetMessagesForUser(string userId, string roomId);

        void SaveMessage(string roomId, Message message);
    }
}
