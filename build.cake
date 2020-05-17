#tool nuget:?package=NUnit.ConsoleRunner&version=3.4.0

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

Task("Release")
    .IsDependentOn("Test")
    .IsDependentOn("Test");

//////////////////////////////////////////////////////////////////////
// PREPARATION
//////////////////////////////////////////////////////////////////////

// Define directories.
var buildDir = Directory("./src/Command.Bot.Service/bin") + Directory(configuration);
var sln = "./src/Command.Bot.sln";

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
    if(IsRunningOnWindows())
    {
      // Use MSBuild
      MSBuild(sln, settings =>
        settings.SetConfiguration(configuration).SetVerbosity(Verbosity.Minimal));
    }
    else
    {
        var settings = new DotNetCoreBuildSettings
        {
            WorkingDirectory = "./src/Command.Bot/",
            Configuration = configuration
        };
        DotNetCoreBuild("Command.Bot.csproj", settings);
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