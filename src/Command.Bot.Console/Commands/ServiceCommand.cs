using System.Reflection;
using System.Threading.Tasks;
using Command.Bot.Core;
using Command.Bot.Core.Properties;
using log4net;

namespace Command.Bot.Console.Commands
{
	public class ServiceCommand : ServiceCommandBase
	{
		private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
	    private SlackService _slackService;

	    public ServiceCommand()
			: base("Command.BotService")
		{
			IsCommand("service", string.Format("Bot running as service"));
		}

		#region Overrides of ServiceCommandBase

		protected override void StartService()
		{
            _log.Info("Starting service");
            _slackService = new SlackService(Settings.Default.BotKey);
            
            _slackService.Connect().ContinueWith(Connected);
		}

	    private void Connected(Task obj)
	    {
	        if (obj.Exception != null)
	        {
                _log.Error(obj.Exception.Message, obj.Exception);
	        }
	    }

	    protected override void StopService()
		{
            _log.Info("Stop the service");
		}

		#endregion

		
	}
	
}