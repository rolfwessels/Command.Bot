using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Command.Bot.Core.SlackIntegration.Contracts;
using Serilog;
using SlackNet;
using SlackNet.Bot;
using ILogger = Serilog.ILogger;

namespace Command.Bot.Core.SlackIntegration
{
    public class SlackDotNetConnection : IDisposable
    {
        private static readonly ILogger _log = Log.ForContext(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly string _key;
        private bool _isStarted;

        public SlackDotNetConnection(string key)
        {
            _key = key;
        }

        public SlackBot Bot { get; private set; }

        public async Task Start(ISlackConnectionHandler slackConnectionHandler)
        {
            if (!_isStarted)
            {
                _isStarted = true;
                Bot = new SlackBot(_key);
                Bot.AddHandler(new MyMessageHandler(slackConnectionHandler, this));
                await Bot.Connect();
            }
        }

        public void Stop()
        {
            if (_isStarted)
            {
                _isStarted = false;
                Bot.Dispose();
                Bot = null;
            }
        }

        public class MyMessageHandler : IMessageHandler
        {
            private readonly ISlackConnectionHandler _handler;
            private readonly SlackDotNetConnection _slackDotNetConnection;

            public MyMessageHandler(ISlackConnectionHandler handler, SlackDotNetConnection slackDotNetConnection)
            {
                _handler = handler;
                _slackDotNetConnection = slackDotNetConnection;
            }

            public async Task HandleMessage(IMessage message)
            {
                await _handler.MessageReceived(new Request(message, _slackDotNetConnection));
            }
        }

        public class Request : ISlackRequest
        {
            private readonly SlackDotNetConnection _connection;
            private readonly IMessage _message;

            public Request(IMessage message, SlackDotNetConnection connection)
            {
                _message = message;
                _connection = connection;
                Detail = new DetailContextDetail(message);
            }

            public ISlackDetail Detail { get; }

            public class DetailContextDetail : ISlackDetail
            {
                public DetailContextDetail(IMessage message)
                {
                    UserName = message.User.Name;
                    Text = message.Text;
                    UserId = message.User.Id;
                    ChannelId = message.Conversation.Id;
                }

                public string UserName { get; }
                public string Text { get; }
                public string UserId { get; }
                public string ChannelId { get;  }
            }

            public bool IsForBot()
            {
                return !_message.User.IsBot && (_message.MentionsBot || _message.Conversation.IsPrivate);
            }

            public async Task Reply(ReplyMessage message)
            {
                var botMessage = new BotMessage
                {
                    Text = message.Text,
                    Conversation = _message.Conversation,
                    Attachments = message.Attachments?.Select(x => new Attachment { Text = x.Text, Color = x.ColorHex })
                        .ToList()
                };

                await _connection.Bot.Send(botMessage);
            }

            public async Task WrapInTyping(Task executeRunner)
            {
                await _connection.Bot.WhileTyping(Detail.ChannelId,() => executeRunner);
            }
        }


        public void Dispose()
        {
            Stop();
        }

        public async Task SayToUser(string userName, string message)
        {
            var user = await Bot.GetUserByName(userName);
            var conversationByUserId = await Bot.GetConversationByUserId(user.Id);
            await Bot.Send(new BotMessage { Conversation = conversationByUserId, Text = message });
        }
    }
}