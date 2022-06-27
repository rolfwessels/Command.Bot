using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Command.Bot.Core;
using Command.Bot.Core.SlackIntegration.Contracts;

namespace Command.Bot.Shared.Components.Runner
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

        public string Command { get; }
        public string Description { get; }

        public Func<IMessageContext, IEnumerable<string>> Execute { get; }

        public string File { get; }
        
        #endregion

        public bool MatchesString(string text)
        {
            return text.ToLower().Trim().Equals(Command.ToLower().Trim()) ||
                   text.ToLower().Trim().StartsWith(Command.ToLower().Trim()+" ");
        }

        public static string GetDescription(string filePath)
        {
            var readAllLines = System.IO.File.ReadAllLines(filePath);
            return readAllLines.Where(x => x.Contains("Description:="))
                .Select(x=>x.Split(new []{":="},StringSplitOptions.None).Last().Trim())
                .FirstOrDefault();
        }
    }

   
}