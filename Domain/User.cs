using System;
using System.Collections.Generic;

namespace Take.TakeChat.Domain
{
    public class User
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public IList<Message> Messages { get; set; } = new List<Message>();
    }
}
