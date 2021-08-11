using System.Threading.Tasks;
using SlackConnector.Models;

namespace Command.Bot.Core.Responders
{
    public abstract class ResponderBase : IResponder
    {
        public virtual bool CanRespond(MessageContext context)
        {
            return MessageContextHelper.IsForBot(context);
        }

        public abstract Task GetResponse(MessageContext context);

       
    }

    
}