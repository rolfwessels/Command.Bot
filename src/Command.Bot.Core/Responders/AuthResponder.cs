using System;
using System.Collections.Generic;
using System.Linq;
using Command.Bot.Core.Properties;
using SlackConnector.Models;

namespace Command.Bot.Core.Responders
{
    public class AuthResponder : ResponderBase
    {
        private readonly string[] _allowedUsers;

        public AuthResponder()
        { 
            _allowedUsers = SplitTheAllowedUsers();
        }

        private static string[] SplitTheAllowedUsers()
        {
            return Settings.Default.AllowedUser.Split(new [] {',',':',';'}).Select(x=>x.Trim()).Where(x=>!string.IsNullOrEmpty(x)).ToArray();
        }

        #region Overrides of ResponderBase

        public override bool CanRespond(MessageContext context)
        {
            return base.CanRespond(context) && _allowedUsers.Length > 0 && !_allowedUsers.Contains(context.Message.User.Name);
        }

        public override BotMessage GetResponse(MessageContext context)
        {
            return new BotMessage() {Text = "This is not the bot you are looking for ... *wave bot like arms*"};
        }

        #endregion
    }
}