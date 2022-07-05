using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Command.Bot.Core.SlackIntegration.Contracts;
using Command.Bot.Core.Utils;
using Serilog;

namespace Command.Bot.Core.SlackIntegration
{

    public class MessageContext : IMessageContext, IDisposable
    {
        private readonly ISlackRequest _message;
        private readonly ISlackRequest _connection;
        private readonly Subject<string> _outputText;
        private readonly IDisposable _disposable;
        private int _counter;
        public static int MaxLength = 100;
        public static TimeSpan BufferTimer = TimeSpan.FromMilliseconds(1000);

        public MessageContext(ISlackRequest message)
        {
            _message = message;
            _connection = message;
            
            _outputText = new Subject<string>();
            CleanText = CleanIt();
            
            
            _disposable = _outputText.AsObservable()
                .Buffer(BufferTimer).Subscribe(x =>
                {
                    foreach (var txt in x.GroupByMaxLength(MaxLength).Select(b=>b.StringJoin("\n"))) 
                    {   
                        try
                        {
                            Log.Information("say:" + txt);
                            Reply($"```{txt}```").Wait();
                        }
                        catch (Exception e)
                        {
                            Log.Error(e, "Failing");
                        }
                    }
                    
                    foreach (var _ in x)
                    {
                        Interlocked.Decrement(ref _counter);
                    }
                }
            );
        }

        private string CleanIt()
        {
            
            var text = _message.Detail?.Text;
            if (text != null)
            {
                text = Regex.Replace(text, @"<@.*?>:", " ");
                text = Regex.Replace(text, @"<@.*?>", " ");
                return text.Trim().Trim('*').Trim().ToLower();
            }
            return null;
        }

        public ISlackRequest Message => _message;


        public Task Reply(string text)
        {
            return Reply(new ReplyMessage() { Text = text});
        }

        public string Text => CleanText;
        public string CleanText { get; }

        public Task SayOutput(string text)
        {
            if (string.IsNullOrEmpty(text)) return Task.FromResult(true);
            Interlocked.Increment(ref _counter);
            _outputText.OnNext(text);
            return Task.CompletedTask;
        }

        public async Task SayError(string text)
        {
            await FlushMessages();
            if (string.IsNullOrEmpty(text)) return ;
            await Reply(new ReplyMessage() { 
                Attachments = new List<ReplyMessage.SlackAttachment>() {
                    new ReplyMessage.SlackAttachment() {ColorHex = "#D00000" , Text = text } }});
        }

        public bool IsForBot()
        {
            return Message.IsForBot();
            
        }

        public Task Reply(ReplyMessage replyMessage)
        {
            if (string.IsNullOrEmpty(replyMessage.Text) && !replyMessage.Attachments.Any()) return Task.FromResult(true);
            AllowLinksInEmails(replyMessage);
            return _connection.Reply(replyMessage);
        }

        private static void AllowLinksInEmails(ReplyMessage replyMessage)
        {
            var linkParser = new Regex(@"\b(?:https?://|www\.)\S+\b", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            foreach (Match m in linkParser.Matches(replyMessage.Text))
                replyMessage.Text = replyMessage.Text.Replace(m.Value, $"<{m.Value}>");
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

        public Task IndicateTyping()
        {
            return _message.IndicateTyping();
        }
    }
}