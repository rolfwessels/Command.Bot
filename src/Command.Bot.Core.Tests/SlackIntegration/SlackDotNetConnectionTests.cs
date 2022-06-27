using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Command.Bot.Core.SlackIntegration;
using Command.Bot.Core.SlackIntegration.Contracts;
using Command.Bot.Shared;
using FluentAssertions;
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
            var slackConnectionHandler = new Reader();
            await slackDotNetConnection.Start(slackConnectionHandler);
            await slackDotNetConnection.SayToUser(Settings.Default.SplitTheAllowedUsers().First(), "You have 10 seconds to type hi!");
            await slackConnectionHandler.Wait();
            // assert
            slackConnectionHandler.Received.Select(x=>x.Detail.Text).Should().Contain("hi");
            await slackConnectionHandler.Received.First().Reply(new ReplyMessage("Success"));
        }
        public class Reader : ISlackConnectionHandler
        {
            private TaskCompletionSource<bool> _taskCompletionSource;

            public Reader()
            {
                _taskCompletionSource = new TaskCompletionSource<bool>();
            }

            #region Implementation of ISlackConnectionHandler

            public Task MessageReceived(ISlackRequest request)
            {
                Received.Add(request);
                _taskCompletionSource.SetResult(true);
                return Task.CompletedTask;
            }

            #endregion

            public async Task Wait()
            {
                await Task.WhenAny(Task.Delay(10000), _taskCompletionSource.Task);
            }

            public List<ISlackRequest> Received { get; } = new List<ISlackRequest>();
        }

        
    }

    
}