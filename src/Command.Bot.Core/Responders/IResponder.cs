using System.Threading.Tasks;
using Command.Bot.Core.SlackIntegration.Contracts;

namespace Command.Bot.Core.Responders
{
    public interface IResponder
    {
        
        bool CanRespond(IMessageContext context);
        Task GetResponse(IMessageContext context);
    }
}