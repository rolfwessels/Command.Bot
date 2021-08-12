using System.Collections.Generic;
using System.Linq;
using Command.Bot.Shared.Components.Runner;

namespace Command.Bot.Shared.Components.Responder
{
    public static class MatchRunnersHelper
    {
        public static FileRunner Find(this IEnumerable<FileRunner> fileRunners,
            string cleanMessage)
        {
            return fileRunners.FirstOrDefault(x => x.MatchesString(cleanMessage));
        }
    }
}