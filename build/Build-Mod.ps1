param([string] $ServerRoot, [string] $PythonPath, [string] $GameBin)
$ErrorActionPreference = 'Stop'
$repo = Split-Path -Parent $PSScriptRoot
if (-not $ServerRoot) {
    $ini = Get-Content (Join-Path $repo 'EasyCommands/EasyCommands.mdk.local.ini') -ErrorAction SilentlyContinue |
        Where-Object { $_ -match '^binarypath=' } | Select-Object -First 1
    if ($ini) { $ServerRoot = Split-Path -Parent ($ini -replace '^binarypath=', '') }
    else { $ServerRoot = Join-Path $repo '.build/dedicated-server' }
}
if (-not $GameBin) { $GameBin = Join-Path $ServerRoot 'DedicatedServer64' }
if (-not (Test-Path -LiteralPath (Join-Path $GameBin 'Sandbox.Game.dll'))) {
    throw "Dedicated server is missing: $gameBin"
}
$GameBin = (Resolve-Path -LiteralPath $GameBin).Path
$run = Join-Path $repo ('.build/mod-poc/staged/' + [guid]::NewGuid().ToString('N'))
if (-not $PythonPath) {
    $pythonCommand = Get-Command python -ErrorAction SilentlyContinue
    if ($pythonCommand) { $PythonPath = $pythonCommand.Source }
}
if ($PythonPath) { $pythonTool = Get-Command $PythonPath -ErrorAction SilentlyContinue; if ($pythonTool) { $PythonPath = $pythonTool.Source } }
if (-not $PythonPath -or -not (Test-Path -LiteralPath $PythonPath) -or $PythonPath -match '[\\/]Microsoft[\\/]WindowsApps[\\/]') { throw 'Pass -PythonPath with a Python 3 executable, not the Windows Store execution alias.' }
$project = Join-Path $repo 'EasyCommands.Mod/EasyCommands.Mod.csproj'
New-Item -ItemType Directory -Path $run -Force | Out-Null
$compileItems = & dotnet msbuild $project "-p:GameBin=$GameBin" -getItem:Compile -nologo
if ($LASTEXITCODE) { throw 'Cannot evaluate mod compile source items.' }
$compileItemsPath = Join-Path $run 'compile-items.json'
$compileItems | Set-Content -LiteralPath $compileItemsPath -Encoding utf8
& $PythonPath (Join-Path $repo 'build/Stage-Mod.py') --repo $repo --output $run --compile-items $compileItemsPath
if ($LASTEXITCODE) { throw 'Mod staging failed.' }
$source = Join-Path $run 'Mods/EasyCommands/Data/Scripts/EasyCommands'
dotnet build (Join-Path $repo 'EasyCommands.Mod/EasyCommands.Mod.csproj') -c Release "-p:GameBin=$gameBin" --nologo
if ($LASTEXITCODE) { throw 'Mod source compile check failed.' }
Compress-Archive -Path (Join-Path $run 'Mods/EasyCommands/*') -DestinationPath (Join-Path $run 'EasyCommands.zip')
Write-Host "Staged mod: $(Join-Path $run 'Mods/EasyCommands')"
Write-Host "Manifest: $(Join-Path $run 'source-manifest.json')"
