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
        Task Say(string text);
        Task IndicateTyping();
        Task FlushMessages();
    }
}