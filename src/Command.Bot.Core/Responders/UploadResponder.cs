using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Command.Bot.Core.Parser;
using Command.Bot.Core.Runner;
using log4net;
using SlackConnector.Models;

namespace Command.Bot.Core.Responders
{
    public class UploadResponder : ResponderBase
    {
        private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly IRunner[] _extensions;

        public UploadResponder()
        {
            _extensions = FileRunners.All;
        }

        #region Overrides of ResponderBase

        public override bool CanRespond(MessageContext context)
        {
            return base.CanRespond(context) && context.HasAttachment();
        }

        #endregion

        #region Overrides of ResponderBase

        public override BotMessage GetResponse(MessageContext context)
        {
            var fullMessage = context.Message.GetFullMessage();
            if (_extensions.Any(x => x.IsExtensionMatch(fullMessage.File.Name)))
            {
                var resource = new Uri(fullMessage.File.UrlDownload);
                context.Say("Reading file");
                var downloadData = resource.Download();
                
                using (var fileStream = File.OpenWrite(FileRunners.GetFileLocation( fullMessage.File.Name)))
                {
                    fileStream.Write(downloadData, 0, downloadData.Length);
                    context.Say("File saved");
                }
                return new BotMessage() { Text = "Command saved" };
            }
            return new BotMessage() { Text = "Unknown file type."};
        }

        #endregion

       
    }
}