using Command.Bot.Shared;
using ManyConsole;
using Serilog;

namespace Command.Bot.Service
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static int Main(string[] args)
        {
            Log.Logger = LogSetup.Default().CreateLogger();
            Log.Information($"Starting Command.Bot service.");
            // locate any commands in the assembly (or use an IoC container, or whatever source)
            var commands = ConsoleCommandDispatcher.FindCommandsInSameAssemblyAs(typeof(CommandBase));
            // then run them.
            return ConsoleCommandDispatcher.DispatchCommand(commands, args, System.Console.Out);
        }
    }
}
