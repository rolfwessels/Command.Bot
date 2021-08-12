using System;
using System.Threading.Tasks;
using SlackConnector.Models;

namespace Command.Bot.Core.Responders
{
    public abstract class ResponderBase : IResponder
    {
        public virtual bool CanRespond(MessageContext.MessageContext context)
        {
            return context.IsForBot();
        }

        public abstract Task GetResponse(MessageContext.MessageContext context);

       
    }

    
}