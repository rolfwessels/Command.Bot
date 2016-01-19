using System.Threading.Tasks;

namespace Command.Bot.Core.Runner
{
    public interface IMessageContext
    {
        Task SayOutput(string text);
        Task SayError(string text);
    }
}