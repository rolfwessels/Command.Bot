using System;
using System.Collections.Generic;
using System.IO;

namespace Command.Bot.Core.Runner
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
                return new FileRunner(command, string.Format("run {0} command.", Path.GetFileName(filePath)),ProcessFile(filePath), filePath);
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
            var commandLine = "powershell -NoProfile -ExecutionPolicy Bypass -Command \"& '"+command+"'\"";
            RunCommand(context, commandLine);
        }
    }
}