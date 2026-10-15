# Building EasyCommands

EasyCommands targets .NET Framework 4.8 and uses MDK2 for programmable-block
analysis and Release script packaging. Install a .NET 8+ SDK and the .NET Framework
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

Without the game installed, run `./build/Get-GameReferences.ps1` once. This uses
SteamCMD's anonymous login to install dedicated server app 298740 and configures
both projects to use `.build/game-references`. It does not start a server.
Normal `dotnet build` and `dotnet test` never invoke SteamCMD.

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
files. Large game content, worlds, and SteamCMD are excluded. Only the packaged
script and test results are published as workflow artifacts.

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
