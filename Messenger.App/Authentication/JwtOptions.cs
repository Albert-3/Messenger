﻿namespace Messenger.App.Authentication
{
    public class JwtOptions
    {
        public string Key { get; set; } = string.Empty;
        public int ExpirationTime { get; set; }

    }
}
