using System;
using System.Net.Mime;
using System.Threading;
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
        private bool _console;

        public BotCommand()
        {
            IsCommand("bot", "Bot runner");
            AddArguments("run", "Run the service in command line", () => _serviceRun = true);
            HasOption("c", "Don't use the console read to block exist", b => _console = true);
            HasAdditionalArguments(1, GetArgumentHelpText());
        }


        #region Overrides of CommandBase

        protected override void RunCommand(string[] remainingArguments)
        {
            if (_serviceRun)
            {
                Log.Information($"Starting service {Settings.Default.BotKey.Substring(1, 5)}");
                _slackService = new SlackService(Settings.Default.BotKey, new ResponseBuilder(Settings.Default.SplitTheAllowedUsers(), Settings.Default.ScriptsPath), Settings.Default.WaitRetryMinutes, Settings.Default.MaxReconnectTries);
                _slackService.Connect().ContinueWith(Connected);
                if (!_console)
                {
                    Console.Out.WriteLine("Press any key to stop.");
                    Console.ReadKey();
                }
                else
                {
                    bool stop = false;
                    Console.CancelKeyPress += (o,c) => stop = true;
                    Console.Out.WriteLine("Press CTRL+C to stop.");
                    while (!stop)
                    {
                        Thread.Sleep(10000);
                    }
                }
                Log.Information("Starting stopped.");

            }

        }

        private void Connected(Task obj)
        {
            if (obj.Exception != null) Log.Error(obj.Exception.Message, obj.Exception);
        }


        #endregion
    }
}