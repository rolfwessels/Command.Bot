using SlackConnector;
using SlackConnector.Models;

namespace Command.Bot.Core
{
    public static class SlackConnectionHelper
    {
        public static void Say(this ISlackConnection connection, string s)
        {
            connection.Say(new BotMessage() {Text = s});
        }
    }
}