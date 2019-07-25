using Command.Bot.Core;
using Serilog;

namespace Command.Bot.Shared
{
    public class LogSetup
    {
        public static LoggerConfiguration Default()
        {
            return new LoggerConfiguration()
//                .ReadFrom.Configuration(BaseSettings.Config)
                .MinimumLevel.Debug()
                .WriteTo.File("c:\\temp\\logs\\command.bot.log", rollOnFileSizeLimit: true, fileSizeLimitBytes: int.MaxValue,
                    retainedFileCountLimit: 2);
        }
    }
}