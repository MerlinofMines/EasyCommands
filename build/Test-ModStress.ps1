param(
    [ValidateRange(1,100)][int] $Blocks = 10,
    [ValidateRange(120,3600)][int] $Ticks = 1200,
    [ValidateSet('bridge','full')][string] $Variant = 'bridge',
    [string] $ServerRoot,
    [string] $StagedMod
)
$ErrorActionPreference = 'Stop'
$repo = Split-Path -Parent $PSScriptRoot
if (-not $ServerRoot) {
    $ini = Get-Content (Join-Path $repo 'EasyCommands/EasyCommands.mdk.local.ini') |
        Where-Object { $_ -match '^binarypath=' } | Select-Object -First 1
    if (-not $ini) { throw 'Specify ServerRoot.' }
    $ServerRoot = Split-Path -Parent ($ini -replace '^binarypath=', '')
}
$options = @{}
if ($Variant -eq 'bridge') {
    if (-not $StagedMod) { throw 'Specify a validated StagedMod directory.' }
    $options.StagedMod = $StagedMod
    $options.ScriptPath = Join-Path $repo 'EasyCommands.Bridge/script.cs.txt'
} else {
    $options.ScriptPath = Join-Path $repo 'EasyCommands/bin/Release/MDK/EasyCommands/script.cs'
}
dotnet build (Join-Path $repo 'integration/Harness/Harness.csproj') -c Release "-p:GameBin=$(Join-Path $ServerRoot 'DedicatedServer64')" --nologo
if ($LASTEXITCODE) { throw 'Harness build failed.' }
& (Join-Path $repo 'build/Test-InGame.ps1') -Suite smoke -ServerRoot $ServerRoot @options `
    -RunRoot (Join-Path $repo '.build/mod-poc/stress') -StressBlocks $Blocks -StressTicks $Ticks -TimeoutSeconds 1200 -SkipBuild
