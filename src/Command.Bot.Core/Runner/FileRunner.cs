using System;
using System.Collections.Generic;

namespace Command.Bot.Core.Runner
{
    public class FileRunner : IResponderDescription
    {
        private readonly Func<MessageContext, IEnumerable<string>> _execute;

        public FileRunner(string command, string description, Func<MessageContext, IEnumerable<string>> execute, string file)
        {
            _execute = execute;
            Command = command;
            Description = description;
            File = file;
        }

        #region Implementation of IResponderDescription

        public string Command { get; private set; }
        public string Description { get; private set; }

        public Func<MessageContext, IEnumerable<string>> Execute
        {
            get { return _execute; }
        }

        public string File { get; private set; }

        #endregion

        public bool MatchesString(string text)
        {
            return text.ToLower().StartsWith(Command.ToLower());
        }
    }
}