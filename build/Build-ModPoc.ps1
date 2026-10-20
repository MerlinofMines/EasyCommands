param([string] $ServerRoot, [string] $PythonPath)
& (Join-Path $PSScriptRoot 'Build-Mod.ps1') @PSBoundParameters
