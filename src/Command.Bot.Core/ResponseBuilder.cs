using System.Collections.Generic;
using System.Linq;
using Command.Bot.Core.Responders;

namespace Command.Bot.Core
{
    public class ResponseBuilder
    {
        private static readonly List<IResponder> _authorizedResponders = new List<IResponder> {
//            new RemoveInstructions(), // security issues 
//            new UploadResponder(), // security issues 
            new RunResponder()
        };

        public static List<IResponder> GetResponders()
        {
            var responders = new List<IResponder>();
            responders.Add(new AuthResponder());
            responders.Add(new HelpResponder(_authorizedResponders));
            responders.AddRange(_authorizedResponders);
            responders.Add(new DefaultResponse());
            return responders;
        }
    }
}