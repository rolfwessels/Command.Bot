using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ManyConsole;
using log4net;
using log4net.Appender;
using log4net.Core;
using log4net.Layout;
using log4net.Repository.Hierarchy;

namespace Command.Bot.Console.Commands
{
	public abstract class CommandBase : ConsoleCommand
	{
		private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		private readonly List<Argument> _arguments;

		protected CommandBase()
		{
			_arguments = new List<Argument>();
			HasOption("v|verbose", "Verbose output", b => Verbose = false);
		}

		protected bool Verbose { get; set; }

		public override int Run(string[] remainingArguments)
		{

			if (Verbose) AddNLogConsoleOutput();
			try
			{
				SetArgumentValues(remainingArguments);
				RunCommand(remainingArguments);
			}
			catch (Exception e)
			{
				_log.Error(e.Message, e);
				System.Console.Error.WriteLine(e.Message);
				return 1;
			}
			return 0;
		}

		

		protected abstract void RunCommand(string[] remainingArguments);

		protected void AddArguments(string name, string descriptions, Action valid)
		{
			_arguments.Add(new Argument(name.Trim().ToLower(), descriptions, valid));
		}

		#region Private Methods

		private void SetArgumentValues(string[] remainingArguments)
		{
			var strings = remainingArguments.Select(x => x.ToLower()).ToArray();
			foreach (var argument in _arguments)
			{
				if (strings.Contains(argument.Name))
				{
					argument.Valid();
				}
			}
		}

		protected string GetArgumentHelpText()
		{
			var list = new List<string>();
			if (_arguments.Any())
			{
				var pad = _arguments.Select(x => x.Name.Length).Max() + 3;
				list.Add(string.Format("<arguments>"));
				list.Add("");
				list.Add("<arguments> available:");
				foreach (var argument in _arguments)
				{
					list.Add(string.Format("  {0} {1}", argument.Name.PadRight(pad, ' '), argument.Descriptions));
				}
				list.Add("");
			}
            return string.Join(Environment.NewLine,list.ToArray());
		}
		private static void AddNLogConsoleOutput()
		{
			var repository = (Hierarchy) LogManager.GetRepository();
			var appender = new ConsoleAppender
				{
					Layout = new PatternLayout("%date %-5level  [%ndc] - %message%newline")
				};
			repository.Root.AddAppender(appender);
			repository.Configured = true;
			repository.RaiseConfigurationChanged(EventArgs.Empty);
			appender.Threshold = Level.Debug;
		}

		#endregion

		#region Nested type: Argument

		public class Argument
		{
			private readonly string _descriptions;
			private readonly string _name;
			private readonly Action _valid;

			public Argument(string name, string descriptions, Action valid)
			{
				_name = name;
				_descriptions = descriptions;
				_valid = valid;
			}

			public string Name
			{
				get
				{
					return _name;
				}
			}

			public string Descriptions
			{
				get { return _descriptions; }
			}

			public Action Valid
			{
				get { return _valid; }
			}
		}

		#endregion

		
	}
}