using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using Command.Bot.Core.Responders;
using Command.Bot.Core.Runner;
using Command.Bot.Core.Utils;
using SlackConnector;
using SlackConnector.Models;

namespace Command.Bot.Core
{
    public class MessageContext : IMessageContext, IDisposable
    {
        private readonly ISlackConnection _connection;
        private readonly Subject<string> _outputText;
        private IDisposable _disposable;
        private readonly IObservable<IList<string>> _buffered;
        private int _counter;

        public MessageContext(SlackMessage message, ISlackConnection connection)
        {
            _connection = connection;
            Message = message;
            _outputText = new Subject<string>();
            
            _buffered = _outputText.AsObservable()
                .Buffer(TimeSpan.FromSeconds(1), 5);
            _disposable = _buffered.Subscribe(x =>
                {
                    connection.Say($"```{x.StringJoin("\n")}```");
                    foreach (var _ in x)
                    {
                        Interlocked.Decrement(ref _counter);
                    }
                }
            );
        }

        public SlackMessage Message { get; }

        public Task Say(string text)
        {
            return GetValue(new BotMessage() { Text = text});
        }

        public string Text => this.CleanMessage();

        public Task SayOutput(string text)
        {
            if (string.IsNullOrEmpty(text)) return Task.FromResult(true);
            Interlocked.Increment(ref _counter);
            _outputText.OnNext(text);
            return Task.CompletedTask;
        }

        public Task SayError(string text)
        {
            if (string.IsNullOrEmpty(text)) return Task.FromResult(true);
            return GetValue(new BotMessage() { Text = $">>>`{text}`" });
        }

        public Task GetValue(BotMessage botMessage)
        {
            if (botMessage.ChatHub == null) botMessage.ChatHub = Message.ChatHub;
            if (string.IsNullOrEmpty(botMessage.Text)) return Task.FromResult(true);
            return _connection.Say(botMessage);
        }

        #region IDisposable

        public void Dispose()
        {
            FlushMessages().Wait();
            _outputText?.Dispose();
            _disposable?.Dispose();
        }

        #endregion

        public async Task FlushMessages()
        {
            while (_counter != 0)
            {
                await Task.Delay(100);
            }
        }
    }
}