using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Command.Bot.Core.SlackIntegration.Contracts;
using Serilog;

namespace Command.Bot.Core.Tests.Runner
{
    public class FakeContext : IMessageContext
    {
        public FakeContext()
        {
            Response = new List<string>();
        }

        #region Implementation of IMessageContext

        public string Text { get; set; }
        public string CleanText { get; }
        public ISlackRequest Message { get; }

        public Task SayOutput(string text)
        {
            Log.Information(text);
            Response.Add("INFO:" + text);
            return Task.FromResult(true);
        }

        public Task SayError(string text)
        {
            Log.Error(text);
            Response.Add("ERROR:" + text);
            return Task.FromResult(true);
        }

        public bool IsForBot()
        {
            throw new System.NotImplementedException();
        }

        public Task Reply(string text)
        {
            throw new System.NotImplementedException();
        }
        

        public Task FlushMessages()
        {
            throw new System.NotImplementedException();
        }
        

        public async Task WrapInTyping(Func<Task> executeRunner)
        {
            await executeRunner();
        }

        public List<string> Response { get; }

        #endregion

        #region Implementation of IDisposable

        public void Dispose()
        {
            throw new System.NotImplementedException();
        }

        #endregion
    }
}