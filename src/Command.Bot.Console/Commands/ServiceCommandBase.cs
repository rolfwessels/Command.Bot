using System;
using System.Reflection;
using System.ServiceProcess;
using System.Linq;
using log4net;
using ServiceInstaller = Command.Bot.Console.Helpers.ServiceInstaller;

namespace Command.Bot.Console.Commands
{
	public abstract class ServiceCommandBase : CommandBase
	{
		private const string AsServiceArgument = "executeAsService";
		private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		private bool _serviceInstall;
		private bool _serviceIsInstalled;
		private string _serviceName;
		private bool _serviceRun;
		private bool _serviceStart;
		private bool _serviceStatus;
		private bool _serviceStop;
		private bool _serviceRemove;
		
		protected ServiceCommandBase(string serviceName)
		{
			_serviceName = serviceName;
			
			AddArguments("run", "Run the service in command line", () =>  _serviceRun = true);
			AddArguments("install", "Install the service", () =>  _serviceInstall = true);
			AddArguments("uninstall", "remove the service service", () =>  _serviceRemove = true);
			AddArguments("status", "Status of the service", () =>  _serviceStatus = true);
			AddArguments("start", "Start the service", () =>  _serviceStart = true);
			AddArguments("stop", "Stop the service", () =>  _serviceStop = true);
			AddArguments("isinstalled", "Is the service installed", () =>  _serviceIsInstalled = true);
			HasOption("servicename=", string.Format("optional service name [{0}].", _serviceName),
			          b => _serviceName = b ?? _serviceName);
			HasAdditionalArguments(1,GetArgumentHelpText());
			
		}

		#region Overrides of CommandBase

		protected override void RunCommand(string[] remainingArguments)
		{
			if (_serviceRun)
			{
				StartService();
				System.Console.Out.WriteLine("Press any key to stop.");
				System.Console.ReadKey();
				StopService();
			}
			if (_serviceInstall)
			{
				Install();
			}
			if (_serviceRemove)
			{
				RemoveService();
			}
			if (_serviceStatus)
			{
				Status();
			}
			if (_serviceStart)
			{
				Start();
			}
			if (_serviceStop)
			{
				Stop();
			}
			if (_serviceIsInstalled)
			{
				IsInstalled();
			}
			if (remainingArguments.Contains(AsServiceArgument))
			{
				var servicesToRun = new ServiceBase[]
				{
					new Service(_serviceName,StartService,StopService), 
				};
				ServiceBase.Run(servicesToRun);
			}

		}

		#endregion

		#region Abstract methods

		protected abstract void StartService();
		protected abstract void StopService();

		#endregion

		#region Private Methods

		private void IsInstalled()
		{
			bool serviceIsInstalled = ServiceInstaller.ServiceIsInstalled(_serviceName);
			System.Console.Out.WriteLine("Service {1} is installed : {0}.", serviceIsInstalled, _serviceName);
		}

		private void Stop()
		{
			System.Console.Out.WriteLine("Stop service {0}.", _serviceName);
			ServiceInstaller.StopService(_serviceName);
		}

		private void Start()
		{
			System.Console.Out.WriteLine("Start service {0}.", _serviceName);
			ServiceInstaller.StartService(_serviceName);
		}

		private void Status()
		{
			System.Console.Out.WriteLine("Service status of {0}.", _serviceName);
			ServiceInstaller.GetServiceStatus(_serviceName);
		}

		private void RemoveService()
		{
			System.Console.Out.WriteLine("Remove service {0}.", _serviceName);
			ServiceInstaller.Uninstall(_serviceName);
		}

		private void Install()
		{
			System.Console.Out.WriteLine("Installing service {0}", _serviceName);
			string location = Assembly.GetExecutingAssembly().Location;
			_log.Debug(String.Format("ServiceConsoleCommand:RunCommand {0}", location));
			ServiceInstaller.InstallAndStart(_serviceName, null, String.Format(@"{0} {1}", location, AsServiceArgument));
			System.Console.Out.WriteLine("Service {0} is now installed.", _serviceName);
		}

		#endregion

		protected class Service : ServiceBase
		{
			private readonly Action _startService;
			private readonly Action _stopService;


			public Service(string serviceName, Action startService, Action stopService)
			{
				_startService = startService;
				_stopService = stopService;
				ServiceName = serviceName;
			}

			protected override void OnStart(string[] args)
			{
			  try
			  {
				_log.Info("Start service");
				_startService();
				_log.Info("Services started");
			  }
			  catch (Exception e)
			  {
				_log.Error(e.Message, e);
				throw;
			  }
			}

			protected override void OnStop()
			{
			  try
			  {
				_log.Info("Stop service");
				  _stopService();
				_log.Info("Services stopped");
			  }
			  catch (Exception e)
			  {
				_log.Error(e.Message, e);
				throw;
			  }
			}


		}
	}

	
}