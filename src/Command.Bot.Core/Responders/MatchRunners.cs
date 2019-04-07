using System;
using System.Collections.Generic;
using System.Linq;
using Command.Bot.Core.Runner;

namespace Command.Bot.Core.Responders
{
    public static class MatchRunners
    {
        public static FileRunner Find(IEnumerable<FileRunner> fileRunners,
            string cleanMessage)
        {
          return fileRunners.FirstOrDefault(x => x.MatchesString(cleanMessage));
        }
        
    }
}