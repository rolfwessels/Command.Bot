using SlackConnector.Models;

namespace Command.Bot.Core.Responders
{
    public interface IResponder
    {
        bool CanRespond(MessageContext context);
        BotMessage GetResponse(MessageContext context);
    }
}