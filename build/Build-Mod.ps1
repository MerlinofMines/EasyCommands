param([string] $ServerRoot, [string] $PythonPath, [string] $GameBin, [string] $OutputDirectory)
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
if (-not $OutputDirectory) { $OutputDirectory = Join-Path $repo ('.build/mod/staged/' + [guid]::NewGuid().ToString('N')) }
$run = [IO.Path]::GetFullPath($OutputDirectory)
if ((Test-Path -LiteralPath $run) -and @(Get-ChildItem -LiteralPath $run -Force).Count) { throw 'OutputDirectory must be empty.' }
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
Compress-Archive -Path (Join-Path $run 'IngameScripts/local/EasyCommands Bridge/*') -DestinationPath (Join-Path $run 'EasyCommands-Bridge.zip')
$protocol = [regex]::Match([IO.File]::ReadAllText((Join-Path $run 'bridge.cs.txt')), 'const string ApiId = "([^"]+)";').Groups[1].Value
$revision = & git -C $repo "-c" "safe.directory=$repo" rev-parse HEAD
if ($LASTEXITCODE) { throw 'Cannot record source revision.' }
$changes = & git -C $repo "-c" "safe.directory=$repo" status --porcelain --untracked-files=normal
if ($LASTEXITCODE) { throw 'Cannot record source status.' }
$artifacts = @('EasyCommands.zip','EasyCommands-Bridge.zip','bridge.cs.txt','source-manifest.json') | ForEach-Object {
    @{ File = $_; Sha256 = (Get-FileHash -LiteralPath (Join-Path $run $_) -Algorithm SHA256).Hash }
}
@{ SchemaVersion = 1; ApiId = $protocol; SourceRevision = [string]$revision; WorkingTreeModified = [bool]$changes; Artifacts = @($artifacts) } |
    ConvertTo-Json -Depth 5 | Set-Content -LiteralPath (Join-Path $run 'release-manifest.json') -Encoding utf8
& $PythonPath (Join-Path $repo 'build/Test-ModPackage.py') --repo $repo --package $run
if ($LASTEXITCODE) { throw 'Mod/bridge package validation failed.' }
Write-Host "Staged mod: $(Join-Path $run 'Mods/EasyCommands')"
Write-Host "Manifest: $(Join-Path $run 'source-manifest.json')"

Write-Host "Staged PB bridge: $(Join-Path $run 'IngameScripts/local/EasyCommands Bridge')"
Write-Host "Release identity: $(Join-Path $run 'release-manifest.json')"
