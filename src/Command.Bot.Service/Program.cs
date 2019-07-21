using System;
using Command.Bot.Service.Commands;
using Command.Bot.Shared;
using ManyConsole;
using Serilog;

namespace Command.Bot.Service
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        private static int Main(string[] args)
        {
            try
            {
                Console.Out.WriteLine("Console.Out");
                Log.Logger = LogSetup.Default().CreateLogger();
                Log.Information($"Starting Command.Bot service.");
                // locate any commands in the assembly (or use an IoC container, or whatever source)
                var commands = ConsoleCommandDispatcher.FindCommandsInSameAssemblyAs(typeof(ServiceCommand));
                // then run them.
                return ConsoleCommandDispatcher.DispatchCommand(commands, args, Console.Out);
            }
            catch (Exception e)
            {
                Log.Error(e.Message, e);
                throw;
            }
        }
    }
}
