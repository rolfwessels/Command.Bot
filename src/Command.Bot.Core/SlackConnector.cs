using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using log4net;
using MargieBot.Responders;

namespace Command.Bot.Core
{
    public class SlackConnector
    {
        private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public void Connect()
        {
            var myBot = new MargieBot.Bot();
            AddRepsponders(myBot);

            _log.Info("Connecting to slack service");
            myBot.ConnectionStatusChanged += MyBotOnConnectionStatusChanged;
            myBot.Connect("xoxb-14491425043-0v6oSPpQVsJ1zjqtiFN5HX4P");
        }

        private static void AddRepsponders(MargieBot.Bot myBot)
        {
            var bots = new List<IResponder> {
                new RemoveInstructions(),
                new DefaultResponse()
            };
            myBot.Responders.Add(new HelpResponder(bots.OfType<IResponderDescription>()));
            foreach (var responder in bots)
            {
                myBot.Responders.Add(responder);
            }
        }

        private void MyBotOnConnectionStatusChanged(bool isConnected)
        {
            _log.Info(isConnected ? "Connected" : "Disconnected");
        }
    }
}