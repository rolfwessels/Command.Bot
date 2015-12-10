using System;
using System.Collections.Generic;
using System.Text;
using MargieBot.Models;
using MargieBot.Responders;

namespace Command.Bot.Core
{
    public class HelpResponder : ResponderBase
    {
        private readonly IEnumerable<IResponderDescription> _responderDescriptions;

        public HelpResponder(IEnumerable<IResponderDescription> responderDescriptions)
        {
            _responderDescriptions = responderDescriptions;
        }

        #region Overrides of ResponderBase

        public override bool CanRespond(ResponseContext context)
        {
            return IsForBot(context) && MessageContains(context, "help");
        }

        #endregion

        #region Implementation of IResponder

        public override BotMessage GetResponse(ResponseContext context)
        {
            return new BotMessage() { Text = string.Format("Hi, You are currently connected to {0}\n\n{1}", GetCurrentMachineInformation(),GetCommands())};
        }

        private string GetCommands()
        {
            var stringBuilder = new StringBuilder();
            foreach (var responderDescription in _responderDescriptions)
            {
                stringBuilder.AppendFormat("*{0}* {1}", responderDescription.Command, responderDescription.Description);
            }
            return stringBuilder.ToString();
        }

        private string GetCurrentMachineInformation()
        {
            return string.Format("{0}({1})", Environment.MachineName, Network.GetLocalIPAddress());
        }

        #endregion
    }
}