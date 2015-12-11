using System.Collections.Generic;
using System.Linq;
using Command.Bot.Core.Responders;

namespace Command.Bot.Core
{
    public class ResponseBuilder
    {
        public static List<IResponder> GetResponders()
        {
            var responders = new List<IResponder>();
            var bots = new List<IResponder> {
                new RemoveInstructions(),
                new UploadResponder(),
                new RunResponder()
            };
            responders.Add(new AuthResponder());
            responders.Add(new HelpResponder(bots));
            responders.AddRange(bots);
            responders.Add(new DefaultResponse());

            return responders;
        }
    }
}