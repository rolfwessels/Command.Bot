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
            string param;
            if (context.StartsWith(Name, out param))
            {
                var firstOrDefault = FileRunners.Scripts.FirstOrDefault(x=>x.MatchesString(param));
                if (firstOrDefault != null)
                {
                    File.Delete(firstOrDefault.File);
                    return new BotMessage() { Text = Path.GetFileName(firstOrDefault.File)+" removed" };
                }
            }
            return new BotMessage() { Text = "File not found." };
        }

        #endregion

        #region Implementation of IResponderDescription

        public string Command {
            get { return Name+" [command]"; }
        }

        public string Description
        {
            get { return "Remove a script."; }
        }

        
    }

        #endregion
    
}