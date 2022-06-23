using System;
using Command.Bot.Core;
using Command.Bot.Core.SlackIntegration;

namespace Command.Bot.Shared
{
    public static class ServiceFactory
    {
        public static SlackService BuildDefaultSlackService()
        {
            MessageContext.BufferTimer = TimeSpan.FromMilliseconds(Settings.Default.BufferTimeMs);
            MessageContext.MaxLength = Settings.Default.MaxSlackMessageLength;
            return new SlackService(Settings.Default.BotKey,
                new ResponseBuilder(Settings.Default.SplitTheAllowedUsers(), Settings.Default.ScriptsPath));
        }
    }
}