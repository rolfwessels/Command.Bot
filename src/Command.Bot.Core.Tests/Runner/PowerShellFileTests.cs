using System.Reflection;
using Command.Bot.Shared.Components.Runner;
using FluentAssertions;
using Serilog;
using NUnit.Framework;

namespace Command.Bot.Core.Tests.Runner
{
    [TestFixture]
    [Category("windows-only")]
    public class PowerShellFileTests : TestsBase
    {

        private PowerShellFile _powerShellFile;

        #region Setup/Teardown

        private void Setup()
        {
            _powerShellFile = new PowerShellFile();
            RiderFixForCurrentPathIssue();
        }

        #endregion

        [Test]
        public void Constructor_WhenCalled_ShouldNotBeNull()
        {
            // arrange
            Setup();
            // assert
            _powerShellFile.Should().NotBeNull();
        }

        [Test]
        public void Extension_WhenCalled_ShouldReturnPs1()
        {
            // arrange
            Setup();
            // action
            var extension = _powerShellFile.Extension;
            // assert
            extension.Should().Be(".ps1");
        }

        [Test]
        public void GetRunner_GivenFileName_ShouldReturnRunner()
        {
            Log.Information("data");
            // arrange
            Setup();
            var fakeContext = new FakeContext();
            var fileRunner = _powerShellFile.GetRunner(@"Samples\psExample.ps1");
            // action
            fileRunner.Execute(fakeContext);
            // assert
            fakeContext.Response.Should().Contain(x => x.StartsWith("INFO:Hello ")).And.HaveCount(1);

        }


        [Test]
        public void GetRunner_GivenTest_ShouldReturnRunner()
        {
            Log.Information("data");
            // arrange
            Setup();
            var fakeContext = new FakeContext();
            fakeContext.Text = "psExample test";
            var fileRunner = _powerShellFile.GetRunner(@"Samples\psExample.ps1");
            // action
            fileRunner.Execute(fakeContext);
            // assert
            fakeContext.Response.Should().Contain(x => x.EndsWith("! - $param1=test")).And.HaveCount(1);

        }




    }
}