using System.Reflection;
using Command.Bot.Shared.Components.Runner;
using FluentAssertions;
using Serilog;
using NUnit.Framework;

namespace Command.Bot.Core.Tests.Runner
{
    [TestFixture]
    [Category("windows-only")]
    public class BatchFileTests : TestsBase
    {

        private BatchFile _batFile;

        #region Setup/Teardown

        private void Setup()
        {
            _batFile = new BatchFile();
            RiderFixForCurrentPathIssue();
        }

        #endregion

        [Test]
        public void Constructor_WhenCalled_ShouldNotBeNull()
        {
            // arrange
            Setup();
            // assert
            _batFile.Should().NotBeNull();
        }

        [Test]
        public void Extension_WhenCalled_ShouldReturnPs1()
        {
            // arrange
            Setup();
            // action
            var extension = _batFile.Extension;
            // assert
            extension.Should().Be(".bat");
        }

        [Test]
        public void GetRunner_GivenFileName_ShouldReturnRunner()
        {
            // arrange
            Setup();
            var fakeContext = new FakeContext();


            var fileRunner = _batFile.GetRunner(@"Samples\batExample.bat");
            // action
            fileRunner.Execute(fakeContext);
            // assert
            fakeContext.Response.Should().Contain("INFO:i am a bat file");
            fakeContext.Response.Should().Contain("INFO:hello");
        }



        [Test]
        public void GetRunner_GivenArgument_ShouldPassTheArgument()
        {
            // arrange
            Setup();
            var fakeContext = new FakeContext() { Text = "batExample.bat --test" };


            var fileRunner = _batFile.GetRunner(@"Samples\batExample.bat");
            // action
            fileRunner.Execute(fakeContext);
            // assert
            fakeContext.Response.Should().Contain("INFO:Your first argument was '--test'");
        }




    }
}