using Serilog;

namespace Command.Bot.Shared
{
    public class LogSetup
    {
        public static LoggerConfiguration Default()
        {
            return new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.File("logs/command.bot.log", rollOnFileSizeLimit: true, fileSizeLimitBytes: 10000000,
                    retainedFileCountLimit: 2);
        }
    }
}