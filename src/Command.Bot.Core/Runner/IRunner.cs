namespace Command.Bot.Core.Runner
{
    public interface IRunner
    {
        string Extension { get;  }
        FileRunner GetRunner(string filePath);
    }
}