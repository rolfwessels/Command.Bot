using MargieBot.Models;

namespace Command.Bot.Core
{
    public class RemoveInstructions : ResponderBase, IResponderDescription
    {
        #region Overrides of ResponderBase

        public override bool CanRespond(ResponseContext context)
        {
            return base.CanRespond(context) && MessageContains(context,Command);
        }

        #endregion

        #region Overrides of ResponderBase

        public override BotMessage GetResponse(ResponseContext context)
        {
            return new BotMessage() { Text = "Remove" };
        }

        #endregion

        #region Implementation of IResponderDescription

        public string Command {
            get { return "remove"; }
        }

        public string Description
        {
            get { return "remove a command"; }
        }
    }

        #endregion
    
}