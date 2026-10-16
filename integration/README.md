# Dedicated-server integration tests

The harness ships independently of the Apex Survival feature branch. The existing mocked tests
remain independent; this harness runs the exact packaged Release script in the game.

## Run

From this worktree, in PowerShell on Windows:

    ./build/Test-InGame.ps1 -Suite smoke
    ./build/Test-InGame.ps1 -Suite apex -ScriptPath 'D:\Artifacts\apex-script.cs'

To rerun one scenario with existing builds:

    ./build/Test-InGame.ps1 -Suite apex -ScriptPath 'D:\Artifacts\apex-script.cs' -Scenario seed-alias-transfer -SkipBuild

The Apex suite requires a script built with the Apex Survival handlers (PR #281).
Use -ScriptPath to supply that artifact while the feature is still separate.

The first run downloads the full dedicated server using the existing SteamCMD helper
(app 298740). Allow disk space for the full installation and network access. You need
the .NET SDK and .NET Framework 4.8 targeting pack described in build/README.md.

The runner builds EasyCommands and the plugin against that server, copies its shipped
Empty World into a unique .build/integration directory, enables in-game scripts and
unattended simulation, then starts a separate server bound to 127.0.0.1:27026.
Do not run two suites simultaneously on the same machine/port.
Each scenario recompiles the full script; allow several minutes for the Apex suite.
The default wall-clock timeout is ten minutes; progress.json identifies the current scenario.

The harness creates a connected static platform, charged battery, programmable block,
light, Farm Plot, Algae Farm, Food Processor, Irrigation System, and adjacent cargo
containers. It seeds the source with five fruit seeds and 99 fruit as a decoy. No
Workshop upload or player connection is required. The suite selects the scenarios
independently of the existing unit suite.

Every scenario gets freshly compiled EasyCommands code, cleared PB storage, its own
Custom Data, a unique pass/fail token, and a simulation-tick deadline. Scenarios run
sequentially on the same fixture; the seed-transfer case intentionally runs once.
Each whole invocation starts from a fresh world.

## Existing installation or custom setups

    ./build/Test-InGame.ps1 -Suite apex -ScriptPath 'D:\Artifacts\apex-script.cs' -ServerRoot 'D:\SE-TestServer'

For a hand-built or saved regression setup:

    ./build/Test-InGame.ps1 -Suite apex -ScriptPath 'D:\Artifacts\apex-script.cs' -ServerRoot 'D:\SE-TestServer' -TemplateWorld 'D:\Worlds\ApexFixture'

Supplying TemplateWorld disables automatic fixture spawning. Supply one powered,
working programmable block named EC Test PB plus exactly one block for each
RequiredBlocks name in the selected JSON suite. Use a disposable fixture save.
For the transfer test, start with five fruit seeds in source cargo and none in
destination cargo, with a working transfer path. Every test starts from the
template copy; the original save is never edited.

To test another script revision, build/run from this worktree after bringing those
commits onto this branch. To add a setup, save a world and add a scenario JSON file
under integration/scenarios, then select its filename (without .json) with -Suite.
A scenario supplies Name, RequiredBlocks, TimeoutTicks, and EasyCommands Commands.
Commands must print EC_PASS only after assertions succeed, or EC_FAIL with a reason.
All required names must be unique across the world.

Use -SkipBuild to repeat tests with the existing built script/plugin. Use
-ScriptPath 'D:\Artifacts\script.cs' to deploy a separately packaged update;
metadata records the hash of that exact file.

## Results and debugging

Each .build/integration/<run-id> retains request.json (including deployed source),
metadata.json (script SHA-256 and server version), fixture.json, progress.json, results.json, world, and server logs.
A missing block, compile error, script exception, explicit failure, tick timeout,
server exit, or wall-clock timeout fails the runner. Failed runs remain available.
The runner stops only the process it launched; it forcibly stops the disposable world
after results because that world is not reused.

The initial Apex suite covers selector/control paths, reading algae production output,
exact seed alias transfers, empty crop state, and food recipe queueing. It does not yet prove planting/harvest behavior,
long-duration crop growth, completed food production, or save/reload persistence.
Those need additional fixtures and assertions.

If results never arrive, inspect server logs for Harness plugin loading. The runner
uses the vanilla dedicated server -plugin argument. Game updates can change plugin
support/API behavior; successful compilation alone does not prove a live test passed.
