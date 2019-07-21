using System.Threading.Tasks;
using Command.Bot.Core;
using Serilog;

namespace Command.Bot.Service.Commands
{
    public class ServiceCommand : ServiceCommandBase
    {
        private SlackService _slackService;

        public ServiceCommand()
            : base("Command.BotService")
        {
            IsCommand("service", "Bot running as service");
        }

        #region Overrides of ServiceCommandBase

        protected override void StartService()
        {
            Log.Information($"Starting service {Settings.Default.BotKey.Substring(1, 5)}");
            _slackService = new SlackService(Settings.Default.BotKey);
            _slackService.Connect().ContinueWith(Connected);
        }

        private void Connected(Task obj)
        {
            if (obj.Exception != null) Log.Error(obj.Exception.Message, obj.Exception);
        }

        protected override void StopService()
        {
            Log.Information("Stop the service");
        }

        #endregion
    }
}