param(
    [string] $ServerRoot,
    [string] $TemplateWorld,
    [ValidatePattern('^[a-zA-Z0-9_-]+$')][string] $Suite = 'smoke',
    [string] $GameBin,
    [ValidateRange(10,3600)][int] $TimeoutSeconds = 600,
    [switch] $SkipBuild,
    [string] $ScriptPath,
    [string] $StagedMod,
    [string] $RunRoot,
    [switch] $PocBenchmark,
    [switch] $PocMissingMod,
    [switch] $PocSkipSamples,
    [switch] $PocSkipFullBatch,
    [ValidateRange(0,100)][int] $StressBlocks = 0,
    [ValidateRange(120,3600)][int] $StressTicks = 1200,
    [Alias('Scenario')][string[]] $ScenarioName
)
$ErrorActionPreference = 'Stop'
if ($StressBlocks -and ($PocBenchmark -or $PocMissingMod)) { throw 'Stress mode cannot be combined with the other PoC test modes.' }
$repo = Split-Path -Parent $PSScriptRoot
$defaultServer = [IO.Path]::GetFullPath((Join-Path $repo '.build/dedicated-server'))
if (-not $ServerRoot) { $ServerRoot = $defaultServer }
$ServerRoot = [IO.Path]::GetFullPath($ServerRoot)
$serverExe = Join-Path $ServerRoot 'DedicatedServer64/SpaceEngineersDedicated.exe'
if (-not (Test-Path -LiteralPath $serverExe)) {
    if ($ServerRoot -ne $defaultServer) { throw "Dedicated server missing: $serverExe" }
    & (Join-Path $repo 'build/Get-GameReferences.ps1')
}
if (-not $TemplateWorld) { $TemplateWorld = Join-Path $ServerRoot 'Content/CustomWorlds/Empty World' }
if (-not $GameBin) { $GameBin = Join-Path $ServerRoot 'DedicatedServer64' }
$TemplateWorld = (Resolve-Path -LiteralPath $TemplateWorld).Path
if (-not (Test-Path -LiteralPath (Join-Path $TemplateWorld 'Sandbox.sbc'))) { throw 'TemplateWorld must be a saved world containing Sandbox.sbc.' }
$suitePath = Join-Path $repo "integration/scenarios/$Suite.json"
$scenarios = @(Get-Content -LiteralPath $suitePath -Raw | ConvertFrom-Json)
if ($ScenarioName) {
    foreach ($name in $ScenarioName) {
        if ($scenarios.Name -notcontains $name) { throw "Unknown scenario: $name" }
    }
    $scenarios = @($scenarios | Where-Object { $_.Name -in $ScenarioName })
}
if ($scenarios.Count -eq 0) { throw 'Suite contains no scenarios.' }
foreach ($scenario in $scenarios) {
    if (-not $scenario.Name -or -not $scenario.Commands -or $scenario.TimeoutTicks -lt 1) { throw 'Invalid scenario.' }
}
if (-not $SkipBuild) {
    & (Join-Path $repo 'build/Set-GameReferences.ps1') -BinaryPath $GameBin
    dotnet build (Join-Path $repo 'EasyCommands/EasyCommands.csproj') -c Release -p:Platform=x64
    if ($LASTEXITCODE) { throw 'EasyCommands build failed.' }
    dotnet build (Join-Path $repo 'integration/Harness/Harness.csproj') -c Release "-p:GameBin=$GameBin"
    if ($LASTEXITCODE) { throw 'Harness build failed.' }
}
if (-not $ScriptPath) { $ScriptPath = Join-Path $repo 'EasyCommands/bin/Release/MDK/EasyCommands/script.cs' }
if (-not (Test-Path -LiteralPath $scriptPath)) { throw "Packaged script missing: $scriptPath" }
$ScriptPath = (Resolve-Path -LiteralPath $ScriptPath).Path
$scriptText = Get-Content -LiteralPath $ScriptPath -Raw
if ([string]::IsNullOrWhiteSpace($scriptText) -or $scriptText.Length -gt 100000) { throw 'Script must contain 1â€“100,000 characters.' }
if (-not $RunRoot) { $RunRoot = Join-Path $repo '.build/integration' }
$RunRoot = [IO.Path]::GetFullPath($RunRoot)
$run = Join-Path $RunRoot ([guid]::NewGuid().ToString('N'))
$world = Join-Path $run 'Saves/TestWorld'
New-Item -ItemType Directory -Force -Path $world | Out-Null
Copy-Item -Path (Join-Path $TemplateWorld '*') -Destination $world -Recurse
if ($StagedMod) {
    $StagedMod = (Resolve-Path -LiteralPath $StagedMod).Path
    $stagedRoot = Split-Path -Parent (Split-Path -Parent $StagedMod)
    foreach ($evidence in @('source-manifest.json', 'staged.diff')) {
        $evidencePath = Join-Path $stagedRoot $evidence
        if (Test-Path -LiteralPath $evidencePath) { Copy-Item -LiteralPath $evidencePath -Destination $run }
    }
    # Dedicated servers reject unpublished local packages. The opt-in test
    # bootstrap compiles these files with the real game's Mod target instead.
    $modDirectory = Join-Path $run 'SourceMod/EasyCommandsPoc'
    New-Item -ItemType Directory -Force -Path $modDirectory | Out-Null
    Copy-Item -LiteralPath (Join-Path $StagedMod 'Data') -Destination $modDirectory -Recurse
}
# Generate private disposable-server settings; never reuse a production config.
$onlineMode = 'PRIVATE'
$benchmarkWatcherSettings = if ($PocBenchmark -or $StressBlocks) { '<AutoRestartEnabled>false</AutoRestartEnabled><WatcherInterval>300</WatcherInterval><WatcherSimulationSpeedMinimum>0</WatcherSimulationSpeedMinimum>' } else { '' }
[xml]$config = @"
<MyConfigDedicated>
  <SessionSettings>
    <GameMode>Creative</GameMode><OnlineMode>$onlineMode</OnlineMode>
    <EnableIngameScripts>true</EnableIngameScripts><PauseGameWhenEmpty>false</PauseGameWhenEmpty>
    <EnableCopyPaste>true</EnableCopyPaste><EnableSpectator>true</EnableSpectator>
    <TrashRemovalEnabled>false</TrashRemovalEnabled><AutoSaveInMinutes>0</AutoSaveInMinutes>
  </SessionSettings>
  <LoadWorld>$([System.Security.SecurityElement]::Escape($world))</LoadWorld>
  <IP>127.0.0.1</IP><ServerPort>27026</ServerPort><SteamPort>8767</SteamPort>
  <ServerName>EasyCommands Integration</ServerName><WorldName>EC Test World</WorldName>
  <ExperimentalMode>true</ExperimentalMode><PauseGameWhenEmpty>false</PauseGameWhenEmpty>
  $benchmarkWatcherSettings
