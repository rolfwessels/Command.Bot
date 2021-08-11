using System.Text.RegularExpressions;
using SlackConnector.Models;

namespace Command.Bot.Core.Responders
{
    public static class MessageContextHelper
    {
        public static bool IsForBot(this MessageContext context)
        {
            return !context.Message.User.IsBot && 
                   (context.Message.MentionsBot || context.Message.ChatHub.Type == SlackChatHubType.DM);
        }

        public static bool MessageContains(this MessageContext context, string value)
        {
            return CleanMessage(context).Contains(value.ToLower());
        }

        public static bool HasAttachment(this MessageContext context)
        {
            return MessageContains(context, "upload");
        }

        public static bool HasMessage(this MessageContext context, string text)
        {
            return CleanMessage(context).Equals(text.ToLower());
        }

        public static bool StartsWith(this MessageContext context, string text)
        {
            return CleanMessage(context).StartsWith(text.ToLower());
        }

        public static bool StartsWith(this MessageContext context, string text, out string result)
        {
            var cleanMessage = CleanMessage(context);
            var startsWith = cleanMessage.StartsWith(text.ToLower());
            result = null;
            if (startsWith)
            {
                result = cleanMessage.Substring(text.Length).Trim();
            }
            return startsWith;
        }

        public static string CleanMessage(this MessageContext context)
        {
            var text = context.Message.Text;
            if (text != null)
            {
                text = Regex.Replace(text, @"<@.*?>:", " ");
                text = Regex.Replace(text, @"<@.*?>", " ");
                return text.Trim().Trim('*').Trim().ToLower();
            }
            return null;
        }

       
    }
}