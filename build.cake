#tool nuget:?package=NUnit.ConsoleRunner&version=3.4.0
// #addin nuget:?package=Cake.Git&version=0.21.0


//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");



//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("Default")
    .Description("The default task that just build and tests.")
    .IsDependentOn("Test");
Task("Build")
    .Description("Build the project.")
    .IsDependentOn("Clean")
    .IsDependentOn("Restore")
    .IsDependentOn("Build-Project");
Task("Test")
    .Description("Run unit tests.")
    .IsDependentOn("Build")
    .IsDependentOn("Run-Unit-Tests");
Task("Dist")
    .Description("Build release zip file in the dist folder.")
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
    StartProcess("dotnet", new ProcessSettings {
        Arguments = new ProcessArgumentBuilder()
            .Append("clean")
            .Append("src/")
            .Append("-v m")
        }
    );  
});

Task("Restore")
    .Does(() =>
{
    DotNetCoreRestore("./src");
});

Task("Build-Project")
    .Does(() =>
{
    // DirectoryPath repoPath = Directory("./");
    // var commits = GitLog(repoPath, int.MaxValue);
    // version = $"{versionPrefix}{commits.Count}";
    // Information("Mark version as: {0}", commits.Count);
    // var dotNetCoreMSBuildSettings = new DotNetCoreMSBuildSettings()
    //         .SetVersion(version)
    //         .SetFileVersion(version)
    //         .SetInformationalVersion(version);
    var settings = new DotNetCoreBuildSettings {
        // MSBuildSettings = dotNetCoreMSBuildSettings,
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
         ArgumentCustomization = args => args.Append("/p:CoverletOutputFormat=opencover")
     };
    if(!IsRunningOnWindows())
    { 
        Information("Adding filter to remove ");
        settings = new DotNetCoreTestSettings
        {
            ArgumentCustomization = args => args.Append("--filter TestCategory!=windows-only")
        };
    }
    Information($"Running  {file.ToString()}"); 
    DotNetCoreTest(file.ToString(), settings);
});

Task("Build-Zip")
    .Does(() =>
{ 
     
    var toFolder = Directory(dirDist) + Directory($"Command.Bot.{version}.{configuration}");
    var from =  $"{Directory(dirService)}/**/*";
    var zipFile = $"{toFolder}.zip";
    Information($"Copy {from} to {toFolder}"); 
    CreateDirectory(toFolder);
    CopyFiles(from,toFolder);

    var toScriptsFolder = Directory(toFolder) + Directory($"scripts");
    var fromScripts = $"{Directory(samplesFolder)}/**/*";
    Information($"Copy {fromScripts} to {toScriptsFolder}"); 
    CreateDirectory(toScriptsFolder);
    CopyFiles(fromScripts,toScriptsFolder);

    Information($"Zipping to {zipFile}"); 
    Zip(toFolder, zipFile);
   
});

///////////////////////////////////////////////////////////////////////////////
// Docker stuff
///////////////////////////////////////////////////////////////////////////////

Task("Build-Docker")
    .Description("Build docker dev container.")
    .Does(() =>
{    
    StartProcess("docker-compose", new ProcessSettings {
        Arguments = new ProcessArgumentBuilder()
            .Append("build")
        }
    );
});

Task("Up")
    .Description("Open in docker dev container.")
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

Task("Down")
    .Description("Stop docker dev container.")
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