namespace Messenger.App.DTOs
{
    public class GetMessageDTO
    {
        public Guid SenderId { get; set; }
        public string SenderName { get; set; }
        public string Text { get; set; }
        public Guid RecipientId { get; set; }
        public string RecipientName { get; set; }
        public DateTime Date { get; set; }
    }
}
