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
            if (HasAFileContainingExtention(fullMessage))
            {
                var uriString = fullMessage.File.UrlDownload ?? fullMessage.File.UrlPrivateDownload ?? fullMessage.File.UrlPrivate;
                _log.Info(string.Format("Downloading '{0}'", uriString));
                var resource = new Uri(uriString);
                context.Say("Reading file");
                var downloadData = resource.Download();
                
                using (var fileStream = File.OpenWrite(FileRunners.GetFileLocation( fullMessage.File.Name)))
                {
                    fileStream.Write(downloadData, 0, downloadData.Length);
                    context.Say("File saved");
                }
                return new BotMessage() { Text = string.Format("Command saved. You can type *{0}* to run the command.", Path.GetFileName(fullMessage.File.Name)) };
            }
            return new BotMessage() { Text = "Unknown file type."};
        }

        private bool HasAFileContainingExtention(MessageParser.RootObject fullMessage)
        {
            return fullMessage != null && (fullMessage.File != null && _extensions.Any(x => x.IsExtensionMatch(fullMessage.File.Name)));
        }

        #endregion

       
    }
}