using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Command.Bot.Core.Runner;
using Serilog;
using SlackConnector.Models;

namespace Command.Bot.Core.Responders
{
    public class RunResponder : ResponderBase , IResponderDescriptions
    {
        
        private IRunner[] _runners;

        public RunResponder()
        {
            _runners = FileRunners.All;
        }

        #region Overrides of ResponderBase

        public override bool CanRespond(MessageContext context)
        {
            return base.CanRespond(context) && FileRunners.Scripts.Find(context.CleanMessage()) != null;
        }

        #endregion

        #region Overrides of ResponderBase

        public override BotMessage GetResponse(MessageContext context)
        {
            var runner = FileRunners.Scripts.Find(context.CleanMessage());
            
            if (runner == null) return new BotMessage() {Text = "Command not found."};
            Log.Information($"Start executing {runner.Command}");
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var enumerable = runner.Execute(context);
            foreach (var text in enumerable)
            {
                context.Say(text);
            }
            
            stopwatch.Stop();
            Log.Information($"Done executing {runner.Command} in {stopwatch.Elapsed}");
            return new BotMessage() { Text = "Done." };
        }

        #endregion

        #region Implementation of IResponderDescriptions

        public IEnumerable<IResponderDescription> Descriptions => FileRunners.Scripts;

        #endregion
    }
}