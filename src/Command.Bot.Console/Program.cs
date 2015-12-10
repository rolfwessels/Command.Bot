using System;
using System.Collections.Generic;
using System.Reflection;
using ManyConsole;
using log4net;
using log4net.Config;

namespace Command.Bot.Console
{
	public class Program
	{
		private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		public static IEnumerable<ConsoleCommand> GetCommands()
		{
			return ConsoleCommandDispatcher.FindCommandsInSameAssemblyAs(typeof(Commands.CommandBase));
		}

		#region Private Methods

		[STAThread]
		private static int Main(string[] args)
		{
			XmlConfigurator.Configure();
			_log.Info("Starting Command.Bot app");
			// locate any commands in the assembly (or use an IoC container, or whatever source)
			IEnumerable<ConsoleCommand> commands = GetCommands();
			// then run them.
			return ConsoleCommandDispatcher.DispatchCommand(commands, args, System.Console.Out);
		}

		#endregion
	}
}