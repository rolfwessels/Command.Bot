using Command.Bot.Shared;
using ManyConsole;
using Serilog;

namespace Command.Bot
{
    public class Program
    {
        public static int Main(string[] args)
        {
            Log.Logger = LogSetup.Default().CreateLogger();
            Log.Information($"Starting Command.Bot app.");
            // locate any commands in the assembly (or use an IoC container, or whatever source)
            var commands = ConsoleCommandDispatcher.FindCommandsInSameAssemblyAs(typeof(BotCommand));
            // then run them.
            return ConsoleCommandDispatcher.DispatchCommand(commands, args, System.Console.Out);
        }
    }
}