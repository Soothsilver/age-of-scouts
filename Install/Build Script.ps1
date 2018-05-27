param(
	[string]$target="NO VERSION SPECIFIED"
)
Function echoc
{
	param ($text)
	Write-Host $text -foregroundcolor cyan
}
Function ZipFiles( $zipfilename, $sourcedir )
{
    [Reflection.Assembly]::LoadWithPartialName( "System.IO.Compression.FileSystem" )
    $compressionLevel = [System.IO.Compression.CompressionLevel]::Optimal
    echoc $sourcedir
    echoc $zipfilename
    [System.IO.Compression.ZipFile]::CreateFromDirectory("..\ReadyToRelease", "a.zip", $compressionLevel, $true) # $sourcedir, $zipfilename, $compressionLevel, $true )
}
if ($target.CompareTo("NO VERSION SPECIFIED") -eq 0)
{
	echoc "You must the target name where Sunheart will be exported."
	echoc "Try `"Build Script.ps1`" -target NewExportDir"
	Return
}
$ErrorActionPreference = "Stop"
$targetOriginal = $target
$target = "Age Of Scouts " + $targetOriginal;
echoc "Build script for Age of Scouts begins..."
echoc "1. Rebuilding solution..."
echoc "========================="
msbuild.exe "..\Age of Scouts.sln" /t:Build /p:Configuration=Release /p:Platform="x86"
echoc `n`n
echoc "==============================="
echoc "2. Creating required directories..."
echoc "==============================="
if (Test-Path "..\Releases" -PathType Container)
{
	echoc "The 'Releases' folder exists."
}
else
{
	echoc "The 'Releases' folder does not exist. Creating it..."
	new-item -name "..\Releases" -itemtype directory | Out-Null
}
if (Test-Path "..\ReadyToRelease" -PathType Container)
{
	echoc "The 'ReadyToRelease' folder exists."
}
else
{
	echoc "The 'ReadyToRelease' folder does not exist. Creating it..."
	new-item -name "..\ReadyToRelease" -itemtype directory | Out-Null
}
$readyToRelease = "..\ReadyToRelease"
echoc "Deleting all files from 'ReadyToRelease'..."
Remove-Item $readyToRelease -Force -Recurse
echoc "Deleted."
echoc "=================================="
echoc "3. Copying all applicable files..."
echoc "=================================="
echoc "Beginning copy..."
$excludeArray = "*.pdb","*.application","*.exe.config","*.exe.manifest"
Copy-Item -Path ".\helpers" -Destination "$readyToRelease\helpers" -Recurse
Copy-Item -Path "..\Age of Scouts\bin\x86\Release\*" -Destination "$readyToRelease" -Recurse -Exclude $excludeArray
Copy-Item -Path "..\Age of Scouts Launcher\bin\Release\*" -Destination "$readyToRelease" -Recurse -Exclude $excludeArray
echoc "Copying complete."
echoc "========================="
echoc "4. Creating a zip file... (will not work)"
echoc "========================="
echoc "Creating zip file..."
$targetZip = $target + ".zip"
$pubVers = Join-Path -Path $pwd -ChildPath "..\Releases"
$targetZipFileName = Join-Path -Path $pubVers -ChildPath $targetZip
$goFrom = Join-Path -Path $pwd -ChildPath "..\ReadyToRelease"
if (Test-Path $targetZipFileName)
{
	Write-Host "Target zip file already exists." -foregroundcolor Red
	Return
}

[Reflection.Assembly]::LoadWithPartialName( "System.IO.Compression.FileSystem" )
$compressionLevel = [System.IO.Compression.CompressionLevel]::Optimal
[System.IO.Compression.ZipFile]::CreateFromDirectory($goFrom, $targetZipFileName, $compressionLevel, $true) # $sourcedir, $zipfilename, $compressionLevel, $true )
echoc "Zip file creation ended."
echoc "=============================="
echoc "5. Creating Inno Setup file..."
echoc "=============================="
# echoc "Modyfing the setup script..."
# $scriptLines = Get-Content "SunheartInstaller.iss"
# $scriptLines[0] = "#define MyAppVersion `"" + $targetOriginal + "`""
# $scriptLines | Set-Content "SunheartInstaller.iss"
echoc "Creating setup file..."
& "ISCC.exe" "AgeOfScoutsInstaller.iss"
echoc "Setup file created. Maybe."
echoc "========================"
echoc "The process is complete."
echoc "The installer is in the Releases folder (but no further subfolder)."
echoc "Publishing complete. Press any key to continue..."
$x = $host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")