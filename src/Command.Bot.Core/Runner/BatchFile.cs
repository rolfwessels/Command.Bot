using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;

namespace Command.Bot.Core.Runner
{
    public class BatchFile : IRunner
    {
        #region Implementation of IRunner

        public string Extension {
            get { return ".bat"; }
        }

        public FileRunner GetRunner(string filePath)
        {
            if (this.IsExtensionMatch(filePath))
            {
                var command = Path.GetFileNameWithoutExtension(filePath);
                return new FileRunner(command, string.Format("run {0} command.", Path.GetFileName(filePath)),
                    ProcessFile(filePath), filePath);
            }
            return null;
        }

        private Func<MessageContext, IEnumerable<string>> ProcessFile(string filePath)
        {
            return (context) => {
                ExecuteCommand(context,filePath);
                return new string[0];
            };
        }

        #endregion

        static void ExecuteCommand(MessageContext context, string command)
        {
            var processInfo = new ProcessStartInfo("cmd.exe", "/c " + command)
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
            };
          
            Process process = Process.Start(processInfo);
            if (process != null)
            {
                process.OutputDataReceived += (sender, e) => context.SayOutput(e.Data);

                process.BeginOutputReadLine();

                process.ErrorDataReceived += (sender, e) => context.SayError(e.Data);
                process.BeginErrorReadLine();

                process.WaitForExit();
                int exitCode = process.ExitCode;
                if (exitCode != 0)
                {
                    context.Say("ExitCode: " + exitCode.ToString(CultureInfo.InvariantCulture));
                }
                process.Close();
            }
        }
    }
}