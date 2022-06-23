using System.Diagnostics;
using System.Globalization;
using System.Linq;
using Command.Bot.Core.SlackIntegration.Contracts;
using Command.Bot.Core.Utils;

namespace Command.Bot.Shared.Components.Runner
{
    public class BaseCommandLineRunner
    {
        protected void RunCommand(IMessageContext context, string fileName, string arguments)
        {
            var processInfo = new ProcessStartInfo(fileName, arguments)
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
            };

            Process process = Process.Start(processInfo);
            if (process != null)
            {
                process.OutputDataReceived += (sender, e) => { if (e.Data != null) context.SayOutput(e.Data); };
                process.BeginOutputReadLine();

                process.ErrorDataReceived += (sender, e) => { if (e.Data != null) context.SayError(e.Data); };
                process.BeginErrorReadLine();

                process.WaitForExit();
                int exitCode = process.ExitCode;
                if (exitCode != 0)
                {
                    context.SayError("ExitCode: " + exitCode.ToString(CultureInfo.InvariantCulture));
                }
                process.Close();
            }
        }

        protected virtual string CommandWithArguments(string contextText, string command)
        {
            var commandWithArguments = command + ' ' + GetCommandArguments(contextText);
            return commandWithArguments.Trim();
        }

        protected virtual string GetCommandArguments(string contextText)
        {
            return contextText?.Split(' ').Skip(1).StringJoin(" ");
        }
    }
}