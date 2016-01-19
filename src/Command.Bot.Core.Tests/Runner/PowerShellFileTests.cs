using System.Reflection;
using Command.Bot.Core.Runner;
using FluentAssertions;
using log4net;
using NUnit.Framework;

namespace Command.Bot.Core.Tests.Runner
{
    [TestFixture]
    public class PowerShellFileTests
    {
        private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private PowerShellFile _powerShellFile;

        #region Setup/Teardown

        public void Setup()
        {
            _powerShellFile = new PowerShellFile();
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
            _log.Info("data");
            // arrange
            Setup();
            var fakeContext = new FakeContext();
            var fileRunner = _powerShellFile.GetRunner(@"Samples\psExample.ps1");
            // action
            fileRunner.Execute(fakeContext);
            // assert
            fakeContext.Response.Should().Contain(x => x.StartsWith("INFO:Hello ")).And.HaveCount(1);

        }




    }
}