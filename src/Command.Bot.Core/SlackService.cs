using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Command.Bot.Core.Responders;
using Command.Bot.Core.SlackIntegration;
using Command.Bot.Core.SlackIntegration.Contracts;
using Serilog;

namespace Command.Bot.Core
{
    public class SlackService : ISlackConnectionHandler
    {

        private static readonly ILogger _log = Log.ForContext(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly string _key;
        
        
        private readonly List<IResponder> _responders;
        private readonly TimeSpan _reconnectTime;
        private readonly int _maxReconnectsBeforeClosingAndStartingAgain;
        private readonly SlackDotNetConnection _connector;
        public int ReconnectingCounter { get; set; }

        public SlackService(string key, IResponseBuilder responseBuilder, int defaultWaitRetryMinutes = 5, int maxReconnectsBeforeClosingAndStartingAgain= 10)
        {
            _key = key;
            _connector = new SlackDotNetConnection(key);
            _responders = responseBuilder.GetResponders();
            _reconnectTime = TimeSpan.FromMinutes(defaultWaitRetryMinutes);
            _maxReconnectsBeforeClosingAndStartingAgain = maxReconnectsBeforeClosingAndStartingAgain;
        }

        public async Task Connect()
        {
            _log.Information("Connecting to slack service");
            await _connector.Start(this);
            _log.Information("Connected");
        }

      


        private void Close()
        {
            _connector.Dispose();
        }
        public async Task MessageReceived(ISlackRequest request)
        {
            using (var messageContext = GetMessageContext(request))
            {
                try
                {
                    await ProcessMessage(messageContext);
                }
                catch (Exception e)
                {
                    _log.Error(e.Message, e);
                    await messageContext.Reply($"Ooops something went wrong ({e.Message})");
                }
            }
        }
        

        private IMessageContext GetMessageContext(ISlackRequest message)
        {
            var messageContext = new MessageContext(message);
            return messageContext;
        }

        public async Task ProcessMessage(IMessageContext messageContext)
        {
            _log.Debug($"Message in {messageContext.Message.Detail.UserName}: {messageContext.Message.Detail.Text}");
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