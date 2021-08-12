using System;
using System.IO;
using System.Reflection;
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
                ChangeCurrentDirectory();
                SetupLogging();
                Log.Information("Starting Command.Bot service.");
                var commands = ConsoleCommandDispatcher.FindCommandsInSameAssemblyAs(typeof(ServiceCommand));
                return ConsoleCommandDispatcher.DispatchCommand(commands, args, Console.Out);
            }
            catch (Exception e)
            {
                Log.Error($"Failed to launch service:{e.Message}", e);
                throw;
            }
        }

        private static void SetupLogging()
        {
            Log.Logger = LogSetup.Default()
                .MinimumLevel.Debug()
                .CreateLogger();
        }

        private static void ChangeCurrentDirectory()
        {
            var location = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if (Directory.GetCurrentDirectory() != location)
            {
                Log.Information($"Change folder from '{Directory.GetCurrentDirectory()}' to '{location}'.");
                Directory.SetCurrentDirectory(location ?? ".");
            }
        }
    }
}
