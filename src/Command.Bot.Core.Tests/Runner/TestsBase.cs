using System.IO;

namespace Command.Bot.Core.Tests.Runner
{
    public class TestsBase
    {
        private static bool _changed;

        protected void RiderFixForCurrentPathIssue()
        {
            if (_changed) return;
            Directory.SetCurrentDirectory(Path.GetDirectoryName(this.GetType().Assembly.Location) ?? "");
            _changed = true;
        }
    }
}