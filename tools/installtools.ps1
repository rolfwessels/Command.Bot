# variables
$nunitUri = "http://launchpad.net/nunitv2/trunk/2.6.3/+download/NUnit-2.6.3.zip"
$7zaUri = "https://github.com/chocolatey/chocolatey/blob/master/src/tools/7za.exe?raw=true"
$nantUri = "http://downloads.sourceforge.net/project/nant/nant/0.92/nant-0.92-bin.zip?r=http%3A%2F%2Fsourceforge.net%2Fprojects%2Fnant%2Ffiles%2Fnant%2F0.92%2F&ts=1382986679&use_mirror=ufpr"
$thisFile = Get-ChildItem $MyInvocation.MyCommand.Path
$nunitDir = Join-Path $thisFile.DirectoryName "nunit"
$nantDir = Join-Path $thisFile.DirectoryName "nant"
$tempDir = Join-Path $thisFile.DirectoryName "temp"

function Download-File {
param (
  [string]$uri,
  [string]$file
 )
  if (![System.IO.Directory]::Exists($tempDir)) {[System.IO.Directory]::CreateDirectory($tempDir)}
  if (![System.IO.File]::Exists($file)) {
	  Write-Host "Downloading $uri to $file"
	  $downloader = new-object System.Net.WebClient
	  $downloader.DownloadFile($uri, $file)
  }
}

function Extract-File {
param (
  [string]$fromFile,
  [string]$toFolder
 )
 	if (![System.IO.Directory]::Exists($toFolder)) {[System.IO.Directory]::CreateDirectory($toFolder)}
  	# download 7zip
	$7zaExe = Join-Path $tempDir '7za.exe'
	Download-File $7zaUri $7zaExe

	# unzip the package
	Write-Host "Extracting $fromFile to $toFolder..."
	Start-Process "$7zaExe" -ArgumentList "x -o`"$toFolder`" -y `"$fromFile`"" -Wait
	#$shellApplication = new-object -com shell.application 
	#$zipPackage = $shellApplication.NameSpace($fromFile) 
	#$destinationFolder = $shellApplication.NameSpace($toFolder) 
	#$destinationFolder.CopyHere($zipPackage.Items(),0x10)
}

function Copy-Folder {
param (
  [string]$foldercontains,
  [string]$toFolder
 )
 	if (![System.IO.Directory]::Exists($toFolder)) {
	  	# extract 
	  	Write-Host "Looking for $foldercontains in $tempDir..."
		$folders = Get-ChildItem -Path $tempDir -Filter $foldercontains -recurse | ?{ $_.PSIsContainer }
		foreach ($objItem in $folders) {
		    $cpFrom = $objItem.FullName
		    $cpFrom = Join-Path $cpFrom "bin"
		    Write-Host "Copy for $cpFrom to $toFolder..."
		    Copy-Item $cpFrom $toFolder -recurse
		}
	}
}

# download nant
if (![System.IO.Directory]::Exists($nunitDir)) {
	$zipfile = Join-Path $tempDir "nunit.zip"
	Download-File $nunitUri $zipfile
	Extract-File $zipfile $tempDir
	Copy-Folder 'nunit*' $nunitDir
}

if (![System.IO.Directory]::Exists($nantDir)) {
	$zipfile = Join-Path $tempDir "nant.zip"
	Download-File $nantUri $zipfile
	Extract-File $zipfile $tempDir
	Copy-Folder 'nant*' $nantDir
}

if ([System.IO.Directory]::Exists($tempDir)) {
	Write-Host "Remove folder $tempDir"
	Remove-Item -Recurse -Force $tempDir
}

