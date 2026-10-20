# Mod integration tests

The test harness runs the mod interpreter through the PB bridge in a hidden, disposable
dedicated server. Prerequisites: .NET Framework 4.8 build tools, PowerShell 7, Python 3,
and a dedicated server installation containing DedicatedServer64 and Content.

```powershell
# Build/package and run the light-toggle scenario.
./build/Test-Mod.ps1 -ServerRoot '<server>' -PythonPath '<python.exe>'

# Reuse a package for compatibility, lifecycle, cadence and output checks.
./build/Test-Mod.ps1 -ServerRoot '<server>' -StagedMod '<package>/Mods/EasyCommands' -CompatibilityOnly -SkipStandaloneBatch

# Focused checks.
./build/Test-Mod.ps1 -ServerRoot '<server>' -StagedMod '<package>/Mods/EasyCommands' -LifecycleOnly
./build/Test-Mod.ps1 -ServerRoot '<server>' -StagedMod '<package>/Mods/EasyCommands' -SchedulingOnly

# Concurrent workloads, including incremental reloads while other PBs run.
./build/Test-ModStress.ps1 -ServerRoot '<server>' -StagedMod '<package>/Mods/EasyCommands' -Blocks 50
```

`-Suite` and `-ScenarioName` select the existing scenario files. `-Benchmark` adds repeated
empty/bridge/standalone installation and readiness timings. `-SkipStandaloneBatch` omits
the expensive ten-PB standalone compilation batch. Timing runs disable the disposable
server's slow-simulation watchdog; the wall-clock timeout still applies.

The tests cover independent engines and nested PB calls, Custom Data edits, recompilation,
async functions and waits, disable/resume, cross-grid IGC, removal, battery power recovery,
warhead destruction and fresh replacement state. The recompile check alternates ten manual\nand automatic starts from completed runtimes, verifies fresh state and nonblank INFO output,\nand records first-output latency in `recompile.json`. Scheduling checks include None and
Update1/10/100, plus exact terminal-output comparisons against standalone PBs at INFO and
SCRIPT_ONLY. The missing-mod runner uses `Test-InGame.ps1 -MissingMod` without a staged mod.

## Test loading and limits

`ModSourceLoader.cs` is test-only. It first applies the installed game's actual
MyScriptManager.UpdateCompatibility preprocessing, including injected Mod API imports
and legacy substitutions, then compiles with MyApiTarget.Mod into a separate sandboxed
assembly and registers the session component. This works around dedicated-server rejection of unpublished local packages.
It validates mod-whitelisted compilation and execution, including native PB bridge calls;
it does not establish normal Workshop download/package discovery or multiplayer joining.
`mod-compile.json` records `PackageLoadingVerified: false`. Normal local/Workshop loading
must also be exercised in a world using the unpacked or subscribed item.

Each run retains exact inputs, hashes, compiler diagnostics, server logs and JSON checks
under ignored `.build/mod/runs/`; concurrent tests use `.build/mod/stress/`.
`Summarize-ModBenchmark.ps1 -RunPath <run>` summarizes timing samples and retained inputs.
Results and investigation notes are generated local artifacts, not tracked source.
The runner owns and stops only its disposable server and never modifies a player world.
