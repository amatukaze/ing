$source = "..\artifacts\Sakuno.ING.Launcher.Canonical\x64\Release\net472\"
$exeName = "IntelligentNavalGun.exe"
$version = (Get-Item (Join-Path $source $exeName -Resolve)).VersionInfo
if (-not (Test-Path "..\packages"))
{
    New-Item -Path '..\packages' -ItemType Directory
}
$destination = '..\packages\ING ' + $version.ProductVersion
if (Test-Path $destination)
{
    Remove-Item -Path $destination -Recurse
}
New-Item -Path $destination -ItemType Directory
Copy-Item -Path (Join-Path $source '*') -Destination $destination -Exclude '*.pdb' -Recurse
Compress-Archive -Path $destination -DestinationPath ($destination + '.zip') -CompressionLevel Optimal -Force
