namespace Messenger.Domain
{
    public class Message
    {
        public Guid Id { get; set; }
        public Guid SenderId { get; set; }
        public User Sender { get; set; }

        //Todo: User Recipient { get; set; }
        public Guid RecipientId { get; set; }
        //Todo: if type is string add max lenght restriction on configuration
        public string Text { get; set; }

        public DateTime Date { get; set; }
        //public string SenderName { get; set; }
    }
}
