using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public override BotMessage GetResponse(MessageContext context)
        {
            var botMessage = new BotMessage() { Text =
                $"Hi, You are currently connected to {GetCurrentMachineInformation()}\n\n{GetCommands()}",
               // Attachments = Attachments()
            
            };

            return botMessage;
        }

        private List<SlackAttachment> Attachments()
        {
            return new List<SlackAttachment>( ) {
                new SlackAttachment() {
                    Text = "Select command",
                    Fallback = "Unfortunately it seems like you cant select a command right now.",
                    CallbackId = "select_command",
                    ColorHex =  "#3AA3E3",
                    Actions =   _responderDescriptions
                        .OfType<IResponderDescriptions>()
                        .SelectMany(x=>x.Descriptions)
                        .Select(BuildAction)
                        .Concat(_responderDescriptions
                            .OfType<IResponderDescription>().Select(BuildAction))
                        .ToList()
                } };
        }

        private SlackAttachmentAction BuildAction(IResponderDescription x)
        {
            Log.Information("x.Command:"+ x.Command);
            return new SlackAttachmentAction()
            {
                Text = x.Command,
                Name = x.Command.ToLower(),
                Value = x.Command.ToLower(),
                Type = "button"
            };
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