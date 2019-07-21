using System;
using System.Linq;
using System.Reflection;
using Command.Bot.Service.Helpers;
using Command.Bot.Shared;
using Serilog;

namespace Command.Bot.Service.Commands
{
    public abstract class ServiceCommandBase : CommandBase
    {
        private const string AsServiceArgument = "executeAsService";

        private bool _serviceInstall;
        private bool _serviceIsInstalled;
        private string _serviceName;
        private bool _serviceRemove;
        private bool _serviceRun;
        private bool _serviceStart;
        private bool _serviceStatus;
        private bool _serviceStop;

        protected ServiceCommandBase(string serviceName)
        {
            _serviceName = serviceName;

            AddArguments("run", "Run the service in command line", () => _serviceRun = true);
            AddArguments("install", "Install the service", () => _serviceInstall = true);
            AddArguments("uninstall", "remove the service service", () => _serviceRemove = true);
            AddArguments("status", "Status of the service", () => _serviceStatus = true);
            AddArguments("start", "Start the service", () => _serviceStart = true);
            AddArguments("stop", "Stop the service", () => _serviceStop = true);
            AddArguments("isinstalled", "Is the service installed", () => _serviceIsInstalled = true);
            HasOption<string>("servicename=", $"optional service name [{_serviceName}].",b => _serviceName = b ?? _serviceName);
            HasAdditionalArguments(1, GetArgumentHelpText());
        }

        #region Overrides of CommandBase

        protected override void RunCommand(string[] remainingArguments)
        {
            if (_serviceRun)
            {
                StartService();
                Console.Out.WriteLine("Press any key to stop.");
                Console.ReadKey();
                StopService();
            }

            if (_serviceInstall) Install();
            if (_serviceRemove) RemoveService();
            if (_serviceStatus) Status();
            if (_serviceStart) Start();
            if (_serviceStop) Stop();
            if (_serviceIsInstalled) IsInstalled();
            if (remainingArguments.Contains(AsServiceArgument)) throw new NotImplementedException();
        }

        #endregion

        #region Private Methods

        private void IsInstalled()
        {
            var serviceIsInstalled = ServiceInstaller.ServiceIsInstalled(_serviceName);
            Console.Out.WriteLine("Service {1} is installed : {0}.", serviceIsInstalled, _serviceName);
        }

        private void Stop()
        {
            Console.Out.WriteLine("Stop service {0}.", _serviceName);
            ServiceInstaller.StopService(_serviceName);
        }

        private void Start()
        {
            Console.Out.WriteLine("Start service {0}.", _serviceName);
            ServiceInstaller.StartService(_serviceName);
        }

        private void Status()
        {
            Console.Out.WriteLine("Service status of {0}.", _serviceName);
            ServiceInstaller.GetServiceStatus(_serviceName);
        }

        private void RemoveService()
        {
            Console.Out.WriteLine("Remove service {0}.", _serviceName);
            ServiceInstaller.Uninstall(_serviceName);
        }

        private void Install()
        {
            Console.Out.WriteLine("Installing service {0}", _serviceName);
            var location = Assembly.GetExecutingAssembly().Location;
            Log.Debug($"ServiceConsoleCommand:RunCommand {location}");
            ServiceInstaller.InstallAndStart(_serviceName, null, $@"{location} {AsServiceArgument}");
            Console.Out.WriteLine("Service {0} is now installed.", _serviceName);
        }

        #endregion

        #region Nested type: Service

        protected class Service : servi
        {
            private readonly Action _startService;
            private readonly Action _stopService;


            public Service(string serviceName, Action startService, Action stopService)
            {
                _startService = startService;
                _stopService = stopService;
                ServiceName = serviceName;
            }

            public string ServiceName { get; set; }

            protected void OnStart(string[] args)
            {
                try
                {
                    Log.Information("Start service");
                    _startService();
                    Log.Information("Services started");
                }
                catch (Exception e)
                {
                    Log.Error(e.Message, e);
                    throw;
                }
            }

            protected void OnStop()
            {
                try
                {
                    Log.Information("Stop service");
                    _stopService();
                    Log.Information("Services stopped");
                }
                catch (Exception e)
                {
                    Log.Error(e.Message, e);
                    throw;
                }
            }
        }

        #endregion

        #region Abstract methods

        protected abstract void StartService();
        protected abstract void StopService();

        #endregion
    }
}