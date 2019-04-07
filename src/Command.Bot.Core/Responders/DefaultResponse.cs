using System.Linq;
using Command.Bot.Core.Runner;
using SlackConnector.Models;

namespace Command.Bot.Core.Responders
{
    internal class DefaultResponse : ResponderBase
    {
        #region Implementation of IResponder

        public override BotMessage GetResponse(MessageContext context)
        {
            var hasSimilar = FileRunners.Scripts.FindWithSimilarNames(context.CleanMessage()).ToArray();
            if (hasSimilar.Any())
            {
                var stringJoinAnd = hasSimilar
                    .Select(x=> $"*{x}*")
                    .StringJoinAnd(" or ");
                return new BotMessage() { Text = $"Could not find command `{context.CleanMessage()}`. Did you mean to run {stringJoinAnd}."};
            }
            return new BotMessage() { Text = "Sorry I don't know that command. Type *help* for command information." };
        }

        #endregion
    }

    
}