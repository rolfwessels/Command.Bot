using System.Diagnostics;
using System.Globalization;

namespace Command.Bot.Core.Runner
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
    }
}