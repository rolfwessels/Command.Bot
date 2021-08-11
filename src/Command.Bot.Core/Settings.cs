using System;
using Microsoft.Extensions.Configuration;

namespace Command.Bot.Core
{
    public class Settings : BaseSettings
    {
        private static readonly Lazy<Settings> _instance = new Lazy<Settings>(() => new Settings(_config.Value));

        public Settings(IConfiguration configuration) : base(configuration, "Bot")
        {
        }

        public static Settings Default => _instance.Value;
        public string BotKey => ReadConfigValue("BotKey", "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx");
        public string AllowedUser => ReadConfigValue("AllowedUser", "xxxxxxxxxxxxxxxxxx");
        public string ScriptsPath => ReadConfigValue("ScriptsPath", "xxxxxxxxxxxxxxxxxx");
        public int MaxReconnectTries => Convert.ToInt32(ReadConfigValue("MaxReconnectTries", "10"));
        public int WaitRetryMinutes => Convert.ToInt32(ReadConfigValue("WaitRetryMinutes", "5"));
    }
}