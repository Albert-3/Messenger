using Messenger.App.DTOs;
using Messenger.Domain;
using Messenger.Domain.Interface;

namespace Messenger.App.Services
{
    public class MessageService
    {
        public IMessageRepository _messageRepository;
        public MessageService(IMessageRepository messageRepository )
        {
            _messageRepository = messageRepository;
        }

        public async Task<Message> MessageSender(Guid senderId, Guid recipietId, string text)
        {
            var message = new Message
            {
                SenderId = senderId,
                RecipientId = recipietId,
                Text = text,
                Date = DateTime.Now,
            };
            await _messageRepository.CreateAsync(message);

            return message;
        }
        public async Task<List<GetMessageDTO>> GetMessage(Guid senderId, Guid recipientId)
        {
            var messageSender = await _messageRepository.GetMessage(senderId, recipientId);
            var messageRecipient = await _messageRepository.GetMessage(recipientId, senderId);

            messageSender.Select(message => new GetMessageDTO
            {
                Date = message.Date,
                SenderId = message.SenderId,
                SenderName = message.Sender.UserName,
                Text = message.Text,
                RecipientId = message.RecipientId
            })
            .ToList();

            messageRecipient.Select(message => new GetMessageDTO
            {
                Date = message.Date,
                SenderId = message.SenderId,
                SenderName = message.Sender.UserName,
                Text = message.Text,
                RecipientId = message.RecipientId
            })
            .ToList();

            return  messageSender.Concat(messageRecipient)
                .OrderBy(m => m.Date).Select(x => new GetMessageDTO
                {
                    Date = x.Date,
                    SenderId = x.SenderId,
                    SenderName = x.Sender.UserName,
                    Text = x.Text,
                    RecipientId = x.RecipientId
                })
                .ToList();
        }
    }
}
