using System.Text.RegularExpressions;
using Command.Bot.Core.SlackIntegration.Contracts;

namespace Command.Bot.Core.Responders
{
    public static class MessageContextHelper
    {
       

        public static bool MessageContains(this IMessageContext context, string value)
        {
            return context.Text.Contains(value.ToLower());
        }

        public static bool HasAttachment(this IMessageContext context)
        {
            return MessageContains(context, "upload");
        }

        public static bool HasMessage(this IMessageContext context, string text)
        {
            return context.Text.Equals(text.ToLower());
        }

        public static bool StartsWith(this IMessageContext context, string text)
        {
            return context.Text.StartsWith(text.ToLower());
        }

        public static bool StartsWith(this IMessageContext context, string text, out string result)
        {
            var cleanMessage = context.Text;
            var startsWith = cleanMessage.StartsWith(text.ToLower());
            result = null;
            if (startsWith)
            {
                result = cleanMessage.Substring(text.Length).Trim();
            }
            return startsWith;
        }
    }
}