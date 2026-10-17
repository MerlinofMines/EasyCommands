[CmdletBinding()]
param(
    # Folder originally published from the game's script browser.
    [string] $LocalScriptPath,
    [Parameter(Mandatory = $true)]
    [string] $BinaryPath,
    [string] $WorkshopToolPath,
    [string] $ChangeNote = 'EasyCommands update',
    [string] $ReleaseNotesPath,
    [switch] $PrivateTest,
    [UInt64] $TestWorkshopId = 0,
    [ValidateSet('Private', 'Unlisted')]
    [string] $TestVisibility = 'Private',
    [switch] $PrepareOnly
)
$ErrorActionPreference = 'Stop'
$repoRoot = Split-Path -Parent $PSScriptRoot
$workshopId = '2698315573'
if ($PrivateTest) {
    if ($TestWorkshopId -eq 2698315573) { throw 'Private testing cannot target the production Workshop item.' }
    $workshopId = [string] $TestWorkshopId
} elseif ($TestWorkshopId -ne 0) { throw '-TestWorkshopId requires -PrivateTest.' }
if (-not $PrivateTest -and $PSBoundParameters.ContainsKey('TestVisibility')) { throw '-TestVisibility requires -PrivateTest.' }
if ($ReleaseNotesPath) {
    if ($PSBoundParameters.ContainsKey('ChangeNote')) {
        throw 'Use either -ChangeNote or -ReleaseNotesPath, not both.'
    }
    $ChangeNote = Get-Content -LiteralPath (Resolve-Path -LiteralPath $ReleaseNotesPath).Path -Raw -Encoding UTF8
}
if ([string]::IsNullOrWhiteSpace($ChangeNote)) { throw 'Release notes must not be empty.' }
if (-not $LocalScriptPath -and (-not $PrivateTest -or $TestWorkshopId -ne 0)) {
    throw 'Supply -LocalScriptPath for an existing Workshop item.'
}
$metadataPath = $null
if ($LocalScriptPath) {
    $LocalScriptPath = (Resolve-Path -LiteralPath $LocalScriptPath).Path
    $metadataPath = Join-Path $LocalScriptPath 'modinfo.sbmi'
    if (-not (Test-Path -LiteralPath $metadataPath -PathType Leaf)) {
        throw 'Missing modinfo.sbmi. Select the local script folder previously published to Steam; refusing to create a new Workshop item.'
    }
    [xml] $metadata = Get-Content -LiteralPath $metadataPath -Raw
    $ids = @($metadata.SelectNodes("//*[local-name()='PublishedFileId'] | //WorkshopIds/WorkshopId/Id | /MyObjectBuilder_ModInfo/WorkshopId[not(*)]") | ForEach-Object { $_.InnerText } | Where-Object { $_ -ne '0' } | Select-Object -Unique)
    if ($PrivateTest -and $TestWorkshopId -eq 0) {
        throw 'Creating a private test item requires omitting -LocalScriptPath; existing metadata must never be reused.'
    }
    if ($ids.Count -ne 1 -or $ids[0] -ne $workshopId) {
        throw "Publishing metadata must identify only the existing EasyCommands item $workshopId. Found: $($ids -join ', ')"
    }
    $services = @($metadata.SelectNodes("//*[local-name()='PublishedServiceName' or local-name()='ServiceName']") | ForEach-Object { $_.InnerText })
    if (@($services | Where-Object { $_ -and $_ -ne 'Steam' }).Count -gt 0) {
        throw 'Publishing metadata identifies a service other than Steam.'
    }
}
if (-not $PrepareOnly -and $workshopId -eq '0') {
    if (-not $WorkshopToolPath) { throw 'Supply -WorkshopToolPath pointing to an installed SEWorkshopTool.exe.' }
    $WorkshopToolPath = (Resolve-Path -LiteralPath $WorkshopToolPath).Path
}

& dotnet build (Join-Path $repoRoot 'EasyCommands/EasyCommands.csproj') -c Release -p:Platform=x64
if ($LASTEXITCODE -ne 0) { throw 'Release build failed; nothing uploaded.' }
$packagePath = Join-Path $repoRoot 'EasyCommands/bin/Release/MDK/EasyCommands'
& (Join-Path $PSScriptRoot 'Test-PackedScript.ps1') -BinaryPath $BinaryPath -ScriptPath (Join-Path $packagePath 'script.cs')

# A unique staging folder avoids altering the original local script or retaining old files.
$itemName = if ($PrivateTest) { 'EasyCommands UPLOAD TEST' } else { 'EasyCommands' }
$stagePath = Join-Path $repoRoot ('.build/workshop/' + [guid]::NewGuid().ToString('N') + '/' + $itemName)
New-Item -ItemType Directory -Path $stagePath -Force | Out-Null
if ($metadataPath) { Copy-Item -LiteralPath $metadataPath -Destination $stagePath }
Copy-Item -LiteralPath (Join-Path $packagePath 'script.cs') -Destination $stagePath
Copy-Item -LiteralPath (Join-Path $packagePath 'thumb.png') -Destination $stagePath
# Keep the UTF-8 release notes outside the uploaded content folder.
$notesPath = Join-Path (Split-Path -Parent $stagePath) 'release-notes.txt'
[IO.File]::WriteAllText($notesPath, $ChangeNote, [Text.UTF8Encoding]::new($false))
if ($workshopId -eq '0') { Write-Host "Target: NEW disposable $TestVisibility test item" }
else { Write-Host "Target: https://steamcommunity.com/sharedfiles/filedetails/?id=$workshopId" }
Write-Host "Staged package: $stagePath"
Write-Host "Change note: $ChangeNote"
if ($PrepareOnly) {
    Write-Host 'Preparation complete. No Workshop tool was started and nothing was uploaded.'
    return
}

# SEWT creates new disposable items privately. Direct Steam updates preserve
# production visibility and support Unlisted without runtime method patching.
if ($workshopId -eq '0') {
    Push-Location (Resolve-Path -LiteralPath $BinaryPath).Path
    try {
        & $WorkshopToolPath push --scripts $stagePath --visibility Private
        if ($LASTEXITCODE -ne 0) { throw "SEWorkshopTool failed with exit code $LASTEXITCODE. Check its output before retrying." }
    }
    finally { Pop-Location }
    [xml] $created = Get-Content -LiteralPath (Join-Path $stagePath 'modinfo.sbmi') -Raw
    $createdIds = @($created.SelectNodes('//WorkshopIds/WorkshopId[ServiceName="Steam"]/Id') | ForEach-Object { $_.InnerText })
    if ($createdIds.Count -ne 1 -or $createdIds[0] -notmatch '^[1-9][0-9]*$' -or $createdIds[0] -eq '2698315573') {
        throw 'Could not identify the new Steam test item. Inspect the uploader output before retrying.'
    }
    $workshopId = $createdIds[0]
}
$updateOptions = @{
    BinaryPath = $BinaryPath; WorkshopId = [UInt64]$workshopId
    ContentPath = $stagePath; ReleaseNotesPath = $notesPath; PrivateTest = $PrivateTest
}
if ($PrivateTest) { $updateOptions.TestVisibility = $TestVisibility }
& (Join-Path $PSScriptRoot 'Send-WorkshopUpdate.ps1') @updateOptions
Write-Host "Verify the release notes: https://steamcommunity.com/sharedfiles/filedetails/changelog/$workshopId"
if ($PrivateTest) {
    Write-Host 'Steam hides change history for Private items, even from the owner. Use an Unlisted test item to verify notes in the browser.'
    Write-Host "Retain the staging folder's modinfo.sbmi to update this test item. Delete the item through Steam after verification."
}
