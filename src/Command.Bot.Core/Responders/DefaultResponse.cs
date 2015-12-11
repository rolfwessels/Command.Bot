using SlackConnector.Models;

namespace Command.Bot.Core.Responders
{
    internal class DefaultResponse : ResponderBase
    {
        #region Implementation of IResponder

        public override BotMessage GetResponse(MessageContext context)
        {
            return new BotMessage() { Text = "Sorry I don't know that command. Type *help* for command information." };
        }

        #endregion
    }

    
}