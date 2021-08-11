using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serilog;
using SlackConnector.Models;

namespace Command.Bot.Core.Responders
{
    public class HelpResponder : ResponderBase
    {
        private readonly IEnumerable<IResponder> _responderDescriptions;

        public HelpResponder(IEnumerable<IResponder> responderDescriptions)
        {
            _responderDescriptions = responderDescriptions;
        }

        #region Overrides of ResponderBase

        public override bool CanRespond(MessageContext context)
        {
            
            return context.IsForBot() && context.HasMessage("help");
        }

        #endregion

        #region Implementation of IResponder

        public override Task GetResponse(MessageContext context)
        {
            return context.Say($"Hi, You are currently connected to {GetCurrentMachineInformation()}\n\n{GetCommands()}");
        }

        private string GetCommands()
        {
            var stringBuilder = new StringBuilder();
            foreach (var responderDescription in _responderDescriptions)
            {
                var description = responderDescription as IResponderDescription;
                if (description != null)
                    AppendDescription(stringBuilder, description);
                var descriptions = responderDescription as IResponderDescriptions;
                if (descriptions == null) continue;
                foreach (var desc in descriptions.Descriptions)
                {
                    AppendDescription(stringBuilder, desc);
                }
            }
            return stringBuilder.ToString();
        }

        private static void AppendDescription(StringBuilder stringBuilder, IResponderDescription description)
        {
            stringBuilder.AppendLine($"*{description.Command}* {description.Description}");
        }

        private string GetCurrentMachineInformation()
        {
            return $"{Environment.MachineName}({Network.GetLocalIPAddress()})";
        }

        #endregion
    }

    
}