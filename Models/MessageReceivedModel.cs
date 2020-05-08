namespace Take.TakeChat.Models
{
    public class MessageReceivedModel
    {
        public string FromUserId { get; set; }

        public string ToUserId { get; set; }

        public bool IsPrivate { get; set; }

        public string Text { get; set; }
    }
}
