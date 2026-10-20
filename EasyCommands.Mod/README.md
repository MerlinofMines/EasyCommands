# EasyCommands Mod

EasyCommands Mod runs the shared EasyCommands interpreter on the world/server. A small
programmable-block bridge supplies the PB context, update callbacks and IGC, and emits
output through native PB Echo. Keep your EasyCommands program in the PB's Custom Data.
The standalone script and mod compile the same parser, handlers, variables and threads.

## Install

Build with `build/Build-Mod.ps1 -GameBin <game-assembly-directory> -PythonPath <python>`.
The command prints a fresh output directory under ignored `.build/mod/staged/` containing:

- `EasyCommands.zip`: mod content, with `Data` at the archive root.
- `EasyCommands-Bridge.zip`: PB script content, with `script.cs` at the archive root.
- `Mods/EasyCommands` and `IngameScripts/local/EasyCommands Bridge`: unpacked local items.
- `bridge.cs.txt`: the exact PB body for pasting directly into a block.
- `release-manifest.json` and `source-manifest.json`: protocol, revision and file hashes.

For a local world, copy `Mods/EasyCommands` into `%APPDATA%/SpaceEngineers/Mods` and enable
it in the world's Mods settings. Enable in-game scripts and Experimental mode. Paste the
bridge into a powered PB, put your commands in Custom Data and Run it. The unpacked bridge
folder can also be copied into `%APPDATA%/SpaceEngineers/IngameScripts/local` for browsing
from the PB script editor. Restart the world when replacing mod source.

## Settings and behavior

The bridge exposes the same defaults for these settings as the standalone script:

```csharp
public UpdateFrequency updateFrequency = UpdateFrequency.Update1;
public LogLevel logLevel = LogLevel.INFO;
```

Update1/10/100 control both incremental parsing and execution cadence. None is manual
execution. INFO includes parsing and execution status; SCRIPT_ONLY retains script output
and errors. Output is replayed through individual native PB Echo calls with the game's
usual display limits. LCD and other block handlers run in the shared engine.

The API is `EasyCommands.Bridge.v3`. Upgrade the mod and bridge together when that API
changes. A missing or incompatible mod stops automatic scheduling and reports an error;
Run again after enabling the matching mod. PB power loss pauses its normal update schedule.
Closing a PB removes its mod runtime. Interpreter state is held in memory and does not
persist across world restarts. Runtime execution is server-side.

## Build, test and publish

The source package uses the mod project's evaluated Compile items and copies them without
rewriting the engine. `Build-Mod.ps1` compiles the target both normally and with the compatibility imports
injected by the game loader, then validates the deployable archives and release hashes. CI builds both editions and checks the bridge's PB API use.
See [build instructions and Steam publishing](../build/README.md#mod-and-bridge-packaging-and-steam-publishing)
and [mod integration tests](../integration/Mod/README.md).

PB API imports are repeated inside engine namespaces so the normal mod loader's
outer imports cannot introduce ambiguous PB/Mod API types. Root imports remain for
standalone MDK packaging; mod-only host imports stay inside their namespaces. The live tests
apply the installed game's actual preprocessing before sandbox compilation.
