using System.Collections.Generic;

namespace Command.Bot.Core
{
    public interface IResponderDescriptions
    {
        IEnumerable<IResponderDescription> Descriptions { get; }
    }
}