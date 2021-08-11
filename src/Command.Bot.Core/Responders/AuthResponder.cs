using System.Linq;
using System.Threading.Tasks;

namespace Command.Bot.Core.Responders
{
    public class AuthResponder : ResponderBase
    {
        private readonly string[] _allowedUsers;

        public AuthResponder(string[] allowedUsers)
        { 
            _allowedUsers = allowedUsers;
        }

        public static string[] SplitTheAllowedUsers()
        {
            return Settings.Default.AllowedUser.Split(',', ':', ';').Select(x=>x.Trim()).Where(x=>!string.IsNullOrEmpty(x)).ToArray();
        }

        #region Overrides of ResponderBase

        public override bool CanRespond(MessageContext context)
        {
            return base.CanRespond(context) && _allowedUsers.Length > 0 && !(_allowedUsers.Contains(context.Message.User.Id) || _allowedUsers.Contains(context.Message.User.Name));
        }

        public override Task GetResponse(MessageContext context)
        {
            return context.Say("This is not the bot you are looking for ... *wave bot like arms*");
        }

        #endregion
    }
}