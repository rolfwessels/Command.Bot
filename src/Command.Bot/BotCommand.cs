using System;
using System.Threading.Tasks;
using Command.Bot.Core;
using Command.Bot.Shared;
using Serilog;

namespace Command.Bot
{
    public class BotCommand : CommandBase
    {
        private SlackService _slackService;
        private bool _serviceRun;

        public BotCommand()
        {
            IsCommand("bot", "Bot runner");
            AddArguments("run", "Run the service in command line", () => _serviceRun = true);
            HasAdditionalArguments(1, GetArgumentHelpText());
        }


        #region Overrides of CommandBase

        protected override void RunCommand(string[] remainingArguments)
        {
            if (_serviceRun)
            {
                Log.Information($"Starting service {Settings.Default.BotKey.Substring(1, 5)}");
                _slackService = new SlackService(Settings.Default.BotKey);
                Console.Out.WriteLine("Press any key to stop.");
                Console.ReadKey();
                Log.Information("Starting stopped.");

            }

        }

        #endregion
    }
}