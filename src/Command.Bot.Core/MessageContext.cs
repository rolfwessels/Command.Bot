using System.Threading.Tasks;
using Command.Bot.Core.Runner;
using SlackConnector;
using SlackConnector.Models;

namespace Command.Bot.Core
{
    public class MessageContext : IMessageContext
    {
        private readonly ISlackConnection _connection;

        public MessageContext(SlackMessage message, bool botHasResponded, ISlackConnection connection)
        {
            _connection = connection;
            Message = message;
            BotHasResponded = false;
            IsFromSlackbot = botHasResponded;
        }

        public SlackMessage Message { get; private set; }
        public bool BotHasResponded { get; set; }
        public bool IsFromSlackbot { get; private set; }

        public Task Say(string text)
        {
            return GetValue(new BotMessage() { Text = text});
        }

        public Task SayOutput(string text)
        {
            if (string.IsNullOrEmpty(text)) return Task.FromResult(true);
            return GetValue(new BotMessage() { Text = $">>>{text}"});
        }

        public Task SayError(string text)
        {
            if (string.IsNullOrEmpty(text)) return Task.FromResult(true);
            return GetValue(new BotMessage() { Text = $">>>`{text}`"});
        }
        public Task GetValue(BotMessage botMessage)
        {
            if (botMessage.ChatHub == null) botMessage.ChatHub = Message.ChatHub;
            if (string.IsNullOrEmpty(botMessage.Text)) return Task.FromResult(true);
            return _connection.Say(botMessage);
        }
    }
}