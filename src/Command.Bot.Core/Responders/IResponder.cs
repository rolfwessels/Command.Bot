using System.Threading.Tasks;
using SlackConnector.Models;

namespace Command.Bot.Core.Responders
{
    public interface IResponder
    {
        bool CanRespond(MessageContext.MessageContext context);
        Task GetResponse(MessageContext.MessageContext context);
    }
}