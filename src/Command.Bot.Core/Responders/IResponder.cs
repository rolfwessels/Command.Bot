using System.Threading.Tasks;
using SlackConnector.Models;

namespace Command.Bot.Core.Responders
{
    public interface IResponder
    {
        bool CanRespond(MessageContext context);
        Task GetResponse(MessageContext context);
    }
}