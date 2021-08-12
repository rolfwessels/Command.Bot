using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Command.Bot.Core.SlackIntegration;
using Command.Bot.Shared;
using NUnit.Framework;
using Serilog;

namespace Command.Bot.Core.Tests.SlackIntegration
{
    public class SlackDotNetConnectionTests
    {
        [Test]
        [Explicit]
        public async Task Start_GivenSimpleRequest_ShouldConnectAndPostMessageToUser()
        {
            // arrange
            if (!Settings.Default.BotKey.StartsWith("xox") && Settings.Default.SplitTheAllowedUsers().Length <= 0) return;
            Log.Logger =  LogSetup.Default()
                .WriteTo.Console()
                .CreateLogger();
            var slackDotNetConnection = new SlackDotNetConnection(Settings.Default.BotKey);    
            // action
            await slackDotNetConnection.Start();
            await slackDotNetConnection.SayToUser(Settings.Default.SplitTheAllowedUsers().First(), "hello I am alive!");
            await Task.Delay(10000);
            // assert
        }


    }
}