</MyConfigDedicated>
"@
$config.Save((Join-Path $run 'SpaceEngineers-Dedicated.cfg'))
# Saved-world settings also need overriding; LoadWorld may take precedence over server settings.
foreach ($settingsFile in @('Sandbox.sbc', 'Sandbox_config.sbc')) {
    $settingsPath = Join-Path $world $settingsFile
    if (-not (Test-Path -LiteralPath $settingsPath)) { continue }
    [xml]$saved = Get-Content -LiteralPath $settingsPath -Raw
    $settings = $saved.DocumentElement.SelectSingleNode('Settings')
    foreach ($pair in @{ EnableIngameScripts='true'; PauseGameWhenEmpty='false'; GameMode='Creative'; OnlineMode=$onlineMode; TrashRemovalEnabled='false'; AutoSaveInMinutes='0' }.GetEnumerator()) {
        $node = $settings.SelectSingleNode($pair.Key)
        if (-not $node) { $node = $saved.CreateElement($pair.Key); $settings.AppendChild($node) | Out-Null }
        $node.InnerText = $pair.Value
    }
    $saved.Save($settingsPath)
}
$requestData = @{ CreateFixture = (-not $PSBoundParameters.ContainsKey('TemplateWorld')); Script = $scriptText; ProgrammableBlock = 'EC Test PB'; Scenarios = $scenarios }
if ($StagedMod) { $requestData.ModPocSourceDirectory = Join-Path $modDirectory 'Data/Scripts/EasyCommands' }
if ($PocBenchmark) {
    $requestData.PocBenchmark = $true
    $requestData.PocSkipSamples = [bool]$PocSkipSamples
    $requestData.PocSkipFullBatch = [bool]$PocSkipFullBatch
    $requestData.FullScript = [IO.File]::ReadAllText((Join-Path $repo 'EasyCommands/bin/Release/MDK/EasyCommands/script.cs'))
}
if ($PocMissingMod) { $requestData.PocMissingMod = $true }
if ($StressBlocks) { $requestData.StressBlocks = $StressBlocks; $requestData.StressTicks = $StressTicks }
$requestData |
    ConvertTo-Json -Depth 10 | Set-Content -LiteralPath (Join-Path $run 'request.json') -Encoding UTF8
