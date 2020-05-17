#tool nuget:?package=NUnit.ConsoleRunner&version=3.4.0
#addin nuget:?package=Cake.Git&version=0.21.0


//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");
var coverageThreshold = Argument("coverageThreshold", "100");



//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("Default")
    .IsDependentOn("Test");
Task("Build")
    .IsDependentOn("Restore")
    .IsDependentOn("Build-Project");
Task("Test")
    .IsDependentOn("Build")
    .IsDependentOn("Run-Unit-Tests");
Task("Dist")
    .IsDependentOn("Test")
    .IsDependentOn("Build-Zip");

//////////////////////////////////////////////////////////////////////
// PREPARATION
//////////////////////////////////////////////////////////////////////

// Define directories.
var sln = "./src/Command.Bot.sln";
var dirDist = "./dist";
var samplesFolder = "./src/Command.Bot.Core.Tests/Samples";
var dirService = Directory("./src/Command.Bot.Service/bin") + Directory(configuration) + Directory("net461");
var versionPrefix = "1.0.";
var version = $"{versionPrefix}0";
//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("Clean")
    .Description("Cleans all directories that are used during the build process.")
    .Does(() =>
{
    DotNetCoreClean("./src");
});

Task("Restore")
    .Does(() =>
{
    DotNetCoreRestore("./src");
});

Task("Build-Project")
    .Does(() =>
{
    DirectoryPath repoPath = Directory("./");
    var commits = GitLog(repoPath, int.MaxValue);
    version = $"{versionPrefix}{commits.Count}";
    Information("Mark version as: {0}", commits.Count);
    var dotNetCoreMSBuildSettings = new DotNetCoreMSBuildSettings()
            .SetVersion(version)
            .SetFileVersion(version)
            .SetInformationalVersion(version);
    var settings = new DotNetCoreBuildSettings {
        MSBuildSettings = dotNetCoreMSBuildSettings,
        Configuration = configuration,
    };

    if(IsRunningOnWindows())
    {
        DotNetCoreBuild(sln, settings);
    }
    else
    {
    
        DotNetCoreBuild("./src/Command.Bot/Command.Bot.csproj", settings);
    }
});

Task("Run-Unit-Tests")
    .DoesForEach(GetFiles("./src/**/*Tests.csproj"), (file) => 
{ 
    var settings = new DotNetCoreTestSettings
     {
         ArgumentCustomization = args => args.Append("/p:CollectCoverage=true")
                                             .Append("/p:CoverletOutputFormat=opencover")
                                             .Append("/p:ThresholdType=line")
                                             .Append($"/p:Threshold={coverageThreshold}")
     };
    Information($"Running  {file.ToString()}"); 
    DotNetCoreTest(file.ToString(), settings);
});

Task("Build-Zip")
    .Does(() =>
{ 
    var toFolder = Directory(dirDist) + Directory($"Command.Bot.{version}.{configuration}");
    var from = Directory(dirService)+Directory("**/*");
    var zipFile = $"{toFolder}.zip";
    Information($"Copy {from} to {toFolder}"); 
    CreateDirectory(toFolder);
    CopyFiles(from,toFolder);

    var toScriptsFolder = Directory(toFolder) + Directory($"scripts");
    var fromScripts = Directory(samplesFolder)+Directory("**/*");
    Information($"Copy {fromScripts} to {toScriptsFolder}"); 
    CreateDirectory(toScriptsFolder);
    CopyFiles(fromScripts,toScriptsFolder);

    Information($"Zipping to {zipFile}"); 
    Zip(toFolder, zipFile);
   
});

///////////////////////////////////////////////////////////////////////////////
// Docker stuff
///////////////////////////////////////////////////////////////////////////////

Task("dcb")
    .Does(() =>
{    
    StartProcess("docker-compose", new ProcessSettings {
        Arguments = new ProcessArgumentBuilder()
            .Append("build")
        }
    );
});

Task("up")
    .Does(() =>
{
    StartProcess("docker-compose", new ProcessSettings {
        Arguments = new ProcessArgumentBuilder()
            .Append("up")
            .Append("-d")
        }
    );
    
    StartProcess("docker-compose", new ProcessSettings {
        Arguments = new ProcessArgumentBuilder()
            .Append("exec")
            .Append("dev")
            .Append("bash")
        }
    );
});

Task("down")
    .Does(() =>
{
    StartProcess("docker-compose", new ProcessSettings {
        Arguments = new ProcessArgumentBuilder()
            .Append("down")
        }
    );
});



//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);