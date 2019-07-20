using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Command.Bot.Core.Responders;
using Serilog;
using SlackConnector;
using SlackConnector.Models;

namespace Command.Bot.Core
{
    public class SlackService
    {
        
        private readonly string _key;
        private readonly ISlackConnector _connector;
        private ISlackConnection _connection;
        private readonly List<IResponder> _responders;

        public SlackService(string key)
        {
            _key = key;

            _connector = new SlackConnector.SlackConnector();
            _responders = ResponseBuilder.GetResponders();
        }

        public async Task Connect()
        {
            Log.Information("Connecting to slack service");
            _connection = await _connector.Connect(_key);
            Log.Information("Connected");
            LinkEvents();
        }

        private void LinkEvents()
        {
            RemoveEvents();
            _connection.OnMessageReceived += MessageReceived;
            _connection.OnDisconnect += Disconnected;
        }

        private void RemoveEvents()
        {
            _connection.OnMessageReceived -= MessageReceived;
            _connection.OnDisconnect -= Disconnected;
        }

        private void Disconnected()
        {
            Log.Information("Disconnected");
            
            RemoveEvents();
            try
            {
                _connection.Close().Wait();
            }
            catch (Exception e)
            {
                Log.Warning("SlackService Ensure disconnected: " + e.Message);
            }
            Log.Information("Trying to reconnect.");
            Connect().ContinueWith(x=>
            {
                if (x.Exception != null) Log.Error(x.Exception.Message, x.Exception);
            });
        }

        private async Task MessageReceived(SlackMessage message)
        {
            Log.Information("message" + message.Text);
            var messageContext = GetMessageContext(message);
            try
            {
                await ProcessMessage(messageContext);
            }
            catch (Exception e)
            {
                Log.Error(e.Message, e);
                _connection.Say(new BotMessage()
                {
                    Text = string.Format("Ooops something went wrong ({0})", e.Message),
                    ChatHub = message.ChatHub
                }).Wait();
            }
        }

        private MessageContext GetMessageContext(SlackMessage message)
        {
            const bool botHasResponded = false;
            var messageContext = new MessageContext(message, botHasResponded, _connection);

            return messageContext;
        }

        private async Task ProcessMessage(MessageContext messageContext)
        {
            Log.Debug(string.Format("Message in {0}: {1}", messageContext.Message.User.Name,
                messageContext.Message.Text));
            foreach (var responder in _responders)
            {
                if (responder.CanRespond(messageContext))
                {
                    var botMessage = responder.GetResponse(messageContext);
                    if (botMessage != null)
                    {
                        if (botMessage.ChatHub == null)
                        {
                            botMessage.ChatHub = messageContext.Message.ChatHub;
                        }
                        await _connection.Say(botMessage);
                        Log.Debug(string.Format("Message out {0}: {1}", messageContext.Message.User.Name,
                            botMessage.Text));
                        messageContext.BotHasResponded = true;
                    }
                }
            }
        }
    }
}