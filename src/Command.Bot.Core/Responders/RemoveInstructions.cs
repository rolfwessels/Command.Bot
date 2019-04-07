using System.IO;
using System.Linq;
using Command.Bot.Core.Runner;
using SlackConnector.Models;

namespace Command.Bot.Core.Responders
{
   
    public class RemoveInstructions : ResponderBase, IResponderDescription
    {
        private const string Name = "remove";

        #region Overrides of ResponderBase

        public override bool CanRespond(MessageContext context)
        {
            return base.CanRespond(context) && context.StartsWith(Name);
        }

        #endregion

        #region Overrides of ResponderBase

        public override BotMessage GetResponse(MessageContext context)
        {
            if (context.StartsWith(Name, out var param))
            {
                var runnerToRemove = FileRunners.Scripts.Find(param);
                if (runnerToRemove == null) return new BotMessage() {Text = $"File '{param}' could not be found."};
                File.Delete(runnerToRemove.File);
                return new BotMessage() { Text = Path.GetFileName(runnerToRemove.File)+" removed" };
            }
            return new BotMessage() { Text = "File not found." };
        }

        #endregion

        #region Implementation of IResponderDescription

        public string Command => Name+" [command]";

        public string Description => "Remove a script.";
    }
        #endregion
    
}