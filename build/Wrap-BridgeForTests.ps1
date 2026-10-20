param(
    [Parameter(Mandatory = $true)][string] $SourcePath,
    [Parameter(Mandatory = $true)][string] $OutputPath
)
$ErrorActionPreference = 'Stop'
$SourcePath = (Resolve-Path -LiteralPath $SourcePath).Path
$source = [IO.File]::ReadAllText($SourcePath)
$linePath = $SourcePath.Replace('\', '/')
$prefix = @'
// Generated test host. The shipped PB body below is included verbatim.
using System;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using VRage;
namespace EasyCommands.Bridge.UnitTests {
    public sealed class Program : MyGridProgram {
'@
$wrapped = $prefix + "`n#line 1 `"$linePath`"`n" + $source + "`n#line default`n    }`n}`n"
New-Item -ItemType Directory -Path (Split-Path -Parent $OutputPath) -Force | Out-Null
[IO.File]::WriteAllText($OutputPath, $wrapped)
