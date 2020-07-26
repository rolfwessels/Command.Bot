#tool nuget:?package=NUnit.ConsoleRunner&version=3.4.0



//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");
var branch = Argument("branch", "");
var dockerImageName =  Argument("dockerImageName", "rolfwessels/command-bot"); 



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

Task("Version")
    .Does(() =>
{
    version = $"{versionPrefix}"+Run("git","rev-list","HEAD","--count");
    Information("Mark version as: {0}", version);
});

Task("Build-Project")
    .IsDependentOn("Version")
    .Does(() =>
{   
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


Task("Push-Docker")
    .Description("Pushes to docker based on git branch.")
    .Does(() =>
{
    if (string.IsNullOrEmpty(branch)) {
        branch = "git rev-parse --abbrev-ref HEAD";
    }
    
    Information($"You are on {branch} branch");
    if (branch.StartsWith("feature")) {
        var target = $"{dockerImageName}:alpha";
        DockerBuildAndPush(new [] {target});
    }
    else if (branch == "master") {
        var target = $"{dockerImageName}:beta";
        DockerBuildAndPush(new [] {target});
    }
    else if (branch.StartsWith("v1")) {
        var target = $"{dockerImageName}:latest";
        DockerBuildAndPush(new [] {target, $"{dockerImageName}:{branch}"});
    }
});


void DockerBuildAndPush(string[] targets)  {
    Information($"Building targets {string.Join(" and ",targets)}"); 
    var buildArguments = new ProcessArgumentBuilder().Append("build");
    foreach (var target in targets)
    {
        buildArguments = buildArguments.Append($"-t {target}");
    }
    buildArguments = buildArguments.Append(".");        
    StartProcess("docker", new ProcessSettings {Arguments = buildArguments});
    StartProcess("docker", new ProcessSettings {
    Arguments = new ProcessArgumentBuilder()
        .Append("push")
        .Append($"{dockerImageName}")
        }
    );
}

string Run(params string[] commands) {
    var arguments = new ProcessArgumentBuilder();
    foreach (var test in commands.Skip(1)) {
      arguments =  arguments.Append(test);
    }
    var output = "";
    var setting = new ProcessSettings { 
        Arguments = arguments, 
        RedirectedStandardOutputHandler = (d) => output += "\n"+d,
        RedirectStandardOutput = true,
    };
    StartProcess(commands.First(),setting);
    return output.Trim();
}
//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);