using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Command.Bot.Core.Utils;

namespace Command.Bot.Core.Responders
{
    public class HelpResponder : ResponderBase , IResponderDescription
    {
        private readonly IEnumerable<IResponder> _responderDescriptions;

        public HelpResponder(IEnumerable<IResponder> responderDescriptions)
        {
            _responderDescriptions = responderDescriptions;
        }

        #region Overrides of ResponderBase

        public override bool CanRespond(MessageContext.MessageContext context)
        {   
            return context.IsForBot() && context.HasMessage(Command);
        }

        #endregion

        #region Implementation of IResponder

        public override Task GetResponse(MessageContext.MessageContext context)
        {
            return context.Say($"Hi, You are currently connected to {GetCurrentMachineInformation()}\n\n{GetCommands()}");
        }

        private string GetCommands()
        {
            var stringBuilder = new StringBuilder();
            foreach (var description in _responderDescriptions.GetCommands())
            {
                AppendDescription(stringBuilder, description);
            }
            return stringBuilder.ToString();
        }

        private static void AppendDescription(StringBuilder stringBuilder, IResponderDescription description)
        {
            stringBuilder.AppendLine($"*{description.Command}* {description.Description}");
        }

        private string GetCurrentMachineInformation()
        {
            return $"{Environment.MachineName}({Network.GetLocalIpAddress()})";
        }

        #endregion

        #region Implementation of IResponderDescription

        public string Command => "help";
        public string Description => "Display help information";

        #endregion
    }

    
}