namespace Command.Bot.Shared.Components.Runner
{
    public interface IRunner
    {
        string Extension { get;  }
        FileRunner GetRunner(string filePath);
    }
}