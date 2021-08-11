using System.IO;
using System.Linq;
using System.Threading.Tasks;
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

        public override async Task GetResponse(MessageContext context)
        {
            if (context.StartsWith(Name, out var param))
            {
                var runnerToRemove = FileRunners.Scripts.Find(param);
                if (runnerToRemove == null)
                {
                    await context.Say($"File '{param}' could not be found.");
                    return;
                }

                File.Delete(runnerToRemove.File);
                await context.Say(Path.GetFileName(runnerToRemove.File)+" removed");
            }
        }

        #endregion

        #region Implementation of IResponderDescription

        public string Command => Name+" [command]";

        public string Description => "Remove a script.";
    }
        #endregion
    
}