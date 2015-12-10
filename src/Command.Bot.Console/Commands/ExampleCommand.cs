using System;
using System.Collections.Generic;
using NDesk.Options;

namespace Command.Bot.Console.Commands
{

	public class ExampleCommand : CommandBase
	{
		public string Argument1;
		public string Argument2;
		public bool BooleanOption;
		public string OptionalArgument1;
		public string OptionalArgument2;
		public List<string> OptionalArgumentList = new List<string>();

		#region Overrides of CommandBase

		protected override void RunCommand(string[] remainingArguments)
		{
			Argument1 = remainingArguments[0];
			Argument2 = remainingArguments[1];

			System.Console.WriteLine(@"Called Example command - Argument1 = ""{0}"" Argument2 = ""{1}"" BooleanOption: {2}",
			                         Argument1, Argument2, BooleanOption);


			if (BooleanOption)
			{
				throw new Exception("Throwing unhandled exception because BooleanOption is true");
			}
		}

		#endregion


		public ExampleCommand()
		{
			IsCommand("Example","Example implementation of a ManyConsole command-line argument parser Command");
			HasOption("b|booleanOption", "Boolean flag option", b => BooleanOption = true);

			//  Setting .Options directly is the old way to do this, you may prefer to call the helper
			//  method HasOption/HasRequiredOption.
			Options = new OptionSet
				{
					{"l|list=", "Values to add to list", v => OptionalArgumentList.Add(v)},
					{
						"r|requiredArguments=", "Optional string argument requiring a value be specified afterwards",
						s => OptionalArgument1 = s
					},
					{
						"o|optionalArgument:", "Optional String argument which is null if no value follow is specified",
						s => OptionalArgument2 = s ?? "<no argument specified>"
					},
				};

			HasRequiredOption("requiredOption=", "Required string argument also requiring a value.", s => { });
			HasOption("anotherOptional=", "Another way to specify optional arguments", s => { });
			HasAdditionalArguments(2, "<Argument1> <Argument2>");
			
		}

		
	}
}