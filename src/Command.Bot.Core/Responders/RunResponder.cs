using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Command.Bot.Core.Runner;
using Serilog;

namespace Command.Bot.Core.Responders
{
    public class RunResponder : ResponderBase , IResponderDescriptions
    {
        
        private IRunner[] _runners;
        private string _path;

        public RunResponder(string scriptPath)
        {
            _runners = FileRunners.All;
            _path = FileRunners.GetOrCreateFullPath(scriptPath) ;
        }

        #region Overrides of ResponderBase

        public override bool CanRespond(MessageContext context)
        {
            return base.CanRespond(context) && FileRunners.Scripts(_path).Find(context.CleanMessage()) != null;
        }

        #endregion

        #region Overrides of ResponderBase

        public override async Task GetResponse(MessageContext context)
        {
            var runner = FileRunners.Scripts(_path).Find(context.CleanMessage());
            if (runner == null)
            {
                await context.Say("Command not found.");
                return;
            }

            Log.Information($"Start executing {runner.Command}");
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var enumerable = runner.Execute(context);
            foreach (var text in enumerable)
            {
                await context.Say(text);
            }

            await context.FlushMessages();
            stopwatch.Stop();
            Log.Information($"Done executing {runner.Command} in {stopwatch.Elapsed}");
            await context.Say("Done.");
        }

        #endregion

        #region Implementation of IResponderDescriptions

        public IEnumerable<IResponderDescription> Descriptions => FileRunners.Scripts(_path);

        #endregion
    }
}