using System;
using System.Reflection;
using System.Threading.Tasks;
using Serilog;
using SlackNet.Bot;

namespace Command.Bot.Core.SlackIntegration
{
    public class SlackDotNetConnection : IDisposable
    {
        private static readonly ILogger _log = Log.ForContext(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly string _key;
        private bool _isStarted;
        private SlackBot _bot;

        public SlackDotNetConnection(string key)
        {
            _key = key;
        }

        public async Task Start()
        {
            if (!_isStarted)
            {
                _isStarted = true;
                _log.Information($"Starting {_key}");
                _bot = new SlackBot(_key);
                _bot.AddHandler(new MyMessageHandler());
                await _bot.Connect();
            }
        }

        public void Stop()
        {
            if (_isStarted)
            {
                _bot.Dispose();
            }
        }

        public class MyMessageHandler : IMessageHandler
        {
            public Task HandleMessage(IMessage message)
            {
                throw new NotImplementedException();
            }
        }

        #region IDisposable

        public void Dispose()
        {
            Stop();
        }

        #endregion

        public async Task SayToUser(string userName, string message)
        {
            var user = await _bot.GetUserByName(userName);
            var conversationByUserId = await _bot.GetConversationByUserId(user.Id);
            await _bot.Send(new BotMessage() { Conversation = conversationByUserId, Text = message });
        }
    }
}