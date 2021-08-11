using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Command.Bot.Core.Runner;
using Command.Bot.Core.Utils;

namespace Command.Bot.Core.Responders
{
    internal class DefaultResponse : ResponderBase
    {
        private readonly List<IResponder> _runResponder;

        public DefaultResponse(List<IResponder> runResponder)
        {
            _runResponder = runResponder;
        }


        #region Implementation of IResponder

        public override async Task GetResponse(MessageContext context)
        {
            var hasSimilar = _runResponder.GetCommands().FindWithSimilarNames(context.CleanMessage()).ToArray();
            if (hasSimilar.Any())
            {
                var stringJoinAnd = hasSimilar
                    .Select(x => $"*{x}*")
                    .StringJoinAnd(" or ");
                await context.Say(
                    $"Could not find command `{context.CleanMessage()}`. Did you mean to run {stringJoinAnd}.");
            }
            else
            {
                await context.Say("Sorry I don't know that command. Type *help* for command information.");
            }
        }

        #endregion
    }

    
}