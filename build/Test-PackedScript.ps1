param(
    [Parameter(Mandatory = $true)]
    [string] $BinaryPath,
    [string] $ScriptPath = (Join-Path (Split-Path -Parent $PSScriptRoot) 'EasyCommands/bin/Release/MDK/EasyCommands/script.cs')
)
$ErrorActionPreference = 'Stop'
$repoRoot = Split-Path -Parent $PSScriptRoot
$BinaryPath = (Resolve-Path -LiteralPath $BinaryPath).Path
$script = [IO.File]::ReadAllText((Resolve-Path -LiteralPath $ScriptPath).Path)
if ([string]::IsNullOrWhiteSpace($script)) { throw 'The packaged script is empty.' }
if ($script.Length -gt 100000) { throw "Packaged script exceeds 100,000 characters: $($script.Length)" }
Write-Host "Packaged script: $($script.Length) characters; $(100000 - $script.Length) characters remaining."
Write-Host "SHA256: $((Get-FileHash -LiteralPath $ScriptPath -Algorithm SHA256).Hash)"

$checkRoot = Join-Path $repoRoot '.build/packed-script-check'
New-Item -ItemType Directory -Force -Path $checkRoot | Out-Null
# Compile the exact artifact body in a programmable-block-style wrapper,
# using the same imports as the source project. This does not run the game.
$usings = Get-Content (Join-Path $repoRoot 'EasyCommands/Program.cs') | Where-Object { $_ -match '^using .*;$' }
$source = ($usings -join "`n") + "`nnamespace IngameScript { public class Program : MyGridProgram {`n" + $script + "`n} }`n"
Set-Content -LiteralPath (Join-Path $checkRoot 'Program.cs') -Value $source -Encoding utf8
@"
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net48</TargetFramework>
    <LangVersion>6</LangVersion>
    <PlatformTarget>x64</PlatformTarget>
    <GenerateAssemblyInfo>false</GenerateAssemblyInfo>
    <GenerateTargetFrameworkAttribute>false</GenerateTargetFrameworkAttribute>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="Mal.Mdk2.References" Version="2.2.9" PrivateAssets="all" />
    <PackageReference Include="Mal.Mdk2.PbAnalyzers" Version="2.2.0" PrivateAssets="all" />
  </ItemGroup>
</Project>
"@ | Set-Content -LiteralPath (Join-Path $checkRoot 'PackedScript.csproj') -Encoding utf8
Set-Content -LiteralPath (Join-Path $checkRoot 'PackedScript.mdk.local.ini') -Value "[mdk]`nbinarypath=$BinaryPath`n" -Encoding utf8
& dotnet build (Join-Path $checkRoot 'PackedScript.csproj') --configuration Release --verbosity minimal
if ($LASTEXITCODE -ne 0) { throw 'The minified script failed its standalone compilation check.' }
