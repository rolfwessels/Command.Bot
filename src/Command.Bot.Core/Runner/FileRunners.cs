using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using Serilog;

namespace Command.Bot.Core.Runner
{
    public static class FileRunners
    {
        
        private static readonly Lazy<string> BasePath = new Lazy<string>(()=> GetOrCreateFullPath(Settings.Default.ScriptsPath));

        static FileRunners()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                All = new IRunner[] {new BatchFile(), new PowerShellFile()};
            }
            else
            {
                All = new IRunner[] { new ShFile() };
            }
            
            
        }

        public static IRunner[] All { get; set; }

        public static IEnumerable<FileRunner> Scripts
        {
            get
            {
                var files = Directory.GetFiles(BasePath.Value);
                foreach (var file in files)
                {   
                    var fileRunner = All.Where(x=>x.IsExtensionMatch(file)).Select(x => x.GetRunner(file)).FirstOrDefault(x => x != null);
                    if (fileRunner != null)
                    {
                        Log.Debug("file:" + file);
                        yield return fileRunner;
                    }
                }
            }
        }

        public static string GetFileLocation(string name)
        {
            return Path.Combine(BasePath.Value, name) ;
        }

        private static string GetOrCreateFullPath(string scripts)
        {
            var directoryName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)??@".\";
            var fullPath = Path.GetFullPath(Path.Combine(directoryName, scripts));
            Log.Debug("FileRunners:GetOrCreateFullPath Using path : "+fullPath);
            if (!Directory.Exists(fullPath))
            {
                Directory.CreateDirectory(fullPath);
            }
            return fullPath;
        }
    }

    
}