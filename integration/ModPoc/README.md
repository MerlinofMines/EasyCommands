> Current implementation: the source transformations have been removed. See
> `EasyCommands.Mod/README.md`. Both targets compile the same engine directly;
> packaging reads the mod project's evaluated Compile items and verifies byte hashes.
> The reports below describe the earlier PoC where identified.

# EasyCommands mod runtime proof of concept

This experiment stages the existing interpreter as mod source and connects it to a small programmable-block bridge. The normal EasyCommands project remains the standalone edition. Generated adaptations and run evidence live under ignored `.build/mod-poc/`.

The bridge is [easycommands-mod-bridge.cs.txt](../../build/prototypes/easycommands-mod-bridge.cs.txt). It discovers `EasyCommands.Bridge.v1`, supplies the current PB invocation context, and applies the returned update frequency/output. The mod owns the actual parser, commands, handlers, variables, and queues. This is a synchronous, PB-driven interpreter; it does not introduce a background scheduler.

## Build and run

Prerequisites are the repository's .NET Framework 4.8/.NET SDK build environment, PowerShell 7, Python 3 for source staging, and the local dedicated-server installation already used by integration tests. Pass a real Python executable rather than the Windows Store execution alias.

```powershell
# Stages a fresh package and runs the existing light-toggle scenario through it.
pwsh -NoProfile -File build/Test-ModPoc.ps1 -PythonPath 'C:\path\to\python.exe'

# Adds repeated installation/readiness timings and compatibility scenarios.
pwsh -NoProfile -File build/Test-ModPoc.ps1 -PythonPath 'C:\path\to\python.exe' -Benchmark

# Re-run functional checks and ten-PB batches without repeating timing samples.
pwsh -NoProfile -File build/Test-ModPoc.ps1 -StagedMod '<staged>/Mods/EasyCommands' -CompatibilityOnly

# Existing broader scenarios through the mod-backed interpreter.
pwsh -NoProfile -File build/Test-ModPoc.ps1 -StagedMod '<staged>/Mods/EasyCommands' -Suite apex
# Optional -ScenarioName selects individual cases from the suite.
```

Both scripts accept `-ServerRoot` for the directory containing `DedicatedServer64` and `Content`. By default they resolve the configured `binarypath` in `EasyCommands/EasyCommands.mdk.local.ini`. `Build-Mod.ps1` can be run independently to create a staged mod and perform an ordinary C# compile check. `Test-ModPoc.ps1 -StagedMod '<staged>/Mods/EasyCommands'` reuses an existing package and needs no Python invocation.

The test runner starts a hidden, disposable dedicated server using port 27026, copies the template world, and stops only its own server process. Use this port when it is free. No Workshop upload or player save is involved. A successful invocation prints the retained artifact directory.

Benchmark mode sets the disposable server's slow-simulation watchdog minimum to zero and its interval to 300 seconds. Repeated full-script compilations intentionally block the simulation long enough to trigger the ordinary watchdog; the runner's wall-clock timeout still bounds the experiment. This setting is not applied to normal integration runs.

## What the automatic loading test establishes

The installed dedicated server rejects unpublished local mod packages before compilation. Its world-source path also encounters a null-context game bug. `ModPocSourceLoader.cs`, compiled into the **test harness**, therefore submits the staged `.cs` files to the game's actual `MyApiTarget.Mod` compiler and registers the resulting session component through the public session API.

The engine remains in a separate assembly compiled with the game's mod whitelist and instrumentation. The harness supplies fixtures and measurement. It does not compile the engine into its own privileged assembly, disable the whitelist, or inject the interpreter into PB source.

This validates sandbox compilation and execution through the PB/mod delegate boundary. It **does not validate normal Workshop/local-mod package discovery**, client joining, or source-mod lifecycle initialization in the normal loader. `mod-compile.json` explicitly records `PackageLoadingVerified: false`. Keep this distinction when quoting results.

For a separate manual packaging check, use the generated `Mods/EasyCommands` folder as a normal local mod in a disposable offline single-player world, place a powered PB, paste the bridge body, and add `print "Hello from the mod"` to Custom Data. This interactive check has not been performed by the automated test.

## Evidence and interpretation

