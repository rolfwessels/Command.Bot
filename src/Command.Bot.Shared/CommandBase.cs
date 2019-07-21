using System;
using System.Collections.Generic;
using System.Linq;
using ManyConsole;
using Serilog;

namespace Command.Bot.Shared
{
    public abstract class CommandBase : ConsoleCommand
    {
        private readonly List<Argument> _arguments;

        protected CommandBase()
        {
            _arguments = new List<Argument>();
            HasOption("v|verbose", "Verbose output", b => Verbose = true);
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
                Log.Error(e.Message, e);
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

        protected string GetArgumentHelpText()
        {
            var list = new List<string>();
            if (_arguments.Any())
            {
                var pad = _arguments.Select(x => x.Name.Length).Max() + 3;
                list.Add("<arguments>");
                list.Add("");
                list.Add("<arguments> available:");
                foreach (var argument in _arguments)
                    list.Add(string.Format("  {0} {1}", argument.Name.PadRight(pad, ' '), argument.Descriptions));
                list.Add("");
            }

            return string.Join(Environment.NewLine, list.ToArray());
        }

        #region Private Methods

        private void SetArgumentValues(string[] remainingArguments)
        {
            var strings = remainingArguments.Select(x => x.ToLower()).ToArray();
            foreach (var argument in _arguments)
                if (strings.Contains(argument.Name))
                    argument.Valid();
        }

        private static void AddNLogConsoleOutput()
        {
            Log.Logger = LogSetup.Default()
                .WriteTo.Console()
                .CreateLogger();
            Log.Information("Adding console output:");
        }

        #endregion

        #region Nested type: Argument

        public class Argument
        {
            public Argument(string name, string descriptions, Action valid)
            {
                Name = name;
                Descriptions = descriptions;
                Valid = valid;
            }

            public string Name { get; }

            public string Descriptions { get; }

            public Action Valid { get; }
        }

        #endregion
    }
}