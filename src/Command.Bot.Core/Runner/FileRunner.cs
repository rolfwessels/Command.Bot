using System;
using System.Collections.Generic;

namespace Command.Bot.Core.Runner
{
    public class FileRunner : IResponderDescription
    {
        public FileRunner(string command, string description, Func<IMessageContext, IEnumerable<string>> execute, string file)
        {
            Execute = execute;
            Command = command;
            Description = description;
            File = file;
        }

        #region Implementation of IResponderDescription

        public string Command { get; private set; }
        public string Description { get; private set; }

        public Func<IMessageContext, IEnumerable<string>> Execute { get; }

        public string File { get; private set; }

        #endregion

        public bool MatchesString(string text)
        {
            return text.ToLower().Trim().Equals(Command.ToLower().Trim());
        }
    }

   
}