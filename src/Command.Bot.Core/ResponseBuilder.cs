using System;
using System.Collections.Generic;
using System.Linq;
using Command.Bot.Core.Responders;

namespace Command.Bot.Core
{
    public class ResponseBuilder : IResponseBuilder
    {
        private readonly List<IResponder> _responders;

        public ResponseBuilder() : this(AuthResponder.SplitTheAllowedUsers(), Settings.Default.ScriptsPath)
        {
        }

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