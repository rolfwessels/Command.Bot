using System;
using System.Threading.Tasks;

namespace Command.Bot.Core.SlackIntegration.Contracts
{
    public interface IMessageContext : IDisposable
    {
        string Text { get; }
        string CleanText { get; }
        ISlackRequest Message { get; }
        Task SayOutput(string text);
        Task SayError(string text);
        bool IsForBot();
        Task Reply(string text);
        Task FlushMessages();
        Task WrapInTyping(Func<Task> executeRunner);
    }
}