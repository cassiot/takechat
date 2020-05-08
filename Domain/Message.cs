using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Take.TakeChat.Domain
{
    public class Message
    {
        public string FromUserId { get; set; }
        
        public string ToUserId { get; set; }

        public bool IsPrivate { get; set; }

        public string Text { get; set; }
    }

    public class User
    {
        public string Id { get; set; }

        public string NickName { get; set; }
    }
}
