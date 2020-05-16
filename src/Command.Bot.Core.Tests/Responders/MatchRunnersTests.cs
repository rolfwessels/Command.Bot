using System.Linq;
using Command.Bot.Core.Responders;
using Command.Bot.Core.Runner;
using FluentAssertions;
using NUnit.Framework;

namespace Command.Bot.Core.Tests.Responders
{
    [TestFixture]
    public class MatchRunnersTests 
    {
        [Test]
        public void Find_GivenNoMatchingRunner_ShouldReturnEmptyList()
        {
            // arrange
            var runners = GetSampleRunners();
            // action
            var fileRunners = runners.Find("TestWen");
            // assert
            fileRunners.Should().BeNull();
        }

        [Test]
        public void Find_GivenValidMatches_ShouldReturnThoseMatches()
        {
            // arrange
            var runners = GetSampleRunners();
            // action
            var fileRunners = runners.Find("test");
            // assert
            
            fileRunners.Command.Should().Be("test");
        }
        
        [Test]
        public void Find_GivenSimilarMatches_ShouldReturnOnlyMatchingOne()
        {
            // arrange
            var runners = GetSampleRunners();
            // action
            var fileRunners = runners.Find("TEST");
            // assert
            
            fileRunners.Command.Should().Be("test");
        }
          
        [Test]
        public void Find_GivenMatchWithArguments_ShouldReturnOnlyMatchingOne()
        {
            // arrange
            var runners = GetSampleRunners();
            // action
            var fileRunners = runners.Find("TEST --test");
            // assert
            
            fileRunners.Command.Should().Be("test");
        }
         
        [Test]
        public void Find_GivenSimilarStartingMatches_ShouldReturnOnlyMatchingOne()
        {
            // arrange
            var runners = GetSampleRunners();
            // action
            var fileRunners = runners.Find("Testweb");
            // assert
            fileRunners.Command.Should().Be("testWeb");
        }
         
        [Test]
        public void Find_GivenMatchesMatchesThatAreSimilar_ShouldReturnOnlyMatchingOne()
        {
            // arrange
            var runners = GetSampleRunners();
            // action
            var fileRunners = runners.FindWithSimilarNames("TestWen");
            // assert
            fileRunners.Should().HaveCount(2);
        }

        [Test]
        public void Find_GivenMatchesMatchesThatAreSimilar_ShouldReturnTheClosesMatchFirst()
        {
            // arrange
            var runners = GetSampleRunners();
            // action
            var fileRunners = runners.FindWithSimilarNames("TestWen");
            // assert
            fileRunners.First().Should().Be("testWeb");
        }



        private static FileRunner[] GetSampleRunners()
        {
            var runners = new[]
            {
                new FileRunner("test", "Runs stuff", context => new[] {"Cool"}, "file.exe"),
                new FileRunner("testWeb", "Runs stuff", context => new[] {"Cool"}, "file.exe"),
                new FileRunner("testWebtestWebtestWebtestWeb", "Runs stuff", context => new[] {"Cool"}, "file.exe"),
            };
            return runners;
        }
    }
}