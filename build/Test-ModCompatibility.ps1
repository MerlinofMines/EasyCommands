param(
    [Parameter(Mandatory = $true)][string] $GameBin,
    [Parameter(Mandatory = $true)][string] $SourceDirectory
)
$ErrorActionPreference = 'Stop'
$repo = Split-Path -Parent $PSScriptRoot
$GameBin = (Resolve-Path -LiteralPath $GameBin).Path
$SourceDirectory = (Resolve-Path -LiteralPath $SourceDirectory).Path
$check = Join-Path $repo ('.build/mod/compatibility/' + [guid]::NewGuid().ToString('N'))
# Normal MyScriptManager loading prepends these imports. The live harness also
# calls the actual game's UpdateCompatibility, including its legacy substitutions.
$prefix = @('using VRage;', 'using VRage.Game.Components;', 'using VRage.ObjectBuilders;',
    'using VRage.ModAPI;', 'using VRage.Game.ModAPI;', 'using Sandbox.Common.ObjectBuilders;',
    'using VRage.Game;', 'using Sandbox.ModAPI;', 'using VRage.Game.ModAPI.Interfaces;',
    'using SpaceEngineers.Game.ModAPI;', '#line 1') -join "`n"
$sourceRoot = Join-Path $check 'Sources'
Get-ChildItem -LiteralPath $SourceDirectory -Filter '*.cs' -File -Recurse | ForEach-Object {
    $relative = $_.FullName.Substring($SourceDirectory.Length).TrimStart('\', '/')
    $destination = Join-Path $sourceRoot $relative
    New-Item -ItemType Directory -Path (Split-Path -Parent $destination) -Force | Out-Null
    [IO.File]::WriteAllText($destination, $prefix + "`n" + [IO.File]::ReadAllText($_.FullName))
}
[xml]$modProject = Get-Content -LiteralPath (Join-Path $repo 'EasyCommands.Mod/EasyCommands.Mod.csproj') -Raw
$references = foreach ($group in $modProject.Project.ItemGroup) {
    foreach ($reference in $group.Reference) {
        if ($null -eq $reference) { continue }
        $hint = ([string]$reference.HintPath).Replace('$(GameBin)', $GameBin)
        $name = [Security.SecurityElement]::Escape([string]$reference.Include)
        $hint = [Security.SecurityElement]::Escape($hint)
        "<Reference Include=`"$name`"><HintPath>$hint</HintPath><Private>false</Private></Reference>"
    }
}
$project = @"
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net48</TargetFramework>
    <LangVersion>6</LangVersion>
    <PlatformTarget>x64</PlatformTarget>
    <EnableDefaultCompileItems>false</EnableDefaultCompileItems>
    <GenerateAssemblyInfo>false</GenerateAssemblyInfo>
    <NoWarn>CS0105</NoWarn>
    <GenerateTargetFrameworkAttribute>false</GenerateTargetFrameworkAttribute>
  </PropertyGroup>
  <ItemGroup>
    <Compile Include="Sources/**/*.cs" />
    $($references -join "`n")
  </ItemGroup>
</Project>
"@
$projectPath = Join-Path $check 'ModCompatibility.csproj'
[IO.File]::WriteAllText($projectPath, $project)
& dotnet build $projectPath -c Release --nologo --verbosity minimal
if ($LASTEXITCODE) { throw 'Mod sources fail with the normal game-loader compatibility imports.' }
Write-Host 'Normal mod-loader compatibility imports compile successfully.'
