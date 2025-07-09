using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messenger.Domain.Interfaces
{
    public interface IGenerateChatRoomName
    {
        public string GenerateChatRoomName(Guid senderId, Guid recipientId);

    }
}
