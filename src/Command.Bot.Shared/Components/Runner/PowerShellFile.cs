using System;
using System.Collections.Generic;
using System.IO;
using Command.Bot.Core.MessageContext;

namespace Command.Bot.Shared.Components.Runner
{
    public class PowerShellFile : BaseCommandLineRunner, IRunner
    {
        
        #region Implementation of IRunner

        public string Extension
        {
            get { return ".ps1"; }
        }

        public FileRunner GetRunner(string filePath)
        {
            if (this.IsExtensionMatch(filePath))
            {
                var command = Path.GetFileNameWithoutExtension(filePath);
                return new FileRunner(command, $"run {Path.GetFileName(filePath)} command.",ProcessFile(filePath), filePath);
            }
            return null;
        }

        private Func<IMessageContext, IEnumerable<string>> ProcessFile(string filePath)
        {
            return (context) =>
            {
                ExecuteCommand(context, filePath);
                return new string[0];
            };
        }

        #endregion

        void ExecuteCommand(IMessageContext context, string command)
        {
            var commandLine = $"powershell -NoProfile -ExecutionPolicy Bypass -Command \"& '{command}' {GetCommandArguments(context.Text)}\"";
            Console.Out.WriteLine("");
            RunCommand(context, "cmd.exe", "/c " + commandLine);
        }
    }
}