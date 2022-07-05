using System.Collections.Generic;
using System.Threading.Tasks;

namespace Command.Bot.Core.SlackIntegration.Contracts
{
    public interface ISlackConnectionHandler
    {
        Task MessageReceived(ISlackRequest request);
    }

    public interface ISlackRequest  
    {
        ISlackDetail Detail { get;}
        bool IsForBot();
        Task Reply(ReplyMessage message);
        Task WrapInTyping(Task executeRunner);
    }

    public interface ISlackDetail
    {
        string UserName { get;  }
        string Text { get;  }
        string UserId { get; }
        string ChannelId { get;  }
    }

    public class ReplyMessage
    {
        public ReplyMessage(string text)
        {
            Text = text;
        }

        public ReplyMessage()
        {
        }

        public string Text { get; set; }
        public List<SlackAttachment> Attachments { get; set; }
        //public string ConversationId { get; set; }

        public class SlackAttachment
        {
            public string ColorHex { get; set; }
            public string Text { get; set; }
        }
    }
}