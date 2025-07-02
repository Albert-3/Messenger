using Messenger.App.DTOs;
using Messenger.App.Hubs;
using Messenger.Domain;
using Messenger.Domain.Interface;
using Messenger.Domain.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace Messenger.App.Services
{
    public class MessageService
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IUserRepository _userRepository;
        private readonly IHubContext<ChatHub, IChatClient> _hubContext;

        public MessageService(IMessageRepository messageRepository,
                              IUserRepository userRepository,
                              IHubContext<ChatHub, IChatClient> hubContext)
        {
            _messageRepository = messageRepository;
            _userRepository = userRepository;
            _hubContext = hubContext;
        }


        public async Task<Message> SendMessageAsync(Guid senderId, Guid recipietId, string text)
        {
            var sender = await _userRepository.GetByIdAsync(senderId);
            var recipient = await _userRepository.GetByIdAsync(recipietId);

            var message = new Message
            {
                SenderId = senderId,
                RecipientId = recipietId,
                Text = text,
                Date = DateTime.Now,
                Sender = sender,
                Recipient = recipient,
            };

            await _messageRepository.CreateAsync(message);
            await _hubContext.Clients.User(recipietId.ToString())
                .ReceiveMessage(message.Sender.UserName, message.Text, message.Date, message.SenderId);

            await _hubContext.Clients.User(senderId.ToString())
                .ReceiveMessage(message.Sender.UserName, message.Text, message.Date, message.SenderId);


            return message;
        }
        public async Task<List<GetMessageDTO>> GetMessagesAsync(Guid senderId, Guid recipientId)
        {
            var messageSender = await _messageRepository.GetMessage(senderId, recipientId);
            var messageRecipient = await _messageRepository.GetMessage(recipientId, senderId);

            return messageSender.Concat(messageRecipient)
                .OrderBy(m => m.Date)
                .Select(m => new GetMessageDTO
                {
                    Date = m.Date,
                    SenderId = m.SenderId,
                    SenderName = m.Sender.UserName, 
                    Text = m.Text,
                    RecipientId = m.RecipientId
                })
                .ToList();
        }
    }
}
