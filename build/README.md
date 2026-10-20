# Building EasyCommands

EasyCommands targets .NET Framework 4.8 and uses MDK2 for programmable-block
analysis and Release script packaging. Install PowerShell 7 (`pwsh` on PATH),
a .NET 8+ SDK and the .NET Framework
4.8 targeting pack on Windows (Visual Studio 2022 includes the targeting pack).
NuGet still supplies MDK2, MSTest, and Moq. Game DLLs
are read from a local game/server installation; no game-binary NuGet package is used.

## Local build

MDK2 can discover a normal Steam game installation automatically. To select a
particular installation, run from the repository root:

```powershell
./build/Set-GameReferences.ps1 -BinaryPath 'C:\Program Files (x86)\Steam\steamapps\common\SpaceEngineers\Bin64'
dotnet build EasyCommands.Tests/EasyCommands.Tests.csproj -c Release -p:Platform=x64
dotnet test EasyCommands.Tests/EasyCommands.Tests.csproj -c Release -p:Platform=x64 --no-build --no-restore
```

The setup script writes ignored, project-specific `*.mdk.local.ini` files for
both projects. Use the same binary directory for production and test code.
The Release build packages the script under `EasyCommands/bin/Release/MDK`.
Debug builds skip packaging for faster test iteration.

After MDK packaging, `Redact-ScriptVisibility.ps1` uses PowerShell's bundled Roslyn
parser to remove redundant `public` modifiers from direct members of the generated
`Program`. It preserves the constructor, `Main`/`Save`, configuration fields,
overrides, and public members inside nested types. Other syntax tokens must remain
identical or the build fails. The source assembly retains its API for unit tests.
Run `./build/Test-ScriptVisibility.ps1` to check these preservation rules.

Without the game installed, run `./build/Get-GameReferences.ps1` once. This uses
SteamCMD's anonymous login to install dedicated server app 298740 and configures
both projects to use `.build/game-references`. It does not start a server.
Normal `dotnet build` and `dotnet test` never invoke SteamCMD.

## Publish the existing Steam Workshop script locally

