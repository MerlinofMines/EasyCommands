param(
    [Parameter(Mandatory = $true)][string] $BinaryPath,
    [Parameter(Mandatory = $true)][UInt64] $WorkshopId,
    [Parameter(Mandatory = $true)][string] $ContentPath,
    [Parameter(Mandatory = $true)][string] $ReleaseNotesPath,
    [switch] $PrivateTest,
    [ValidateSet('Private', 'Unlisted')][string] $TestVisibility = 'Private',
    [switch] $BuildOnly
)
$ErrorActionPreference = 'Stop'
if ($WorkshopId -eq 0 -or ($PrivateTest -and $WorkshopId -eq 2698315573) -or
    (-not $PrivateTest -and $WorkshopId -ne 2698315573)) { throw 'Workshop ID is not allowed in this publishing mode.' }
if (-not $PrivateTest -and $PSBoundParameters.ContainsKey('TestVisibility')) { throw '-TestVisibility requires -PrivateTest.' }
$BinaryPath = (Resolve-Path -LiteralPath $BinaryPath).Path
$ContentPath = (Resolve-Path -LiteralPath $ContentPath).Path
$ReleaseNotesPath = (Resolve-Path -LiteralPath $ReleaseNotesPath).Path
$toolRoot = Join-Path (Split-Path -Parent $PSScriptRoot) '.build/workshop-api'
New-Item -ItemType Directory -Path $toolRoot -Force | Out-Null
foreach ($library in @('Steamworks.NET.dll', 'steam_api64.dll')) {
    Copy-Item -LiteralPath (Join-Path $BinaryPath $library) -Destination $toolRoot -Force
}
$compiler = Join-Path $env:WINDIR 'Microsoft.NET/Framework64/v4.0.30319/csc.exe'
$executable = Join-Path $toolRoot 'WorkshopUpload.exe'
& $compiler /nologo /target:exe /platform:x64 "/out:$executable" "/reference:$toolRoot/Steamworks.NET.dll" (Join-Path $PSScriptRoot 'WorkshopUpload.cs')
if ($LASTEXITCODE -ne 0) { throw 'Could not compile the Steam API upload helper.' }
[IO.File]::WriteAllText((Join-Path $toolRoot 'steam_appid.txt'), '244850')
if ($BuildOnly) { Write-Host "Built Steam API helper: $executable"; return }
$mode = if ($PrivateTest) { 'private-test' } else { 'production' }
$visibility = if ($PrivateTest) { $TestVisibility } else { 'Preserve' }
Push-Location $toolRoot
try {
    & $executable ([string]$WorkshopId) $ContentPath $ReleaseNotesPath $mode $visibility
    if ($LASTEXITCODE -ne 0) { throw 'Steam API upload failed. Inspect the item before retrying.' }
}
finally { Pop-Location }
