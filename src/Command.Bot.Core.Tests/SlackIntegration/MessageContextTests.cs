using System.Linq;
using System.Threading.Tasks;
using Command.Bot.Core.SlackIntegration;
using Command.Bot.Core.SlackIntegration.Contracts;
using FluentAssertions;
using NUnit.Framework;

namespace Command.Bot.Core.Tests.SlackIntegration
{
    public class MessageContextTests
    {
        [Test]
        [TestCase("test http://test.com ", "test <http://test.com> ")]
        [TestCase("test https://test.com", "test <https://test.com>")]
        [TestCase("test www.test.com", "test <www.test.com>")]
        [TestCase("www.test.com www.test2.com more", "<www.test.com> <www.test2.com> more")]
        public async Task Reply_GivenLinkInMessage_ShouldMarkAsMDLink(string input, string expected)
        {
            // arrange
            var fakeSlackMessage = new FakeSlackMessage();
            var messageContext = new MessageContext(fakeSlackMessage);
            // action
            await messageContext.Reply(input);
            // assert
            fakeSlackMessage.Said.First().Text.Should().Be(expected);
        }


    }
}