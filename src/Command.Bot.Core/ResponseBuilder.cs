using System.Collections.Generic;
using System.Linq;
using Command.Bot.Core.Responders;

namespace Command.Bot.Core
{
    public class ResponseBuilder : IResponseBuilder
    {
        private readonly List<IResponder> _responders;

        public ResponseBuilder()
        {
            var authorizedResponders = new List<IResponder> {
                new RunResponder()
            };
            _responders = new List<IResponder> { new AuthResponder(), new HelpResponder(authorizedResponders) };
            _responders.AddRange(authorizedResponders);
            _responders.Add(new DefaultResponse());
        }

        public List<IResponder> GetResponders()
        {
            
            return _responders;
        }
    }
}