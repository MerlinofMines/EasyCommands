# EasyCommands concurrent PB stress results

Measured 2026-10-19 with dedicated-server Steam build `24675709`, game `01_210_014`,
on the local Ryzen 7 1700X machine. All three final runs passed: ten original full-script
PBs, ten mod-backed PBs, and fifty mod-backed PBs. The ten-PB variants used byte-identical
EasyCommands program inputs and produced **identical final assertion results**, including
cycle counts, checksums, message counts, nested-call counts, instance sentinels and tick count.

**Preserve incremental parsing.** The user explicitly accepts parsing over several ticks
to protect other running PBs. This test keeps `commandParseAmount = 1` and
`commandParameterParseAmount = 100` in both variants; no eager parser was introduced.

## Installation versus execution

| Measurement | 10 full-script PBs | 10 bridge + mod PBs | 50 bridge + mod PBs |
| --- | ---: | ---: | ---: |
| PB source installation, total | 234.004 s | 0.536 s | 2.284 s |
| Steady combined script time, mean/update | 0.520 ms | 0.466 ms | 2.271 ms |
| Steady combined script time, 95th percentile | 0.573 ms | 0.517 ms | 3.994 ms |
| Steady combined script time, maximum | 4.287 ms | 6.931 ms | 5.122 ms |
| Observed steady updates/second | 60.00 | 60.00 | 60.00 |
| Mean script time while reloading 20% of PBs | 0.605 ms | 0.496 ms | 2.380 ms |
| Reload combined script time, 95th percentile | 0.696 ms | 0.579 ms | 4.888 ms |
| Reload combined script time, maximum | 4.758 ms | 2.781 ms | 9.613 ms |
| Steady mean after reload | 0.517 ms | 0.450 ms | 2.240 ms |
| Final PB correctness checks | 10/10 passed | 10/10 passed | 50/50 passed |

These are combined durations for the entire PB group, not per-block times. Steady and
reload phases had no summed script-time sample above a 16.67 ms update budget. The steady
window covered 1,200 updates, followed by concurrent reloads and another 600-update window.
The first sample of each phase is omitted from aggregates. This is one final run of each
configuration, not enough to establish a small steady-runtime speed advantage. The important
result is comparable runtime cost and correct scaling, alongside much cheaper PB installation.

Installation means assigning instrumented PB source, including C# compilation and constructor
work. It excludes shared mod compilation, the bridge's first interpreter invocation, subsequent
EasyCommands parsing, network delivery, and editor interaction. The earlier
[installation benchmark](mod-poc-results.md) measures cold and warm first calls separately.

The shared engine compiled once per mod-backed test process: **14.44 seconds** for the ten-PB
run and **14.60 seconds** for the fifty-PB run. This was a real Mod-target compilation of all
66 staged source files. It was not repeated for each PB. In normal deployment this shared
compilation belongs to world/mod loading, potentially on clients as well as the server.
Our test bootstrap invokes it just after session loading; **normal package loading and complete
server-startup timing remain unverified**. Do not claim a measured 14-second total server startup.

## Correctness and concurrent parsing

Each PB runs a different synthetic program, drawn from five workload families: repeated
arithmetic, alternating conditional branches, list iteration, function calls, and timed
async/await work. Programs intentionally share global-variable names while using distinct
seeds and expected results. Unused functions add realistic parser work. The harness checks
checksums and monotonic progress throughout execution, then checks exact final counters and
per-instance sentinels. Every PB must make progress, including the slower list/async programs.

A plain C# peer sends targeted vanilla IGC broadcasts over powered antenna grids. Manual Run
requests and synchronous nested calls run alongside the main programs. The ten-PB run verified
1,917 message deliveries in the final program generations, with exact expected counters.
Programs are reloaded in 20 percent of the PBs while the remaining blocks continue to run;
reloaded programs have new seeds, new message tags, and additional parsed helper functions.

Initial parsing took 197 observed updates for the ten-PB groups and 207 for the fifty-PB group.
Concurrent reloads took 265 and 308 updates respectively, approximately 4.4 and 5.1 seconds
at 60 updates/second. These delays are compatible with the requested incremental behavior.
The retained raw progress and final checks show continued work in the other PBs during reload.

## Cold costs and limits

Cold behavior is not stall-free. The fifty-PB initial parsing phase had four summed script-time
samples above 16.67 ms, with a maximum of 39.48 ms. The first messaging/cross-PB warm-up had
one approximately 397 ms script-time sample; the ten-PB bridge and original variants also had
one such warm-up sample, approximately 394 and 440 ms respectively. The experiment does not
isolate which first-use path or JIT work caused that spike. It is separate from shared mod
compilation and from steady/reload execution. Initial PB activation/engine creation can add
other first-call costs excluded from these phase aggregates.

Both variants receive identical test-only Main-entry telemetry using the public
`Runtime.LastRunTimeMs` and `Storage` APIs. This includes synchronous mod work but reports the
previous call, so attribution lags by one invocation. Summing nested calls can double-count
inner durations. Telemetry adds overhead; wall-frame measurements also include harness polling,
logging, game scheduling, and unrelated server work. These are comparative elapsed-time
measurements, not exact CPU accounting or universal performance guarantees.

This was a short synthetic workload test in a small empty world, not a long soak or a complete
handler/real-ship-script compatibility suite. Fifty full-script PBs were not tested. Production
work remains: normal mod-package loading, client/lifecycle coverage, authorization, explicit
execution budgets, and replacing the temporary static active-context adaptation.

## Reproduction and evidence

Run the commands in [the test README](../integration/ModPoc/README.md). The runner is
[Test-ModStress.ps1](Test-ModStress.ps1); generated fixtures and assertions are in
[PocStress.cs](../integration/ModPoc/PocStress.cs). No production interpreter source was changed.

Retained runs under `.build/mod-poc/stress/`:

- Ten bridge PBs: `3316e1971fa0470b8e21b520141e94ba`.
- Ten full-script PBs: `3cc2bf39e6724837bb9bb09d1fd6de7c`.
- Fifty bridge PBs: `4e7d083261e446a1816dc20a6b125fc2`.

Each contains `request.json`, generated `stress-programs/`, `stress-instrumented.cs.txt`,
`stress-install.json`, raw `stress-metrics.jsonl`, aggregate `stress-summary.json`,
`stress-final-output.json`, and `results.json`. Mod-backed runs additionally retain the staged
source, manifest/diff, compiler diagnostics, and explicit `PackageLoadingVerified: false`.
The original release bridge remains 1,747 characters; stress instrumentation makes it 1,981.
The instrumented full interpreter is 96,277 characters.
