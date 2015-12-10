using MargieBot.Models;
using MargieBot.Responders;

namespace Command.Bot.Core
{
    public abstract class ResponderBase : IResponder
    {
        public virtual bool CanRespond(ResponseContext context)
        {
            return IsForBot(context);
        }

        public abstract BotMessage GetResponse(ResponseContext context);

        protected static bool IsForBot(ResponseContext context)
        {
            return context.Message.User.IsSlackbot == false && context.BotHasResponded == false && (context.Message.MentionsBot || context.Message.ChatHub.Type == SlackChatHubType.DM);
        }

        protected static bool MessageContains(ResponseContext context, string value)
        {
            return context.Message.Text.ToLower().Contains(value.ToLower());
        }
    }
}