`Publish-Workshop.ps1` builds Release, checks the exact packaged script with
`Test-PackedScript.ps1`, and uploads through your local Steam session. It is
locked to [EasyCommands item 2698315573](https://steamcommunity.com/sharedfiles/filedetails/?id=2698315573).
It requires the `modinfo.sbmi` from the local script folder previously published
in-game, and refuses missing, zero, different, or multiple Workshop IDs. It stages
only that metadata, the freshly built `script.cs`, and `thumb.png` under ignored
`.build/workshop/`, leaving your original local script folder untouched.

One-time setup:

1. Install the game and .NET Framework 4.8. `Send-WorkshopUpdate.ps1` compiles
   `WorkshopUpload.cs` locally against the game's Steamworks.NET library.
   [SEWorkshopTool](https://github.com/Gwindalmir/SEWorkshopTool#installation)
   is needed only to create a new disposable test item; the downloaded copy on
   this computer is under ignored `.build/workshop-tool/SEWorkshopTool.exe`.
2. Run Steam and sign in as the account that owns the existing Workshop item.
   Steam ownership authorizes updates; having a copy of this script or knowing
   the Workshop ID does not grant publishing access. No credentials go in GitHub.
3. Find the original local script folder under
   `%APPDATA%\SpaceEngineers\IngameScripts\local`. Its `modinfo.sbmi` must contain
   Workshop ID 2698315573 (normally `WorkshopIds/WorkshopId/Id`). If the metadata
   is missing, recover the original
   publishing metadata before continuing; do not publish a new item to replace it.

First prepare and inspect the package without uploading:

```powershell
./build/Publish-Workshop.ps1 `
  -LocalScriptPath "$env:APPDATA\SpaceEngineers\IngameScripts\local\EasyCommands" `
  -BinaryPath 'C:\Program Files (x86)\Steam\steamapps\common\SpaceEngineers\Bin64' `
  -PrepareOnly
```

Then publish from the checkout you want to release:

```powershell
./build/Publish-Workshop.ps1 `
  -LocalScriptPath "$env:APPDATA\SpaceEngineers\IngameScripts\local\EasyCommands" `
  -BinaryPath 'C:\Program Files (x86)\Steam\steamapps\common\SpaceEngineers\Bin64' `
  -ChangeNote 'Release vX.Y.Z: describe changes here'
```

`-ChangeNote` publishes the text to Steam's Workshop change notes with the upload.
For multiline release notes, replace it with `-ReleaseNotesPath './release-notes.txt'`
(a UTF-8 text file). Supply only one of these options. The helper rejects empty
notes and prints the notes during preparation, so `-PrepareOnly` also previews
what will be posted. The notes file is kept outside the uploaded script content.

To publish a tagged release, check out that tag first. The helper builds the
current working tree, including local edits; the change note does not select a
Git revision. Run the unit tests before publishing. Check the uploader's output
and the existing Workshop page afterward to confirm the update. Live upload
requires local Steam authentication and is not exercised by automated tests.

### Disposable upload test

Use `-PrivateTest` without `-LocalScriptPath` to create a separate, explicitly
private item named `EasyCommands UPLOAD TEST`. Never copy production metadata
into a new test item. Add `-TestVisibility Unlisted` to verify change notes in
the browser: Steam hides change history for Private items, even from the signed-in
owner. Unlisted items are accessible to anyone with their link, but do not appear
in searches or on your profile. For example:

```powershell
./build/Publish-Workshop.ps1 -PrivateTest `
  -TestVisibility Unlisted `
  -BinaryPath 'C:\Program Files (x86)\Steam\steamapps\common\SpaceEngineers\Bin64' `
  -WorkshopToolPath './.build/workshop-tool/SEWorkshopTool.exe' `
  -ChangeNote "Private upload test`nSecond line of release notes"
```

The tool writes the new item's ID into `modinfo.sbmi` in the printed staging
folder. To verify updating that same test item, run again with `-PrivateTest`,
`-TestWorkshopId <new ID>`, and `-LocalScriptPath '<previous staging folder>'`.
Include `-TestVisibility Unlisted` again to keep it unlisted; test uploads default
to Private otherwise. Test mode refuses the production ID. Existing-item uploads
use Steam's update-only API, verify the signed-in owner and game before uploading,
and read back the final visibility. They cannot create a new item. Verify the content, intended
visibility, and both change notes on Steam before deleting this disposable item
through its Workshop owner's controls. Keep the test item until verification is
complete. `-PrepareOnly` never creates an item.

The test item retained ID 3821621809 across repeated uploads. Changing it from
Private to Unlisted revealed all five original upload history entries, including
multiline notes from SEWorkshopTool: the notes were saved while Private but hidden
on Steam's website. Existing published items retain their visibility; the
`-TestVisibility` option is accepted only in test mode. These tests did not update
the production item. SEWorkshopTool 0.7.20 also downgraded a requested Unlisted
upload to Private in this test. Existing-item uploads therefore use the direct
Steam API, which preserves production visibility and explicitly sets and verifies
Private or Unlisted in test mode. No Steam credentials are stored in the repo.
The final complete run of `Publish-Workshop.ps1` rebuilt and validated the script,
published revision 7 to that same test ID, and verified Unlisted visibility through
both the API and Steam's browser page, where the multiline notes were visible.

## GitHub Actions caching

The workflow restores `.build/game-references` using an exact key derived from
`build/space-engineers-cache-version.txt` and the download helper. The key does
not depend on the commit SHA, date, or latest Steam release.

- Cache hit: validate/configure the restored references, then build and test.
  SteamCMD is not invoked.
- Cache miss: anonymously download the dedicated server, keep its binary
  directory and Steam app manifest, then save the cache **before** building.
  Compilation/test failures therefore do not cause repeat downloads.
- To intentionally refresh references, increment the cache-version file and
  commit it. The next miss fetches the current public Steam build. Its build ID
  is printed in the workflow log and retained in the cached manifest.

Only `DedicatedServer64` is cached, including its native and transitive runtime
files. Large game content, worlds, and SteamCMD are excluded. The standalone script, mod/bridge release packages and test results are retained as workflow artifacts.

GitHub caches are disposable and branch-scoped: a new branch can restore its
own caches and default-branch caches, but not an unrelated branch's cache.
After this workflow is merged, the first default-branch run seeds the shared
cache. Eviction or a changed cache revision causes another download. The
revision is a refresh control, **not** a pin to an immutable Steam release:
a cache miss after eviction can fetch newer binaries for the same revision.

When validating a refresh, run the workflow twice and check that the second
run reports a cache hit and skips both download and save steps. The test suite
must pass and the `EasyCommands-script` artifact must contain `script.cs`.

## Checking the packaged script

The workflow also compiles the exact minified `script.cs` artifact inside a
programmable-block-style wrapper, with C# 6 and MDK2's game API analyzer. It
fails if the script exceeds 100,000 characters and prints its SHA-256 hash.
To run the same check locally after a Release build:

```powershell
./build/Test-PackedScript.ps1 -BinaryPath 'C:\Program Files (x86)\Steam\steamapps\common\SpaceEngineers\Bin64'
```

This verifies compilation and allowed API use, not execution inside a running
game. The unit suite exercises the original source against the same DLLs.

The separate `docs` job builds `docs/` with GitHub's Jekyll Pages builder on
pushes and pull requests. It does not deploy. Live Pages publication continues
from `currentRelease` and its `/docs` folder.

## In-game integration tests

See [integration/README.md](../integration/README.md) for the isolated dedicated-server runner and Apex suite.

## Mod and bridge packaging and Steam publishing

The standalone Workshop script remains item 2698315573. Publish **EasyCommands Mod** as
a separate world mod and **EasyCommands Bridge** as a separate in-game script. Users can
keep the standalone edition or enable the mod and load the bridge into their PBs.
Do not upload the mod or bridge to the standalone script's existing item.

```powershell
./build/Build-Mod.ps1 -GameBin '<game Bin64>' -PythonPath '<python.exe>'
```

The printed directory contains `EasyCommands.zip`, `EasyCommands-Bridge.zip`, unpacked
local-item folders, a pasteable bridge and release/source hash manifests. The mod archive
contains the engine and mod host under `Data/Scripts/EasyCommands`, plus the shared
thumbnail/license. The bridge archive contains `script.cs`, thumbnail and license.
Neither archive contains harness code, DLLs, run evidence or publishing identity.
Both are validated against the exact source inputs and the same bridge API identifier.
The build also compiles staged sources with the normal loader compatibility imports
through `Test-ModCompatibility.ps1`; imports inside the engine/host namespaces avoid
collisions between PB and Mod API interfaces. The live test runner additionally applies
the installed game's actual compatibility preprocessing.
CI retains both ZIPs and manifests in the `EasyCommands-mod` artifact. It never uploads
to Steam. `WorkingTreeModified` in the release manifest records builds with local edits;
for a release, build a clean tagged checkout and retain its manifests with the artifacts.

### First publication

1. Copy the generated `Mods/EasyCommands` folder into
   `%APPDATA%/SpaceEngineers/Mods/EasyCommands`. Copy the generated
   `IngameScripts/local/EasyCommands Bridge` folder into the matching directory under
   `%APPDATA%/SpaceEngineers`.
2. Test normal local-mod loading in a disposable single-player world: enable the mod,
   enable in-game scripts, load the bridge into a PB and run commands from Custom Data.
3. Sign in to Steam as the publishing account. In the world's Edit Settings > Mods,
   select the local EasyCommands mod and Publish to Steam. Publish the companion script
   from the PB script editor's local script browser. Give the two items distinct names
   and descriptions, and link each to the other. Start private/unlisted while validating
   the subscribed versions, then make the intended release public.
4. Retain each local item's generated `modinfo.sbmi` separately. Record its Steam item
   ID with the release. Those identities belong to different content types and must never
   be copied between the mod, bridge and standalone script.

Keen documents the local-mod publishing flow in its
[Workshop guide](https://www.spaceengineersgame.com/modding-guides/space-engineers-workshop-guide/).
Steam creates each item once, then updates it using that item's published ID; see the
[Steamworks publishing flow](https://partner.steamgames.com/doc/features/workshop/implementation?language=english).
New Workshop IDs are assigned at publication, so no IDs or placeholder metadata are
embedded in a build. The existing `Publish-Workshop.ps1` stays specific to the standalone
script; it is not the uploader for these new items.

### Updates

Build both packages from the release checkout. Copy new content into each original local
publishing folder while retaining **that folder's own** `modinfo.sbmi`, then republish from
the corresponding game browser. Keep stale source files out of the mod folder when files
are renamed or removed; the generated package itself is always fresh. Verify the ID and
release content on both Workshop pages. Retain release notes alongside the hash manifests.

For a compatible API, the mod and bridge can be updated independently. For an API change,
publish the matching pair as one coordinated release and tell users to reload the world
and recompile/update their PB bridges. Workshop subscription does not replace source
already loaded into a PB. A mismatched bridge stops with a clear compatibility message.
The bridge item description must tell users to enable the companion world mod; subscribing
to a script is not a substitute for adding that mod to the world's configuration.

Validate Workshop subscription/download, a new world, save/reload and client joining
before public release. The automated mod tests use a documented compiler bootstrap;
they do not cover normal Workshop package discovery. Publishing requires the local Steam
owner session. No Steam credentials or publishing metadata belong in CI or source control.
