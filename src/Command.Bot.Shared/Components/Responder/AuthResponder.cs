using System.Linq;
using System.Threading.Tasks;
using Command.Bot.Core.Responders;
using Command.Bot.Core.SlackIntegration.Contracts;

namespace Command.Bot.Shared.Components.Responder
{
    public class AuthResponder : ResponderBase
    {
        private readonly string[] _allowedUsers;

        public AuthResponder(string[] allowedUsers)
        { 
            _allowedUsers = allowedUsers;
        }
        
        

        #region Overrides of ResponderBase

        public override bool CanRespond(IMessageContext context)
        {
            return base.CanRespond(context) && _allowedUsers.Length > 0 && !(_allowedUsers.Contains(context.Message.Detail.UserId) || _allowedUsers.Contains(context.Message.Detail.UserName));
        }

        public override Task GetResponse(IMessageContext context)
        {
            return context.Reply("This is not the bot you are looking for ... *wave bot like arms*");
        }

        #endregion
    }
}