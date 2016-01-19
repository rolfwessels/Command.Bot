using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Command.Bot.Core.Runner;
using log4net;

namespace Command.Bot.Core.Tests.Runner
{
    public class FakeContext : IMessageContext
    {
        private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        readonly List<string> _response  = new List<string>();
        #region Implementation of IMessageContext

        public Task SayOutput(string text)
        {
            _log.Info(text);
            _response.Add("INFO:" + text);
            return Task.FromResult(true);
        }

        public Task SayError(string text)
        {
            _log.Error(text);
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