﻿namespace Messenger.Domain
{
    public class User
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string PhoneNuber { get; set; }
        public string Password { get; set; }
        public ICollection<Message> Messages { get; set; }
    }
}