$steamBuildId = $null
$manifestPath = Join-Path $ServerRoot 'steamapps/appmanifest_298740.acf'
if (Test-Path -LiteralPath $manifestPath) {
    $steamBuildId = [regex]::Match((Get-Content -LiteralPath $manifestPath -Raw), '"buildid"\s+"(\d+)"').Groups[1].Value
}
@{ SteamBuildId = $steamBuildId; ScriptSha256 = (Get-FileHash -LiteralPath $scriptPath).Hash; ScriptCharacters = $scriptText.Length; Suite = $Suite; Scenarios = @($scenarios.Name); StartedUtc = [DateTime]::UtcNow.ToString('o'); SourceMode = $(if ($StagedMod) { 'ModCompilerTestBootstrap' } else { 'None' }); PocBenchmark = [bool]$PocBenchmark
   ServerVersion = (Get-Item -LiteralPath $serverExe).VersionInfo.FileVersion } |
    ConvertTo-Json | Set-Content -LiteralPath (Join-Path $run 'metadata.json')
$plugin = Join-Path $repo 'integration/Harness/bin/Release/net48/Harness.dll'
if (-not (Test-Path -LiteralPath $plugin)) { throw "Harness plugin missing: $plugin. Run without -SkipBuild first." }
$previousRoot = $env:EC_TEST_ROOT
$process = $null
try {
    $env:EC_TEST_ROOT = $run
    $process = Start-Process -FilePath $serverExe -ArgumentList @('-console', '-ignorelastsession', '-path', ('"' + $run + '"'), '-plugin', ('"' + $plugin + '"')) -WorkingDirectory (Split-Path -Parent $serverExe) -WindowStyle Hidden -PassThru
    $deadline = [DateTime]::UtcNow.AddSeconds($TimeoutSeconds)
    $resultPath = Join-Path $run 'results.json'
    while (-not (Test-Path -LiteralPath $resultPath)) {
        if ($process.HasExited) { throw "Server exited before results. Logs: $run" }
        if ([DateTime]::UtcNow -gt $deadline) { throw "Server timed out. Check plugin loading and logs: $run" }
        Start-Sleep -Milliseconds 500
    }
    $results = @(Get-Content -LiteralPath $resultPath -Raw | ConvertFrom-Json)
    $results | Format-Table Name, Passed, Ticks
    Write-Host "Artifacts: $run"
    if ((-not $PocBenchmark -and -not $StressBlocks -and $results.Count -ne $scenarios.Count) -or @($results | Where-Object { -not $_.Passed }).Count) { throw "Integration suite failed. Results: $resultPath" }
    if ($PocBenchmark -and -not $PocSkipSamples) { & (Join-Path $repo 'build/Summarize-ModPoc.ps1') -RunPath $run }
} finally {
    $env:EC_TEST_ROOT = $previousRoot
    # This process owns only the disposable world; preserve its directory for diagnostics.
    if ($process -and -not $process.HasExited) { Stop-Process -Id $process.Id -Force }
}
