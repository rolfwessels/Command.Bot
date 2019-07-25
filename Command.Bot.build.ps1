Framework "4.0"

#
# properties
#

properties {
    $buildConfiguration = 'debug'
    $buildDirectory = 'build'
    $buildReportsDirectory = Join-Path $buildDirectory 'reports'
    $buildPackageDirectory = Join-Path $buildDirectory 'packages'
    $buildDistDirectory = Join-Path $buildDirectory 'dist'
    $buildPublishProjects = 'Command.Bot.Service'

    $srcDirectory = 'src'
    $srcSolution = Join-Path $srcDirectory 'Command.Bot.sln'

    $codeCoverRequired = 10

    $versionMajor = 1
    $versionMinor = 0
    $versionBuild = 1
    $versionRevision = 0
    
    $nuget = './src/.nuget/NuGet.exe';
}

#
# task
#

task default -depends build  -Description "By default it just builds"
task clean -depends build.clean, build.cleanbin -Description "Removes build folder"
task build -depends build.cleanbin, version, build.compile, build.copy -Description "Cleans bin/object and builds the project placing binaries in build directory"
task test -depends build, test.run  -Description "Builds and runs part cover tests"
task full -depends test, deploy.zip -Description "Versions builds and creates distributions"
task deploy -depends clean, version, build, deploy.zip -Description "Deploy the files to webserver using msdeploy"
task prerequisite -depends prerequisite.choco, prerequisite.dotnet  -Description "Install all prerequisites"

#
# task depends
#

task build.clean {
    remove-item -force -recurse $buildDirectory -ErrorAction SilentlyContinue
    $binFolders = Get-ChildItem ($srcDirectory + '\*\*') | where { $_.name -eq 'bin' -or $_.name -eq 'obj' } | Foreach-Object { $_.fullname }
    if ($binFolders -ne $null) {
        remove-item $binFolders -force -recurse -ErrorAction SilentlyContinue
    }
}

task build.cleanbin {
    remove-item -force -recurse $buildReportsDirectory -ErrorAction SilentlyContinue
    remove-item -force -recurse (buildConfigDirectory) -ErrorAction SilentlyContinue
    $binFolders = Get-ChildItem $srcDirectory -include bin, obj | Foreach-Object { $_.fullname }
    if ($binFolders -ne $null) {
        remove-item $binFolders -force -recurse -ErrorAction SilentlyContinue
    }
}

task build.compile {
    foreach ($buildPublishProject in $buildPublishProjects) {
        $toFolder = (Join-Path ( Join-Path (resolve-path .)(buildConfigDirectory)) $buildPublishProject)
        $project = Join-Path $srcDirectory $buildPublishProject
        
        Push-Location 'src'
        dotnet build 
        Pop-Location
        Push-Location $project
        if ($buildConfiguration -ne 'release') {
            write-host "Publish $project with suffix $buildConfiguration" -foreground "magenta"
            dotnet publish -c $buildConfiguration --self-contained --version-suffix $buildConfiguration  -v quiet
        }
        else {
            write-host "Publish $project  $buildConfiguration" -foreground "magenta"
            dotnet publish -c $buildConfiguration --self-contained -v quiet
        }
        #msbuild   /v:q
        if (!$?) {
            throw "Failed to publish $project"
        }
        
        Pop-Location
    }
}

task version {
    $projectFolders = Get-ChildItem $srcDirectory '*' -Directory
    foreach ($projectFolder in $projectFolders) {
        $projectFiles = Get-ChildItem $projectFolder.FullName '*.csproj' -File
        foreach ($projectFile in $projectFiles) {
            [xml]$Xml = Get-Content $projectFile.FullName
            $result = $Xml.Project.PropertyGroup.Version
            if (![string]::IsNullOrEmpty($result)) {
                $version = (fullversionrev) 
                write-host "Set version $version in  $projectFile" -foreground "magenta"
                $Xml.Project.PropertyGroup.Version = $version
                $Xml.Save( $projectFile.FullName)
            }
        }
    }
}

