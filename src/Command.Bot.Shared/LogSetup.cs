using Command.Bot.Core;
using Serilog;

namespace Command.Bot.Shared
{
    public class LogSetup
    {
        public static LoggerConfiguration Default()
        {
            return new LoggerConfiguration()
                .ReadFrom.Configuration(BaseSettings.Config);
        }
    }
}