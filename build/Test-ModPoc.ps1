param([string] $ServerRoot, [string] $StagedMod, [string] $PythonPath, [switch] $Benchmark, [switch] $CompatibilityOnly, [switch] $SkipFullBatch, [switch] $LifecycleOnly, [switch] $SchedulingOnly,
    [ValidatePattern('^[a-zA-Z0-9_-]+$')][string] $Suite = 'smoke', [string[]] $ScenarioName)
$ErrorActionPreference = 'Stop'
$repo = Split-Path -Parent $PSScriptRoot
if (-not $ServerRoot) {
    $ini = Get-Content (Join-Path $repo 'EasyCommands/EasyCommands.mdk.local.ini') -ErrorAction SilentlyContinue |
        Where-Object { $_ -match '^binarypath=' } | Select-Object -First 1
    if ($ini) { $ServerRoot = Split-Path -Parent ($ini -replace '^binarypath=', '') }
    else { $ServerRoot = Join-Path $repo '.build/dedicated-server' }
}
if (-not $StagedMod) {
    & (Join-Path $repo 'build/Build-ModPoc.ps1') -ServerRoot $ServerRoot -PythonPath $PythonPath
    $latest = Get-ChildItem -LiteralPath (Join-Path $repo '.build/mod-poc/staged') -Directory |
        Sort-Object LastWriteTimeUtc -Descending | Select-Object -First 1
    $StagedMod = Join-Path $latest.FullName 'Mods/EasyCommands'
}
dotnet build (Join-Path $repo 'integration/Harness/Harness.csproj') -c Release "-p:GameBin=$(Join-Path $ServerRoot 'DedicatedServer64')" --nologo
if ($LASTEXITCODE) { throw 'Harness build failed.' }
& (Join-Path $repo 'build/Test-InGame.ps1') -Suite $Suite -ServerRoot $ServerRoot `
    -StagedMod $StagedMod -ScriptPath (Join-Path $repo 'EasyCommands.Bridge/script.cs.txt') `
    -RunRoot (Join-Path $repo '.build/mod-poc/runs') -PocBenchmark:($Benchmark -or $CompatibilityOnly -or $LifecycleOnly -or $SchedulingOnly) -PocSkipSamples:($CompatibilityOnly -or $LifecycleOnly -or $SchedulingOnly) -PocSchedulingOnly:$SchedulingOnly -PocLifecycleOnly:$LifecycleOnly -PocSkipFullBatch:$SkipFullBatch -ScenarioName $ScenarioName -SkipBuild
