param(
    [string] $Destination = (Join-Path (Split-Path -Parent $PSScriptRoot) '.build/game-references')
)
$ErrorActionPreference = 'Stop'
$Destination = [IO.Path]::GetFullPath($Destination)
$workRoot = Join-Path (Split-Path -Parent $PSScriptRoot) '.build'
$steamCmdRoot = Join-Path $workRoot 'steamcmd'
$serverRoot = Join-Path $workRoot 'dedicated-server'
New-Item -ItemType Directory -Force -Path $steamCmdRoot, $serverRoot, $Destination | Out-Null
$steamCmd = Join-Path $steamCmdRoot 'steamcmd.exe'
if (-not (Test-Path -LiteralPath $steamCmd)) {
    $archive = Join-Path $workRoot 'steamcmd.zip'
    Invoke-WebRequest 'https://steamcdn-a.akamaihd.net/client/installer/steamcmd.zip' -OutFile $archive
    Expand-Archive -LiteralPath $archive -DestinationPath $steamCmdRoot -Force
}

# This script is called only on an exact cache miss, never on a cache hit.
# SteamCMD occasionally fails during self-update; retry before failing the job.
$installed = $false
for ($attempt = 1; $attempt -le 3; $attempt++) {
    & $steamCmd +force_install_dir $serverRoot +login anonymous +app_update 298740 validate +quit
    if ($LASTEXITCODE -eq 0 -and (Test-Path -LiteralPath (Join-Path $serverRoot 'DedicatedServer64/Sandbox.Game.dll'))) {
        $installed = $true
        break
    }
    if ($attempt -lt 3) { Start-Sleep -Seconds 5 }
}
if (-not $installed) { throw 'SteamCMD could not install Space Engineers Dedicated Server (298740).' }

# Retain the whole binary directory, including native/transitive dependencies,
# but exclude the large Content directory, worlds, and the Steam installation.
Copy-Item -Path (Join-Path $serverRoot 'DedicatedServer64/*') -Destination $Destination -Recurse -Force
$manifest = Join-Path $serverRoot 'steamapps/appmanifest_298740.acf'
if (-not (Test-Path -LiteralPath $manifest)) { throw "Steam app manifest missing: $manifest" }
Copy-Item -LiteralPath $manifest -Destination (Join-Path $Destination 'appmanifest_298740.acf')
$manifestText = Get-Content -LiteralPath $manifest -Raw
$buildId = [regex]::Match($manifestText, '"buildid"\s+"(\d+)"').Groups[1].Value
if (-not $buildId) { throw 'Steam app manifest did not contain a build ID.' }
$size = (Get-ChildItem -LiteralPath $Destination -File -Recurse | Measure-Object Length -Sum).Sum
Write-Host "Downloaded Steam build $buildId. Cached binary directory: $([math]::Round($size / 1MB, 1)) MiB."
& (Join-Path $PSScriptRoot 'Set-GameReferences.ps1') -BinaryPath $Destination