Every run retains its PB source in `request.json`, compiler diagnostics/time in `mod-compile.json`, server logs, `results.json`, and the copied source files under `SourceMod/`. Benchmark mode adds raw `measurements.jsonl` and `poc-progress.json`. New runs also copy the staging manifest and generated diff.

`build/Summarize-ModPoc.ps1 -RunPath '<run>'` writes timing statistics and byte hashes of the retained source inputs. It reports timing completeness separately from functional-suite success, so a later scenario failure cannot be hidden by completed timing samples.

The benchmark alternates empty/bridge/full scripts over six rounds: one first sample and five warm samples of each within one process. These are not independent cold-process samples. Installation is the timed `ProgramData` assignment, including compilation and any constructor work. Engine initialization moves to the bridge's first call, which is timed separately. Readiness waits for an actual output marker. Twenty verified steady calls are measured per sample. Ten-PB batches verify the scaling effect.

The staged engine preserves its default parser settings. The small readiness program is `print "POC_READY"`; larger user programs can take longer to parse. Compiler time measured by the test bootstrap is not a complete normal-world-startup benchmark.

Compatibility checks cover separate globals in two instances, nested PB calls, source edits, bridge recompilation, waits/functions/async queues, disabled/resumed execution, cross-grid vanilla IGC, and block removal. The IGC fixture uses two powered antenna grids 100 metres apart with a plain C# PB peer.

## Concurrent workload stress test

`build/Test-ModStress.ps1 -Blocks 10 -Ticks 1200 -Variant bridge -StagedMod '<staged>/Mods/EasyCommands'`
runs distinct arithmetic, conditional, list iteration, function, and timed async programs together.
Use `-Variant full` without `-StagedMod` for the original packed interpreter, or `-Blocks 50`
for the larger mod-backed run. Run servers sequentially because they share the disposable server port.

Each PB has a different seed and program, but intentionally shares variable names with the others.
The harness checks independently calculated checksums throughout execution, exact IGC delivery and
manual/nested call counts at completion, unique instance sentinels, and progress for every PB.
It reloads larger Custom Data programs in 20 percent of PBs while all others continue to execute.
Incremental parsing is an explicit requirement: accepting several ticks of parsing is preferable
to stalling the other PBs. The existing parser budgets are preserved in both compared variants.
The antenna fixtures span separate grids; a plain C# peer sends targeted broadcast messages.

The retained `stress-programs/`, `stress-install.json`, `stress-metrics.jsonl`, `stress-summary.json`,
and `results.json` contain the inputs, measurements, and assertions. Both PB variants receive identical
test-only Main-entry instrumentation using the public `Runtime.LastRunTimeMs` and `Storage` APIs.
This measures previous-call elapsed time, including the synchronous mod delegate, with a one-call lag.
The first frame of each phase is omitted to limit boundary attribution; the earlier installation
benchmark separately measures the cold first interpreter invocation. These phase aggregates do not
include that complete first-invocation cost.
The added telemetry has overhead, and summing nested calls can double-count their inner duration;
these are comparative workload measurements, not exact server CPU accounting. Harness wall-frame
intervals include game scheduling, fixture observation and file logging. Idle, parsing, cold calls,
steady execution and concurrent reloads are reported separately. Stress mode also adjusts the
disposable watchdog to tolerate the full-script installation batch; the outer timeout remains active.

See `build/mod-stress-results.md` for measured outcomes and limitations.

## Production follow-up

The permanent shared-source proposal is in `build/mod-shared-core-plan.md`; broader existing
handler-suite results and the corrected crop assertion are in `build/mod-broad-results.md`.

This is a private-fixture proof of concept. Release work still includes explicit execution budgets, complete authorization of mod operations, comprehensive handler/client/lifecycle coverage, normal mod-package validation, and replacing the temporary static active-context adaptation with an explicit per-instance execution context. Runtime execution state is not persisted across restarts.

Power interruption/recovery and actual warhead-destruction cleanup are now verified:
see [lifecycle results](../../build/mod-lifecycle-results.md). Use `-LifecycleOnly` for the focused battery/warhead test.
The complete compatibility run includes these checks after its earlier scenarios.
