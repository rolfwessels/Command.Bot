using System;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace Command.Bot.Shared
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
        public int BufferTimeMs => Convert.ToInt32(ReadConfigValue("BufferTimeMs", "2000"));
        public int MaxSlackMessageLength => Convert.ToInt32(ReadConfigValue("MaxSlackMessageLength", "200"));

        public string[] SplitTheAllowedUsers()
        {
            return AllowedUser.Split(',', ':', ';').Select(x => x.Trim()).Where(x => !string.IsNullOrEmpty(x)).ToArray();
        }
    }
}