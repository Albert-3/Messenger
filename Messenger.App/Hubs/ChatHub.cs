using Messenger.Domain.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace Messenger.App.Hubs
{
    public record UserConnection(string UserName, string chatRoom ,DateTime dateTime);

    public class ChatHub : Hub<IChatClient>
    {
        public async Task JoinChat(UserConnection connection)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, connection.chatRoom);
        }
    }

}