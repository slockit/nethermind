Write-Host 'Building Nethermind.BeaconNode.Host'

@(Join-Path $PSScriptRoot '../Nethermind.BeaconNode.Test') | % { 
    dotnet test $_
    if (!$?) { throw 'Tests failed' }
}

if (Test-Path (Join-Path $PSScriptRoot '/bin/Release')) {
    Remove-Item -Path (Join-Path $PSScriptRoot 'bin/Release') -Recurse
}

$gitversion = Join-Path $PSScriptRoot 'tools/gitversion/GitVersion.dll' 
$v = (dotnet $gitversion | ConvertFrom-Json)
dotnet publish (Join-Path $PSScriptRoot 'Nethermind.BeaconNode.Host.csproj') -c Release -p:AssemblyVersion=$($v.AssemblySemVer) -p:FileVersion=$($v.AssemblySemFileVer) -p:Version=$($v.SemVer)+$($v.ShortSha) -p:PackageVersion=$($v.NuGetVersion)

$versionFolder = Join-Path $PSScriptRoot "release/$($v.NuGetVersion)"
if (Test-Path $versionFolder) {
    Remove-Item -Path $versionFolder -Recurse
}
New-Item -Path $versionFolder -ItemType Directory -Force
Copy-Item -Path (Join-Path $PSScriptRoot 'bin/Release/netcoreapp3.0/publish/*') -Destination $versionFolder -Recurse

$hostsettings = Get-Content -Raw -Path (Join-Path $versionFolder 'hostsettings.json') | ConvertFrom-Json
$hostsettings.Environment = 'Production'
$hostsettings | ConvertTo-Json | Set-Content -Path (Join-Path $versionFolder 'hostsettings.json')

$latestFolder = Join-Path $PSScriptRoot 'release/latest'
if (Test-Path $latestFolder) {
    Remove-Item -Path $latestFolder -Recurse
}
New-Item -Path $latestFolder -ItemType Directory -Force
Copy-Item -Path (Join-Path $versionFolder '*') -Destination $latestFolder -Recurse
