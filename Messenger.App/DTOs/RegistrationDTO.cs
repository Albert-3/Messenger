﻿namespace Messenger.App.DTOs
{
    public class RegistrationDTO
    {
        public string UserName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }   
        public string Password { get; set; }
        public string RepeatPassword { get; set; }
    }
}
