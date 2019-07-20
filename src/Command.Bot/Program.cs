using System.Collections.Generic;
using Command.Bot.Commands;
using Command.Bot.Console.Commands;
using Command.Bot.Core.Responders;
using ManyConsole;
using Serilog;

namespace Command.Bot
{
    public class Program
    {
        public static IEnumerable<ConsoleCommand> GetCommands()
        {
            return ConsoleCommandDispatcher.FindCommandsInSameAssemblyAs(typeof(CommandBase));
        }

		#region Private Methods
            public static int Main(string[] args)
        {
            Log.Logger = LogSetup.Default().CreateLogger();

            Log.Information($"Starting Command.Bot app [{args.StringJoinAnd()}]");
            // locate any commands in the assembly (or use an IoC container, or whatever source)
            var commands = GetCommands();
            // then run them.
            return ConsoleCommandDispatcher.DispatchCommand(commands, args, System.Console.Out);
        }
	    #endregion
    }
}