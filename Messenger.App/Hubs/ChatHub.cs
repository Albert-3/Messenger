using Messenger.Domain.Interfaces;
using Microsoft.AspNetCore.SignalR;
using System.Reflection;

namespace Messenger.App.Hubs
{
    public record UserConnection(Guid senderId, Guid RecipientId);

    public class ChatHub : Hub<IChatClient> ,IGenerateChatRoomName
    {
        public async Task JoinChat(UserConnection connection)
        {
            string chatRoom = GenerateChatRoomName(connection.senderId, connection.RecipientId);
            await Groups.AddToGroupAsync(Context.ConnectionId,chatRoom);
        }
        public string GenerateChatRoomName(Guid user1, Guid user2)
        {
            var ids = new[] { user1, user2 }.OrderBy(x => x).ToArray();
            return $"chat_{ids[0]}_{ids[1]}";
        }


    }

}