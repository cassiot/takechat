using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Take.TakeChat.Api.Models
{
    public class MessageReceived
    {
        public string FromUserId { get; set; }

        public string ToUserId { get; set; }

        public bool IsPrivate { get; set; }

        public string Text { get; set; }
    }

    public class MessageReturn
    {
        public string FromUserId { get; set; }

        public bool IsPrivate { get; set; }

        public string Text { get; set; }
    }
}
