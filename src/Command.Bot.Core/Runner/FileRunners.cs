using System.Collections.Generic;
using System.IO;
using System.Linq;
using Command.Bot.Core.Responders;

namespace Command.Bot.Core.Runner
{
    public static class FileRunners
    {
        static FileRunners()
        {
            All = new IRunner[] { new BatchFile()};
        }

        public static IRunner[] All { get; set; }

        public static IEnumerable<FileRunner> Scripts
        {
            get
            {
                var files = Directory.GetFiles(GetOrCreateFullPath(@"scripts\"));
                foreach (var file in files)
                {
                    var fileRunner = All.Where(x=>x.IsExtensionMatch(file)).Select(x => x.GetRunner(file)).FirstOrDefault(x => x != null);
                    if (fileRunner != null) yield return fileRunner;
                }
            }
        }

        public static string GetFileLocation(string name)
        {
            return GetOrCreateFullPath(@"scripts\") + name;
        }

        private static string GetOrCreateFullPath(string scripts)
        {
            var orCreateFullPath = Path.GetFullPath(scripts);
            if (!Directory.Exists(orCreateFullPath))
            {
                Directory.CreateDirectory(orCreateFullPath);
            }
            return orCreateFullPath;
        }
    }

    
}