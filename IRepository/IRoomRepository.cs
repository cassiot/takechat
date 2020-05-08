using System.Collections.Generic;
using Take.TakeChat.Domain;

namespace Take.TakeChat.Repository
{
    public interface IRoomRepository
    {
        IEnumerable<Message> GetMessagesForUser(string roomId, string userId);

        void SaveMessage(Message message);
    }
}
