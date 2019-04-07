using System.Linq;
using Command.Bot.Core.Responders;
using Command.Bot.Core.Runner;
using Command.Bot.Core.Tests.Runner;
using FizzWare.NBuilder;
using FluentAssertions;
using NUnit.Framework;

namespace Command.Bot.Core.Tests.Responders
{
    [TestFixture]
    public class MatchRunnersTests : TestsBase
    {
        [Test]
        public void Find_GivenNoMatchingRunner_ShouldReturnEmptyList()
        {
            // arrange
            var runners = new[] {new FileRunner("nope", "Runs stuff", context => new[] {"Cool"}, "file.exe")};
            // action
            var fileRunners = MatchRunners.Find(runners, "test");
            // assert
            fileRunners.Should().BeNull();
        }

        
        [Test]
        public void Find_GivenValidMatches_ShouldReturnThoseMatches()
        {
            // arrange
            var runners = new[] {
                new FileRunner("find", "Runs stuff", context => new[] {"Cool"}, "file.exe"),
                new FileRunner("notFind", "Runs stuff", context => new[] {"Cool"}, "file.exe"),
            };
            // action
            var fileRunners = MatchRunners.Find(runners, "find");
            // assert
            
            fileRunners.Command.Should().Be("find");
        }
        
        [Test]
        public void Find_GivenSimilarMatches_ShouldReturnOnlyMatchingOne()
        {
            // arrange
            var runners = new[] {
                new FileRunner("find", "Runs stuff", context => new[] {"Cool"}, "file.exe"),
                new FileRunner("notFind", "Runs stuff", context => new[] {"Cool"}, "file.exe"),
            };
            // action
            var fileRunners = MatchRunners.Find(runners, "Find");
            // assert
            
            fileRunners.Command.Should().Be("find");
        }
         
        [Test]
        public void Find_GivenSimilarStartingMatches_ShouldReturnOnlyMatchingOne()
        {
            // arrange
            var runners = new[] {
                new FileRunner("test", "Runs stuff", context => new[] {"Cool"}, "file.exe"),
                new FileRunner("testWeb", "Runs stuff", context => new[] {"Cool"}, "file.exe"),
            };
            // action
            var fileRunners = MatchRunners.Find(runners, "Testweb");
            // assert
            fileRunners.Command.Should().Be("testWeb");
        }
    }
}