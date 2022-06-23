using System;
using System.Collections.Generic;
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
        private SlackBot _bot;

        public SlackDotNetConnection(string key)
        {
            _key = key;
        }

        public async Task Start(ISlackConnectionHandler slackConnectionHandler)
        {
            if (!_isStarted)
            {
                _isStarted = true;
                _log.Information($"Starting {_key}");
                _bot = new SlackBot(_key);
                _bot.AddHandler(new MyMessageHandler(slackConnectionHandler,this));
                
                await _bot.Connect();
            }
        }

        public SlackBot Bot => _bot;

        public void Stop()
        {
            if (_isStarted)
            {
                _isStarted = false;
                _bot.Dispose();
                _bot = null;
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
            private readonly IMessage _message;
            private readonly SlackDotNetConnection _connection;

            public Request(IMessage message, SlackDotNetConnection connection)
            {
                _message = message;
                _connection = connection;
                Detail = new DetailContextDetail(message);
            }   

            public class DetailContextDetail : ISlackDetail
            {
                public DetailContextDetail(IMessage message)
                {
                    UserName = message.User.Name;
                    Text = message.Text;
                    UserId = message.User.Id;
                }

                #region Implementation of ISlackMessage

                public string UserName { get; }
                public string Text { get; }
                public string UserId { get; }

                #endregion
            }


            #region Implementation of ISlackMessageContext

            public ISlackDetail Detail { get; }

            public bool IsForBot()
            {
             return !_message.User.IsBot &&  (_message.MentionsBot || _message.Conversation.IsPrivate);
            }

            public async Task Reply(ReplyMessage message)
            {
                var botMessage = new BotMessage() {
                    Text = message.Text ,
                    Conversation = _message.Conversation,
                    Attachments = message.Attachments?.Select(x=>new Attachment() {Text = x.Text,Color = x.ColorHex}).ToList()
                };
                
                await _connection._bot.Send(botMessage);
            }

            public Task IndicateTyping()
            {
                _connection._bot.WhileTyping(_message.Conversation.Id,()=> Task.Delay(1000));
                return Task.CompletedTask;
            }

            #endregion
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