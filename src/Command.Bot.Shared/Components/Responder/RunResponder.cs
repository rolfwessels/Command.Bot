using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Command.Bot.Core;
using Command.Bot.Core.Responders;
using Command.Bot.Core.SlackIntegration.Contracts;
using Command.Bot.Shared.Components.Runner;
using Serilog;

namespace Command.Bot.Shared.Components.Responder
{
    public class RunResponder : ResponderBase, IResponderDescriptions
    {
        private IRunner[] _runners;
        private string _path;

        public RunResponder(string scriptPath)
        {
            _runners = FileRunners.All;
            _path = FileRunners.GetOrCreateFullPath(scriptPath);
        }

        #region Overrides of ResponderBase

        public override bool CanRespond(IMessageContext context)
        {
            return base.CanRespond(context) && FileRunners.Scripts(_path).Find(context.Text) != null;
        }

        #endregion

        #region Overrides of ResponderBase

        public override async Task GetResponse(IMessageContext context)
        {
            var runner = FileRunners.Scripts(_path).Find(context.Text);
            if (runner == null)
            {
                await context.Reply("Command not found.");
                return;
            }

            await context.WrapInTyping(() => ExecuteRunner(context, runner));
        }

        private static async Task ExecuteRunner(IMessageContext context, FileRunner runner)
        {
            Log.Information($"Start executing {runner.Command}");
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var enumerable = runner.Execute(context);
            foreach (var text in enumerable)
            {
                await context.Reply(text);
            }

            await context.FlushMessages();
            stopwatch.Stop();
            Log.Information($"Done executing {runner.Command} in {stopwatch.Elapsed}");
            await context.Reply("Done.");
        }

        #endregion

        #region Implementation of IResponderDescriptions

        public IEnumerable<IResponderDescription> Descriptions => FileRunners.Scripts(_path);

        #endregion
    }
}