namespace Messenger.App.DTOs
{
    public class MessageSenderDTO
    {
        public Guid SenderId { get; set; }
        public string SenderName { get; set; }
        public string Text { get; set; }
        public Guid RecipientId { get; set; }
        public DateTime Date { get; set; }
    }
}
