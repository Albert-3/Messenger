using Messenger.App.DTOs;
using Messenger.App.Hubs;
using Messenger.Domain;
using Messenger.Domain.Interface;
using Messenger.Domain.Interfaces;
using Microsoft.AspNetCore.SignalR;
using static System.Net.Mime.MediaTypeNames;

namespace Messenger.App.Services
{
    public class MessageService
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IUserRepository _userRepository;
        private readonly IHubContext<ChatHub, IChatClient> _hubContext;
        private readonly IGenerateChatRoomName _generateChatRoomName;
        public MessageService(IMessageRepository messageRepository,
                              IUserRepository userRepository,
                              IHubContext<ChatHub, IChatClient> hubContext,
                              IGenerateChatRoomName generateChatRoomName)
        {
            _messageRepository = messageRepository;
            _userRepository = userRepository;
            _hubContext = hubContext;
            _generateChatRoomName = generateChatRoomName;
        }


        public async Task<Message> SendMessageAsync(string chatRoom, Guid senderId, Guid recipietId, string text)
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
            await _hubContext.Clients.Group(chatRoom)
                .ReceiveMessage(sender.Id, sender.UserName,message.Text, message.Date);

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
        public string GenerateChatRoomName(Guid user1, Guid user2)
        {
            var ids = new[] { user1, user2 }.OrderBy(x => x).ToArray();
            return $"chat_{ids[0]}_{ids[1]}";
        }
    }
}
