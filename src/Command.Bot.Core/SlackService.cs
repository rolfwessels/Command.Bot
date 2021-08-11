using System;
using System.Collections.Generic;
using System.Linq;
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

        private static readonly ILogger _log = Log.ForContext(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly string _key;
        private readonly ISlackConnector _connector;
        private ISlackConnection _connection;
        private readonly List<IResponder> _responders;
        private readonly TimeSpan _reconnectTime;
        private readonly int _maxReconnectsBeforeClosingAndStartingAgain;
        public int ReconnectingCounter { get; set; }

        public SlackService(string key, IResponseBuilder responseBuilder, int defaultWaitRetryMinutes = 5, int maxReconnectsBeforeClosingAndStartingAgain= 10)
        {
            _key = key;
            _connector = new SlackConnector.SlackConnector();
            _responders = responseBuilder.GetResponders();
            _reconnectTime = TimeSpan.FromMinutes(defaultWaitRetryMinutes);
            _maxReconnectsBeforeClosingAndStartingAgain = maxReconnectsBeforeClosingAndStartingAgain;
        }

        public async Task Connect()
        {
            _log.Information("Connecting to slack service");
            _connection = await _connector.Connect(_key);
            _log.Information("Connected");
            LinkEvents();
        }

        private void LinkEvents(bool linkEvents = true)
        {
            if (_connection == null) return;
            _connection.OnMessageReceived -= MessageReceived;
            _connection.OnDisconnect -= OnDisconnectedTryReconnect;
            _connection.OnReconnecting -= Reconnecting;
            if (linkEvents)
            {
                _connection.OnMessageReceived += MessageReceived;
                _connection.OnDisconnect += OnDisconnectedTryReconnect;
                _connection.OnReconnecting += Reconnecting;
            }
        }


        private async Task Reconnecting()
        {
            ReconnectingCounter++;
            _log.Debug($"OnReconnecting {ReconnectingCounter}");
            
            if (ReconnectingCounter >= _maxReconnectsBeforeClosingAndStartingAgain)
            {
                ReconnectingCounter = 0;
                Close();
                _log.Debug($"Wait a {Math.Round(_reconnectTime.TotalMinutes)}m then reconnect...");
                await Task.Delay(_reconnectTime);
                _log.Debug($"Trying to reconnect!");
                await Connect();
            }
        }

        private void OnDisconnectedTryReconnect()
        {
            _log.Information("Disconnected. Slack will try to reconnect on its own.");
        }

        private void Close()
        {
            if (_connection == null) return;
            LinkEvents(false);
            try
            {
                _connection.Close().Wait();
                _connection = null;
            }
            catch (Exception e)
            {
                _log.Warning("SlackService Ensure disconnected: " + e.Message);
            }
        }

        private async Task MessageReceived(SlackMessage message)
        {

            var messageContext = GetMessageContext(message);
            try
            {
                await ProcessMessage(messageContext);
            }
            catch (Exception e)
            {
                _log.Error(e.Message, e);
                _connection.Say(new BotMessage()
                {
                    Text = $"Ooops something went wrong ({e.Message})",
                    ChatHub = message.ChatHub
                }).Wait();
            }
        }

        private MessageContext GetMessageContext(SlackMessage message)
        {
            var messageContext = new MessageContext(message,_connection);
            return messageContext;
        }

        public async Task ProcessMessage(MessageContext messageContext)
        {
            _log.Debug($"Message in {messageContext.Message.User.Name}: {messageContext.Message.Text}");
            foreach (var responder in _responders.Where(responder => responder.CanRespond(messageContext)))
            {
                await responder.GetResponse(messageContext);
                break;
            }
        }

        #region IDisposable

        public void Dispose()
        {
            Close();
        }

        #endregion
    }

    public interface IResponseBuilder
    {
        List<IResponder> GetResponders();
    }
}