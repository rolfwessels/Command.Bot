using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Command.Bot.Core.Runner;
using log4net;
using SlackConnector.Models;

namespace Command.Bot.Core.Responders
{
    public class RunResponder : ResponderBase , IResponderDescriptions
    {
        private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private IRunner[] _runners;

        public RunResponder()
        {
            _runners = FileRunners.All;
        }

        #region Overrides of ResponderBase

        public override bool CanRespond(MessageContext context)
        {
            return base.CanRespond(context) && MatchRunners.Find(FileRunners.Scripts, context.CleanMessage()) != null;
        }

        #endregion

        #region Overrides of ResponderBase

        public override BotMessage GetResponse(MessageContext context)
        {
            var runner = MatchRunners.Find(FileRunners.Scripts, context.CleanMessage());
            if (runner == null) return new BotMessage() {Text = "Command not found."};
            var enumerable = runner.Execute(context);
            foreach (var text in enumerable)
            {
                context.Say(text);
            }
            return new BotMessage() { Text = "Done." };
        }

        #endregion

       
        #region Implementation of IResponderDescriptions

        public IEnumerable<IResponderDescription> Descriptions {
            get
            {
                return FileRunners.Scripts;
            }
        }

        #endregion
    }
}