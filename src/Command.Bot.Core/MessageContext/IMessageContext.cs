using System.Threading.Tasks;

namespace Command.Bot.Core.MessageContext
{
    public interface IMessageContext
    {
        string Text { get; }
        Task SayOutput(string text);
        Task SayError(string text);
    }
}