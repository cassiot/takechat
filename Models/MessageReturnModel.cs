using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Take.TakeChat.Models
{

    public class MessageReturnModel
    {
        public string FromUserId { get; set; }

        public bool IsPrivate { get; set; }

        public string Text { get; set; }
    }
}
