using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Take.TakeChat.Domain
{
    public class Message
    {
        public string Id { get; private set; } = Guid.NewGuid().ToString();

        public string RoomId { get; set; }

        public string FromUserId { get; set; }

        public string ToUserId { get; set; }

        public bool IsPrivate { get; set; }

        public string Text { get; set; }
    }
}
