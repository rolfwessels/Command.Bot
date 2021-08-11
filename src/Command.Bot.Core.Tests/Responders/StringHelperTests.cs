using Command.Bot.Core.Responders;
using Command.Bot.Core.Utils;
using FluentAssertions;
using NUnit.Framework;

namespace Command.Bot.Core.Tests.Responders
{
    [TestFixture]
    public class StringHelperTests
    {
        [Test]
        public void StringJoinAnd_GivenNoValues_ShouldReturnEmptyString()
        {
            // arrange
            var sut = new string[] { };
            // action
            var result = sut.StringJoinAnd();
            // assert
            result.Should().Be(null);
        }
        
        [Test]
        public void StringJoinAnd_GivenOnlyValues_ShouldReturnValue()
        {
            // arrange
            var sut = new[] { "sample"};
            // action
            var result = sut.StringJoinAnd();
            // assert
            result.Should().Be("sample");
        }
        
        [Test]
        public void StringJoinAnd_GivenMultipleValues_ShouldAllValuesAnd()
        {
            // arrange
            var sut = new[] { "sample" , "sample" , "sample" };
            // action
            var result = sut.StringJoinAnd();
            // assert
            result.Should().Be("sample, sample and sample");
        }
        
        [Test]
        public void StringJoinAnd_GivenMultipleValuesAndOr_ShouldAllValuesOr()
        {
            // arrange
            var sut = new[] { "sample" , "sample" , "sample" };
            // action
            var result = sut.StringJoinAnd(" or ");
            // assert
            result.Should().Be("sample, sample or sample");
        }
    }
}