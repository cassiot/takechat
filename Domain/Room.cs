using System;
using System.Collections.Generic;

namespace Take.TakeChat.Domain
{
    public class Room
    {
        public string Id { get; private set; } = Guid.NewGuid().ToString();

        public string Name { get; set; }

        public IList<Message> Messages { get; set; } = new List<Message>();
        
        public IList<User> Users { get; set; } = new List<User>();
    }
}
