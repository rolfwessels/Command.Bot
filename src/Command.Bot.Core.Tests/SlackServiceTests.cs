using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using SlackConnector;
using SlackConnector.EventHandlers;
using SlackConnector.Models;

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
            var slackConnection = new FakeConnection();
            // action
            await _slackService.ProcessMessage(new MessageContext(Message("hi"), slackConnection));
            // assert
            slackConnection.Said.Select(x=>x.Text).Should().Contain("Could not find command `hi`. Did you mean to run *help*.");
        }


        [Test]
        public async Task ProcessMessage_GivenCommandThatIsNotEvenCloseToAnythign_ShouldRespondWithFailure()
        {
            // arrange
            Setup();
            var slackConnection = new FakeConnection();
            // action
            await _slackService.ProcessMessage(new MessageContext(Message("hdfasdfasdi"), slackConnection));
            // assert
            slackConnection.Said.Select(x => x.Text).Should().Contain("Sorry I don't know that command. Type *help* for command information.");
        }

        [Test]
        public async Task ProcessMessage_GivenHelp_ShouldRespond()
        {
            // arrange
            Setup();
            var slackConnection = new FakeConnection();
            // action
            await _slackService.ProcessMessage(new MessageContext(Message("Help"), slackConnection));
            // assert
            slackConnection.Said.Select(x=>x.Text).First().Should().Contain("You are currently connected");
        }

        [Test]
        public async Task ProcessMessage_GivenInvalidUser_ShouldRespond()
        {
            // arrange
            Setup();
            var slackConnection = new FakeConnection();
            await _slackService.ProcessMessage(new MessageContext(Message("Help","invalid-user"), slackConnection));
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
            var slackConnection = new FakeConnection();
            // action
            await _slackService.ProcessMessage(new MessageContext(Message("batExample -test"), slackConnection));
            // assert
            slackConnection.Said.Select(x => x.Text).Should().Contain("```hello\ni am a bat file\nYour first argument was '-test'\nOr is it '-test'```");
        }

        [Test]
        public async Task ProcessMessage_GivenRunCommand_ShouldSayWhenDone()
        {
            // arrange
            Setup();
            var slackConnection = new FakeConnection();
            // action
            await _slackService.ProcessMessage(new MessageContext(Message("batExample"), slackConnection));
            // assert
            slackConnection.Said.Select(x => x.Text).Should().Contain("Done.");
        }

        [Test]
        public async Task ProcessMessage_GivenRunCommandWithSimilarName_ShouldRespond()
        {
            // arrange
            Setup();
            var slackConnection = new FakeConnection();
            // action
            await _slackService.ProcessMessage(new MessageContext(Message("batExample1"), slackConnection));
            // assert
            slackConnection.Said.Select(x => x.Text).Should().Contain("Could not find command `batexample1`. Did you mean to run *batExample* or *psExample*.");
        }

        private static SlackMessage Message(string text, string allowedUser = "allowedUser")
        {
            return new SlackMessage()
                {User = new SlackUser {Name = allowedUser,IsBot = false},
                    Text = text,
                    ChatHub = new SlackChatHub() {Type = SlackChatHubType.DM},MentionsBot = true};
        }

        private void Setup()
        {
            _slackService = new SlackService(null,new ResponseBuilder(new []{"allowedUser"},"Samples"));
        }
    }

    public class FakeConnection : ISlackConnection
    {
        #region Implementation of ISlackConnection

        public Task Close()
        {
            throw new NotImplementedException();
        }

        public Task Say(BotMessage message)
        {
            Said.Add(message);
            return Task.CompletedTask;
        }

        public Task Upload(SlackChatHub chatHub, string filePath)
        {
            throw new NotImplementedException();
        }

        public Task Upload(SlackChatHub chatHub, Stream stream, string fileName)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<SlackChatHub>> GetChannels()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<SlackUser>> GetUsers()
        {
            throw new NotImplementedException();
        }

        public Task<SlackChatHub> JoinDirectMessageChannel(string user)
        {
            throw new NotImplementedException();
        }

        public Task<SlackChatHub> JoinChannel(string channelName)
        {
            throw new NotImplementedException();
        }

        public Task<SlackChatHub> CreateChannel(string channelName)
        {
            throw new NotImplementedException();
        }

        public Task ArchiveChannel(string channelName)
        {
            throw new NotImplementedException();
        }

        public Task<SlackPurpose> SetChannelPurpose(string channelName, string purpose)
        {
            throw new NotImplementedException();
        }

        public Task<SlackTopic> SetChannelTopic(string channelName, string topic)
        {
            throw new NotImplementedException();
        }

        public Task IndicateTyping(SlackChatHub chatHub)
        {
            throw new NotImplementedException();
        }

        public Task Ping()
        {
            throw new NotImplementedException();
        }

        public Task<Stream> DownloadFile(Uri downloadUri)
        {
            throw new NotImplementedException();
        }

        public IReadOnlyDictionary<string, SlackChatHub> ConnectedHubs { get; }
        public IReadOnlyDictionary<string, SlackUser> UserCache { get; }
        public bool IsConnected { get; }
        public DateTime? ConnectedSince { get; }
        public string SlackKey { get; }
        public ContactDetails Team { get; }
        public ContactDetails Self { get; }
        public List<BotMessage> Said { get;  } = new List<BotMessage>();

        public event DisconnectEventHandler OnDisconnect;
        public event ReconnectEventHandler OnReconnecting;
        public event ReconnectEventHandler OnReconnect;
        public event MessageReceivedEventHandler OnMessageReceived;
        public event ReactionReceivedEventHandler OnReaction;
        public event ChatHubJoinedEventHandler OnChatHubJoined;
        public event UserJoinedEventHandler OnUserJoined;
        public event PongEventHandler OnPong;
        public event ChannelCreatedHandler OnChannelCreated;

        #endregion
    }
}