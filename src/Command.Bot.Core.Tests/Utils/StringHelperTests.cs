using System.Linq;
using Command.Bot.Core.Utils;
using FluentAssertions;
using NUnit.Framework;

namespace Command.Bot.Core.Tests.Utils
{
    [TestFixture]
    public class StringHelperTests
    {
        [Test]
        public void GroupByMaxLength_GivenValueUnderMax_ShouldGroupIntoOne()
        {
            // arrange
            var sut = new string[] { "test1", "test2" };
            // action
            var result = sut.GroupByMaxLength(50).ToList();
            // assert
            result.Should().HaveCount(1);
            result.FirstOrDefault().Should().ContainInOrder(sut);
        }

        [Test]
        public void GroupByMaxLength_GivenOneValueOverMax_ShouldStillReturnIt()
        {
            // arrange
            var sut = new string[] { "test1", "test2" };
            // action
            var result = sut.GroupByMaxLength(1).ToList();
            // assert
            result.Should().HaveCount(2);
            result.FirstOrDefault().Should().ContainInOrder(sut.Take(1));
        }

        [Test]
        public void GroupByMaxLength_GivenValueOverMax_ShouldGroupIntoTwo()
        {
            // arrange
            var sut = new string[] { "test1","test2","test3" };
            // action
            var result = sut.GroupByMaxLength(10);
            // assert
            result.Should().HaveCount(2);
        }

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