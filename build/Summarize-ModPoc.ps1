param([Parameter(Mandatory = $true)][string] $RunPath)
$ErrorActionPreference = 'Stop'
$RunPath = (Resolve-Path -LiteralPath $RunPath).Path
function Get-TimingStatistics([double[]] $Values) {
    $sorted = @($Values | Sort-Object)
    if (-not $sorted.Count) { return $null }
    $middle = [int][Math]::Floor($sorted.Count / 2)
    $median = if ($sorted.Count % 2) { $sorted[$middle] } else { ($sorted[$middle - 1] + $sorted[$middle]) / 2 }
    return [ordered]@{ Count = $sorted.Count; MedianMs = [Math]::Round($median, 4); MinMs = [Math]::Round($sorted[0], 4); MaxMs = [Math]::Round($sorted[-1], 4) }
}
$rows = @(Get-Content -LiteralPath (Join-Path $RunPath 'measurements.jsonl') | ForEach-Object { $_ | ConvertFrom-Json } | Where-Object { $_.Kind -in @('sample', 'batch') })
$variants = [ordered]@{}
foreach ($name in @('empty', 'bridge', 'full')) {
    $samples = @($rows | Where-Object { $_.Kind -eq 'sample' -and $_.Variant -eq $name })
    $warm = @($samples | Where-Object { -not $_.FirstInProcessForVariant })
    $variants[$name] = [ordered]@{
        FirstSample = ($samples | Where-Object FirstInProcessForVariant | Select-Object -First 1)
        WarmInstall = Get-TimingStatistics @($warm | ForEach-Object { $_.InstallMs })
        WarmFirstCall = Get-TimingStatistics @($warm | ForEach-Object { $_.FirstCallMs })
        WarmReadyAfterCall = Get-TimingStatistics @($warm | ForEach-Object { $_.ReadyAfterCallMs })
        WarmInstallThroughReady = Get-TimingStatistics @($warm | ForEach-Object { $_.InstallMs + $_.ReadyAfterCallMs })
        WarmSteadyCall = Get-TimingStatistics @($warm | ForEach-Object { $_.SteadyCallMs })
    }
}
$resultsPath = Join-Path $RunPath 'results.json'
$results = if (Test-Path -LiteralPath $resultsPath) { @(Get-Content -LiteralPath $resultsPath -Raw | ConvertFrom-Json) } else { @() }
$request = Get-Content -LiteralPath (Join-Path $RunPath 'request.json') -Raw | ConvertFrom-Json
$compile = Get-Content -LiteralPath (Join-Path $RunPath 'mod-compile.json') -Raw | ConvertFrom-Json
$timingComplete = (@($rows | Where-Object Kind -eq sample).Count -eq 18 -and @($variants.Values | Where-Object { $_.WarmInstall.Count -ne 5 }).Count -eq 0)
$summary = [ordered]@{
    RunPath = $RunPath
    PackageLoadingVerified = $false
    Completed = ($results.Count -gt 0 -and @($results | Where-Object { -not $_.Passed }).Count -eq 0 -and ($request.PocSkipSamples -or $timingComplete))
    CompatibilityOnly = [bool]$request.PocSkipSamples
    TimingSamplesComplete = $timingComplete
    PassedChecks = @($results | Where-Object Passed).Count
    FailedChecks = @($results | Where-Object { -not $_.Passed })
    BridgeCharacters = $request.Script.Length
    FullScriptCharacters = $request.FullScript.Length
    ModCompile = $compile
    Variants = $variants
    Batches = @($rows | Where-Object Kind -eq batch)
    SamplingNote = 'One process; first sample per variant plus five warm samples. Not independent cold-process measurements. Readiness uses a one-line print program.'
}
$summary | ConvertTo-Json -Depth 12 | Set-Content -LiteralPath (Join-Path $RunPath 'metrics-summary.json') -Encoding utf8
# Hash the exact retained inputs, independently of any earlier staging manifest.
$inputHashes = @()
if ($request.ModPocSourceDirectory -and (Test-Path -LiteralPath $request.ModPocSourceDirectory)) {
    $sourceRoot = [IO.Path]::GetFullPath($request.ModPocSourceDirectory)
    $inputHashes = @(Get-ChildItem -LiteralPath $sourceRoot -Filter '*.cs' -File -Recurse | Sort-Object FullName | ForEach-Object {
        [ordered]@{ Source = $_.FullName.Substring($sourceRoot.Length).TrimStart('\', '/'); Sha256 = (Get-FileHash -LiteralPath $_.FullName -Algorithm SHA256).Hash }
    })
}
$inputHashes | ConvertTo-Json -Depth 4 | Set-Content -LiteralPath (Join-Path $RunPath 'retained-source-hashes.json') -Encoding utf8
$variants.GetEnumerator() | ForEach-Object {
    [PSCustomObject]@{ Variant = $_.Key; WarmSamples = $_.Value.WarmInstall.Count; InstallMedianMs = $_.Value.WarmInstall.MedianMs; FirstCallMedianMs = $_.Value.WarmFirstCall.MedianMs; InstallThroughReadyMedianMs = $_.Value.WarmInstallThroughReady.MedianMs }
} | Format-Table -AutoSize
Write-Host "Completed: $($summary.Completed); passed checks: $($summary.PassedChecks); summary: $(Join-Path $RunPath 'metrics-summary.json')"
