using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Command.Bot.Core.Runner;
using Serilog;

namespace Command.Bot.Core.Tests.Runner
{
    public class FakeContext : IMessageContext
    {
        

        readonly List<string> _response  = new List<string>();
        #region Implementation of IMessageContext

        public Task SayOutput(string text)
        {
            Log.Information(text);
            _response.Add("INFO:" + text);
            return Task.FromResult(true);
        }

        public Task SayError(string text)
        {
            Log.Error(text);
            _response.Add("ERROR:" + text);
            return Task.FromResult(true);
        }

        public List<string> Response
        {
            get { return _response; }
        }

        #endregion
    }
}