task build.copy {
    'Copy the console'
    $fromFolder = Join-Path $srcDirectory (Join-Path $buildPublishProjects (Join-Path (Join-Path bin $buildConfiguration ) 'net461\') )

    
    $toFolder = Join-Path (buildConfigDirectory) 'Command.Bot\'
    "$fromFolder*"
    copy-files "$fromFolder" $toFolder 
    
    write-host 'update configs' -foreground "magenta"
    remove-item (join-path  $toFolder 'appsettings.development.json')
    copy-files (join-path $srcDirectory 'Command.Bot.Core.Tests\Samples') (join-path $toFolder 'scripts')

 
}

task nuget.restore {
    ./src/.nuget/NuGet.exe install src\.nuget\packages.config -OutputDirectory lib
}

task test.run -depend nuget.restore  -precondition { return $buildConfiguration -eq 'debug' } {
    $reportFolder = mkdir $buildReportsDirectory -ErrorAction SilentlyContinue
    
    $currentPath = resolve-path '.'
    $openConverDirectory = resolve-path 'lib\OpenCover.4.6.519\tools'
    $nunitDirectory = resolve-path 'lib\NUnit.ConsoleRunner.3.7.0\tools\nunit3-console.exe'
    $reportGenerator = 'lib\ReportGenerator.3.0.0-beta3\tools'

    $openConverExe = './OpenCover.Console.exe'
    $nunit2failed = 'false'
    $failures = 0
   
    #$testFolders1 = Get-ChildItem $srcDirectory '*.Tests' -Directory  | Where-Object {Test-Path $_} | Select-Object -first 1
    $allTestDlls = Get-ChildItem $srcDirectory '*.Tests' -Directory | foreach { Join-Path (Join-Path $_.FullName (srcBinFolder)) ($_.Name + '.dll') } | Where-Object { Test-Path $_ } 
    $combinedDlls = [string]::Join(" ", $allTestDlls)
    $testOutputPrefix = "testResults";
    
    $buildReportsDirectoryResolved = '..\..\..\' + $buildReportsDirectory;
    $runTestsFolderResult = Join-Path $buildReportsDirectoryResolved ($testOutputPrefix + '.xml')
    $runTestsFolderOut = Join-Path $buildReportsDirectoryResolved ($testOutputPrefix + '.txt')
    $runTestsFolderPartResult = Join-Path $buildReportsDirectoryResolved ($testOutputPrefix + '.part.xml')

    
    Set-Location $openConverDirectory
    
    $target = '-targetargs:' + $combinedDlls + ' -noheader  -shadowcopy --out:' + $runTestsFolderOut + '  --result=' + $runTestsFolderResult
    
    &($openConverExe) -target:$nunitDirectory $target -oldstyle  -register:user -output:$runTestsFolderPartResult -log:Warn
    [xml]$Xml = Get-Content $runTestsFolderResult
    [int]$result = $Xml.'test-run'.failed
    $failures += $result

    if ($failures -gt 0) {
        throw "$failures Tests have failed!!!"
    }

    write-host 'Generate report' -foreground "magenta"
    Set-Location $currentPath
    Set-Location $reportGenerator
    $buildReportsDirectoryRelative = Join-Path '..\..\..\' $buildReportsDirectory
    $reports = Join-Path  $buildReportsDirectoryRelative '*.part.xml'
    $targetdir = Join-Path  $buildReportsDirectoryRelative 'CodeCoverage'
    $reporttypes = 'HTML;HTMLSummary;XMLSummary'
    $filters = '+Ifs';

    ./ReportGenerator.exe -reports:$reports -targetdir:$targetdir -reporttypes:$reporttypes -filters:$filters  -verbosity:Error
    Set-Location $currentPath
    write-host 'Validate code coverage' -foreground "magenta"

    $codeCoverSummary = Join-Path $buildReportsDirectory 'CodeCoverage\Summary.xml'
    [xml]$Xml = Get-Content $codeCoverSummary
    [int]$codeCover = $Xml.CoverageReport.Summary.LineCoverage -replace '%', ''

    if ($codeCover -lt $codeCoverRequired) {
        throw 'The solution currently has ' + $codeCover + '% coverage, less than the required ' + $codeCoverRequired + '%'
    }
    else {
        write-host "The solution currently has $codeCover% coverage" -foreground 'green'
        
    }
}

task deploy.zip {
    mkdir $buildDistDirectory -ErrorAction SilentlyContinue
    $folders = Get-ChildItem (buildConfigDirectory) -Directory
    foreach ($folder in $folders) {
        $version = fullversion
        $zipname = Join-Path $buildDistDirectory ($folder.name + '.v.' + $version + '.' + $buildConfiguration + '.zip' )
        write-host ('Create ' + $zipname)
        ZipFiles $zipname $folder.fullname
    }
}


task ? -Description "Helper to display task info" {
    WriteDocumentation
}

#
# functions
#

function fullversion() {
    $version = $versionBuild
    if ($env:BUILD_NUMBER) {
        $version = $env:BUILD_NUMBER
    }
    return "$versionMajor.$versionMajor.$version"
}

function fullversionrev() {
    return  (fullversion) + ".$versionRevision"
}


function srcBinFolder() {
    return  Join-Path (Join-Path bin $buildConfiguration ) 'win10-x64'
}

function buildConfigDirectory() {
    Join-Path $buildDirectory $buildConfiguration
}

function global:copy-files($source, $destination, $include = @(), $exclude = @()) {
    $sourceFullName = resolve-path $source
    $relativePath = Get-Item $source | Resolve-Path -Relative
    $mkdirResult = mkdir $destination -ErrorAction SilentlyContinue
    $files = Get-ChildItem $source -include $include -Recurse -Exclude $exclude
    foreach ($file in $files) {
        $relativePathOfFile = Get-Item $file.FullName | Resolve-Path -Relative
        $tofile = Join-Path $destination $relativePathOfFile.Substring($relativePath.length)
        Copy-Item -Force $relativePathOfFile $tofile
    }
}

function ZipFiles( $zipfilename, $sourcedir ) {
    del $zipfilename -ErrorAction SilentlyContinue
    Add-Type -Assembly System.IO.Compression.FileSystem
    $compressionLevel = [System.IO.Compression.CompressionLevel]::Optimal
    [System.IO.Compression.ZipFile]::CreateFromDirectory($sourcedir,
        $zipfilename, $compressionLevel, $false)
}

function xmlPoke([string]$file, [string]$xpath, $value, $attr = "", [hashtable]$namespaces  ) { 
    "loaded file $file"
    [xml] $fileXml = Get-Content $file 
    $xmlNameTable = new-object System.Xml.NameTable
    $xmlNameSpace = new-object System.Xml.XmlNamespaceManager($xmlNameTable)

    foreach ($key in $namespaces.keys) {
        $xmlNameSpace.AddNamespace($key, $namespaces.$key);
    }
    
    $node = $fileXml.SelectSingleNode($xpath, $xmlNameSpace) 
    
    if ($node) { 
        if ( [string]::IsNullOrEmpty($attr) ) {
            "xmlpoke: set node value $($node.name) $value"
            $node.InnerText = $value 
        }
        else {
            "xmlpoke: set attribute  $($node.name) $attr $value"
            $node.attributes[$attr].value = $value 
        }

        $fileXml.Save($file)  
    } 
    else {
        write-host  "Could not find node $xpath"  -foreground "red" 
    }
}

function WriteDocumentation() {
    $currentContext = $psake.context.Peek()

    if ($currentContext.tasks.default) {
        $defaultTaskDependencies = $currentContext.tasks.default.DependsOn
    }
    else {
        $defaultTaskDependencies = @()
    }

    $docs = $currentContext.tasks.Keys | foreach-object {
        if ($_ -eq "default" -or $_ -eq "?") {
            return
        }

        if ($_ -contains '.') {
            return
        }

        $task = $currentContext.tasks.$_
        new-object PSObject -property @{
            Name        = $task.Name;
            Description = $task.Description;
        }
    }

    $docs | where { -not [string]::IsNullOrEmpty($_.Description) } | sort 'Name' | sort 'Description' -Descending | format-table -autoSize -wrap -property Name, Description

    'Examples:'
    '----------'
    ''
    'Clean build directory before executing build:'
    'go clean,build'
    ''
    ''
    'Release build:'
    'go deploy -properties @{''buildConfiguration''=''Release''}'
    ''
    'Staging deploy to sepecified folder:'
    'go deploy -properties @{buildConfiguration=''Staging'';deployServiceDest =''computerName=''''xxxx'''',userName=''''xxx'''',password=''''xxxx'''',includeAcls=''''False'''',tempAgent=''''false'''',dirPath=''''d:\server\temp'''''' }'

}

