# EasyCommands mod runtime proof of concept: results

Measured 2026-10-19 on the local dedicated-server build `24675709`, logged game version `01_210_014`. This is an isolated runtime experiment; normal mod-package loading and interactive single-player validation remain pending.

Follow-up: [concurrent workload stress tests](mod-stress-results.md) passed for ten full-script PBs, ten mod-backed PBs, and fifty mod-backed PBs, including simultaneous program execution and incremental reloads.

**The existing interpreter runs through a 1,747-character PB bridge after compilation by the game's real Mod-target compiler.** The existing light-toggle scenario passed through this bridge. The engine's parsing, execution, variables, queues, and block handlers reside in a separately sandbox-compiled assembly.

## PB installation and startup

Five warm samples per variant, preceded by one first sample of each in the same process. These are not independent cold-process samples. Server-side installation times include the synchronous `ProgramData` assignment and constructor work; editor interaction and network delivery are excluded. The command program is `print "POC_READY"`, with the existing parser settings.

| Measurement | Empty PB | Bridge + mod runtime | Full EasyCommands PB |
| --- | ---: | ---: | ---: |
| PB source characters | 56 | 1,747 | 96,043 |
| Warm installation median | 22.51 ms | **39.47 ms** | **23,677.89 ms** |
| Warm installation range | 22.07–23.76 ms | 36.42–48.97 ms | 23,512.58–23,869.90 ms |
| Warm first invocation median | 0.60 ms | 3.45 ms | 18.40 ms |
| Warm installation through program-ready median | 23.44 ms | 51.01 ms | 23,762.42 ms |
| Steady print-command call median, 100 warm calls | 0.0020 ms | 0.0490 ms | 0.0428 ms |

The bridge's first sample installed in 66.40 ms; its first interpreter invocation took 300.44 ms. Including readiness polling, installation through the first small program's output was **418.77 ms**. Subsequent installations reused already-initialized code and were much cheaper. The bridge is **98.18% smaller**, and its warm PB installation median is approximately **600 times faster** in this experiment.

The separate shared mod source compilation took **14.34 seconds** for 66 source files. This cost occurs once in the test process and is excluded from per-PB installation. It is an actual Mod-target compile measurement from the test bootstrap, not a complete normal-world-loading measurement. Do not describe the first world load as a 39 ms operation.

Readiness wall times depend on scheduling, especially while the dedicated server recovers from long compilation pauses. Both engine variants reached this tiny program's marker in two observed update ticks. Larger programs still require parsing; this experiment does not establish a general parsing speedup. Steady execution of the tiny print command was similar and slightly slower through the bridge. The proven benefit is removing repeated compilation of the large engine and its PB source-size constraint.

## Runtime checks and scaling

The corrected compatibility suite exercises independent global variables in two PBs, nested A-to-B calls and a guarded A-to-B-to-A call, source changes, recompilation/re-registration, timed waits, functions and async queues, disable/resume, vanilla IGC on two antenna-connected grids 100 metres apart, ten-PB batches, and block removal.

All nine corrected compatibility/scaling checks passed. The ten-PB bridge batch installed in **396.79 ms total**, spent **35.52 ms total** in its explicit first calls, and had all ten small programs complete within **451.16 ms** including installation. The full-script batch spent **236.75 seconds** installing and **188.33 ms** in explicit first calls; all ten programs were ready after **237.48 seconds**. The shared mod compile is excluded from both batch wall times.

A separate missing-mod control also passed: the bridge displayed its unavailable/incompatible message, rejected the submitted argument explicitly, and did not execute Custom Data.

The existing standalone full-script light-toggle smoke test passed again in 16 execution ticks, confirming the ordinary harness path remains operational. The interpreter's production source was unchanged.

## What was actually loaded

The dedicated server rejects unpublished local mod packages with `Local mods are not allowed in multiplayer.` The installed game's world-source path also fails in `MyScriptManager.GetAssemblyName` on its base context. These failures happened before mod compilation and are retained in the research output.

The test harness therefore submits the staged `.cs` files to the same `MyVRage.Platform.Scripting.CompileAsync(MyApiTarget.Mod, ...)` entry point used by the mod loader and registers the resulting session component through the public session API. The whitelist and mod instrumentation remain enabled. The interpreter executes from that separately compiled assembly; it is not compiled into the privileged harness DLL.

This establishes sandbox compilation and runtime interoperability. It does **not** establish ordinary Workshop/local-mod discovery, normal mod-context lifecycle initialization, client joining, or a production authorization boundary. A normal local-mod folder is staged for a separate manual single-player packaging check. No Workshop item or player save was modified.

## Evidence

- Initial sandbox light-toggle success: `.build/mod-poc/runs/41d2ca0825554733bfedb56c7e5c8269/`.
- Complete 18-sample timing dataset: `.build/mod-poc/runs/a2b084d09cfb418c9a8880d58381bff6/`. All timing samples passed. A later nested-call fixture failed parsing; that failure remains in `results.json` and was corrected for the separate compatibility run.
- Corrected compatibility/scaling run: `.build/mod-poc/runs/6593618f929e459bb6ffe4dd79f8b93b/`.
- Missing-mod control: `.build/mod-poc/controls/14925d56358f4c9c948b7ba59678a64f/`.
- Standalone regression control: `.build/mod-poc/controls/b1fd3f79db4a4467b216a6fe57cfa959/`.
- Fresh staged package: `.build/mod-poc/staged/2a67a7911b1a49339979e3af599b8875/Mods/EasyCommandsPoc/`. All 66 source files were verified byte-for-byte against the compatibility run's retained source.

`request.json` retains exact PB source. `mod-compile.json` retains compiler target, diagnostics, source count, and elapsed time. `measurements.jsonl` retains raw samples. `metrics-summary.json` and `retained-source-hashes.json` are generated by [Summarize-ModPoc.ps1](Summarize-ModPoc.ps1). Timing completeness and functional success are reported separately.

An earlier repeated-compile run triggered the dedicated server's slow-simulation watchdog. Benchmark mode now uses a zero slow-simulation floor and a 300-second watchdog interval in its disposable configuration; the runner still enforces a wall-clock deadline. This allows intentional repeated full-script compilation stalls to be measured without changing normal integration settings.

## Reproduce and next steps

See [the runnable instructions](../integration/ModPoc/README.md), [the bridge body](prototypes/easycommands-mod-bridge.cs.txt), and [the implementation plan](mod-poc-plan.md).

The standalone interpreter source was not edited. Three staging adaptations replace PB hosting, restore the static active context around nested calls, and route PB invocation through mod `Run`. Release work still includes explicit execution budgets, complete access validation and lifecycle cleanup, wider handler/client coverage, normal source-mod package validation, and a proper shared core with explicit instance context.
