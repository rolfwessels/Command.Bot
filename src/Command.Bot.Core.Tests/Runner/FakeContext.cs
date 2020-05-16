using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Command.Bot.Core.Runner;
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

        public List<string> Response { get; }

        #endregion
    }
}