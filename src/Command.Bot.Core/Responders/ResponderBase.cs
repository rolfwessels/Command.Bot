using System;
using System.Threading.Tasks;
using Command.Bot.Core.SlackIntegration.Contracts;

namespace Command.Bot.Core.Responders
{
    public abstract class ResponderBase : IResponder
    {
        public virtual bool CanRespond(IMessageContext context)
        {
            return context.IsForBot();
        }

        public abstract Task GetResponse(IMessageContext context);

       
    }

    
}