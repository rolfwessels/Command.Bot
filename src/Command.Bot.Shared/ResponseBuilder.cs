using System.Collections.Generic;
using Command.Bot.Core;
using Command.Bot.Core.Responders;
using Command.Bot.Shared.Components.Responder;

namespace Command.Bot.Shared
{
    public class ResponseBuilder : IResponseBuilder
    {
        private readonly List<IResponder> _responders;
        
        public ResponseBuilder(string[] allowedUsers, string defaultScriptsPath)
        {
            _responders = new List<IResponder> {
                new AuthResponder(allowedUsers),
                new RunResponder(defaultScriptsPath)
            };
            _responders.Add(new HelpResponder(_responders));
            _responders.Add(new DefaultResponse(_responders)); 
        }

        public List<IResponder> GetResponders()
        {
            
            return _responders;
        }
    }
}