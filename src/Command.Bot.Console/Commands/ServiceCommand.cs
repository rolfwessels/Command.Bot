using System.Reflection;
using Command.Bot.Core;
using log4net;

namespace Command.Bot.Console.Commands
{
	public class ServiceCommand : ServiceCommandBase
	{
		private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
	    private SlackConnector _slackConnector;

	    public ServiceCommand()
			: base("Command.BotService")
		{
			IsCommand("service", string.Format("Bot running as service"));
		}

		#region Overrides of ServiceCommandBase

		protected override void StartService()
		{
            _log.Info("Starting service");
		    _slackConnector = new SlackConnector();
            _slackConnector.Connect();
		}

		protected override void StopService()
		{
            _log.Info("Stop the service");
		}

		#endregion

		
	}
	
}