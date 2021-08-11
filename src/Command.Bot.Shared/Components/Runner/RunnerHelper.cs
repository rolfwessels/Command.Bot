using System.IO;

namespace Command.Bot.Shared.Components.Runner
{
    public static class RunnerHelper
    {
        public static bool IsExtensionMatch(this IRunner runner, string name)
        {
            var extension = Path.GetExtension(name);
            extension = extension != null ? extension.ToLower() : null;
            return extension != null && runner.Extension == extension;
        }
    }
}