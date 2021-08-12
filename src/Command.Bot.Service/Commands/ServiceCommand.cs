using System;
using System.Threading;
using System.Threading.Tasks;
using Command.Bot.Core;
using Command.Bot.Shared;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Command.Bot.Service.Commands
{
    public class ServiceCommand : ServiceCommandBase
    {
        public ServiceCommand()
            : base("Command.BotService")
        {
            IsCommand("service", "Bot running as service");
        }

        
        #region Overrides of ServiceCommandBase

        protected override void AddService(IServiceCollection services)
        {
            services.AddHostedService<SlackBackgroundService>();
        }

        #endregion
    }

    public class SlackBackgroundService : BackgroundService
    {
        private SlackService _slackService;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Log.Information($"Starting service {Settings.Default.BotKey.Substring(1, 5)}");
            _slackService = ServiceFactory.BuildDefaultSlackService();
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await _slackService.Connect();
                    break;
                }
                catch (Exception e)
                {
                    Log.Error(e,$"Could not connect: {e.Message}");
                }
            }
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000000, stoppingToken);
            }
        }
    }
}