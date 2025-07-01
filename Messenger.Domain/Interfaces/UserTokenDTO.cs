using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messenger.Domain.Interfaces
{
    public class UserTokenDTO
    {
        public Guid Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
    }

}
