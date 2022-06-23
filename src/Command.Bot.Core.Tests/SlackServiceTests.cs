using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Command.Bot.Core.SlackIntegration;
using Command.Bot.Core.SlackIntegration.Contracts;
using Command.Bot.Core.Utils;
using Command.Bot.Shared;
using FluentAssertions;
using NUnit.Framework;

namespace Command.Bot.Core.Tests
{
    public class SlackServiceTests
    {
        private SlackService _slackService;

        [Test]
        public async Task ProcessMessage_GivenHi_ShouldRespond()
        {
            // arrange
            Setup();
            var slackConnection = new FakeSlackMessage();
            // action
            await _slackService.ProcessMessage(new MessageContext(Message("hi", slackConnection)));
            // assert
            slackConnection.Said.Select(x => x.Text).Should().Contain("Could not find command `hi`. Did you mean to run *help*.");
        }


        [Test]
        public async Task ProcessMessage_GivenCommandThatIsNotEvenCloseToAnythign_ShouldRespondWithFailure()
        {
            // arrange
            Setup();
            var slackConnection = new FakeSlackMessage();
            // action
            await _slackService.ProcessMessage(new MessageContext(Message("hdfasdfasdi", slackConnection)));
            // assert
            slackConnection.Said.Select(x => x.Text).Should().Contain("Sorry I don't know that command. Type *help* for command information.");
        }

        [Test]
        public async Task ProcessMessage_GivenHelp_ShouldRespond()
        {
            // arrange
            Setup();
            var slackConnection = new FakeSlackMessage();
            // action
            await _slackService.ProcessMessage(new MessageContext(Message("Help", slackConnection)));
            // assert
            slackConnection.Said.Select(x => x.Text).First().Should().Contain("You are currently connected");
        }

        [Test]
        public async Task ProcessMessage_GivenHelpShouldFindDescription_ShouldRespond()
        {
            // arrange
            Setup();
            var slackConnection = new FakeSlackMessage();
            // action
            await _slackService.ProcessMessage(new MessageContext(Message("Help", slackConnection)));
            // assert
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                slackConnection.Said.Select(x => x.Text).StringJoin("\n").Should().Contain("Runs examples");
                slackConnection.Said.Select(x => x.Text).StringJoin("\n").Should().Contain("Runs ps examples");
            }
            else
            {
                slackConnection.Said.Select(x => x.Text).StringJoin("\n").Should().Contain("Runs sh examples");
                
            }
        }
        [Test]
        public async Task ProcessMessage_GivenInvalidUser_ShouldRespond()
        {
            // arrange
            Setup();
            var slackConnection = new FakeSlackMessage();
            await _slackService.ProcessMessage(new MessageContext(Message("Help", slackConnection, "invalid-user")));
            // action
            slackConnection.Said.Should().HaveCount(1);
            // assert
            slackConnection.Said.Select(x => x.Text).First().Should().Contain("This is not the bot you are looking for");
        }

        [Test]
        public async Task ProcessMessage_GivenRunCommand_ShouldRespond()
        {
            // arrange
            Setup();
            var slackConnection = new FakeSlackMessage();
            // action
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                await _slackService.ProcessMessage(
                    new MessageContext(Message("batExample -test", slackConnection)));
                // assert
                slackConnection.Said.Select(x => x.Text).Should()
                    .Contain("```hello\ni am a bat file\nYour first argument was '-test'\nOr is it '-test' ?```");
            }
            else
            {
                await _slackService.ProcessMessage(
                    new MessageContext(Message("shExample -test", slackConnection)));
                await Task.Delay(1000);
                // assert

                slackConnection.Said.Select(x => x.Text).Where(x => !string.IsNullOrEmpty(x)).StringJoin().Should()
                    .Contain("Argument was -test");
            }
        }

        [Test]
        public async Task ProcessMessage_GivenRunCommand_ShouldSayWhenDone()
        {
            // arrange
            Setup();
            var slackConnection = new FakeSlackMessage();
            // action
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                await _slackService.ProcessMessage(
                    new MessageContext(Message("batExample", slackConnection)));
            }
            else
            {
                await _slackService.ProcessMessage(
                    new MessageContext(Message("shExample", slackConnection)));
                await Task.Delay(1000);
            }

            // assert
            slackConnection.Said.Select(x => x.Text).Should().Contain("Done.");
        }

        [Test]
        public async Task ProcessMessage_GivenRunCommandWithSimilarName_ShouldRespond()
        {
            // arrange
            Setup();
            var slackConnection = new FakeSlackMessage();
            // action
            await _slackService.ProcessMessage(new MessageContext(Message("batExample1", slackConnection)));
            // assert
            slackConnection.Said.Select(x => x.Text).FirstOrDefault().Should().Contain("Could not find command `batexample1`. Did you mean to run");
        }

        private static ISlackRequest Message(string text, FakeSlackMessage fakeSlackMessage, string allowedUser = "allowedUser" )
        {
            fakeSlackMessage.Detail = new FakeSlackMessage.DetailValue(text, allowedUser);
            return fakeSlackMessage;
            {

                // User = new SlackUser { Name = allowedUser, IsBot = false },
                // Text = text,
                // ChatHub = new SlackChatHub() { Type = SlackChatHubType.DM },
                // MentionsBot = true
            };
        }

        private void Setup()
        {
            _slackService = new SlackService(null, new ResponseBuilder(new[] { "allowedUser" }, "Samples"));
        }
    }

    internal class FakeSlackMessage : ISlackRequest
    {
        #region Implementation of ISlackRequest

        public ISlackDetail Detail { get; set; }
        public List<ReplyMessage> Said { get; set; } = new List<ReplyMessage>();

        public bool IsForBot()
        {
            return true;
        }

        public Task Reply(ReplyMessage message)
        {
            Said.Add(message);
            return Task.CompletedTask;
        }

        public Task IndicateTyping()
        {
            return Task.CompletedTask;
        }

        #endregion

        public class DetailValue : ISlackDetail
        {
            public DetailValue(string text, string allowedUser)
            {
                UserName = allowedUser;
                Text = text;
                UserId = allowedUser+"Id";
            }

            #region Implementation of ISlackDetail

            public string UserName { get; }
            public string Text { get; }
            public string UserId { get; }

            #endregion
        }
    }
}