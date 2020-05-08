using System;

namespace Take.TakeChat.Domain
{
    public class User
    {
        public string Id { get; private set; } = Guid.NewGuid().ToString();

        public string NickName { get; set; }
    }
}
