param(
    [Parameter(Mandatory = $true)]
    [string] $BinaryPath
)
$ErrorActionPreference = 'Stop'
$BinaryPath = (Resolve-Path -LiteralPath $BinaryPath).Path
foreach ($assembly in @('Sandbox.Common.dll', 'Sandbox.Game.dll', 'SpaceEngineers.Game.dll', 'VRage.Game.dll', 'VRage.Math.dll')) {
    if (-not (Test-Path -LiteralPath (Join-Path $BinaryPath $assembly))) {
        throw "Missing game reference: $assembly in $BinaryPath"
    }
}
$repoRoot = Split-Path -Parent $PSScriptRoot
foreach ($project in @('EasyCommands', 'EasyCommands.Tests')) {
    # MDK2 2.2.9 uses the project-specific filename for reference discovery.
    $settings = "[mdk]`nbinarypath=$BinaryPath`noutput=auto`ninteractive=DoNothing`n"
    Set-Content -LiteralPath (Join-Path $repoRoot "$project/$project.mdk.local.ini") -Value $settings -Encoding utf8
}
Write-Host "Both projects will use game assemblies from $BinaryPath"
