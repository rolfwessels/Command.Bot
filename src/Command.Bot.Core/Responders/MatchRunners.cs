using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Command.Bot.Core.Utils;

namespace Command.Bot.Core.Responders
{
    public static class MatchRunners
    {

        public static IEnumerable<IResponderDescription> GetCommands(this IEnumerable<IResponder> responders)
        {
            foreach (var responderDescription in responders)
            {
                if (responderDescription is IResponderDescription description)
                    yield return description;
                else if (responderDescription is IResponderDescriptions descriptions)
                    foreach (var desc in descriptions.Descriptions)
                    {
                        yield return desc;;
                    }
            }
        }
        public static IEnumerable<string> FindWithSimilarNames(this IEnumerable<IResponderDescription> runners, string message, int max = 3)
        {
            var source2 = message.Trim().ToLower();
            var enumerable = runners
                .Select(x=>x.Command)
                .Select(name=> new {command = name, matchQuality = LevenshteinDistance.Calculate(name.Trim().ToLower(), source2)});
           
            return enumerable
                .Where(x=>x.matchQuality < 6)
                .OrderBy(x=>x.matchQuality)
                .Select(x=>x.command)
                .Take(max);
        }
    }
}