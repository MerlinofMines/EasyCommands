# Shared PB/mod host implementation validation

Implemented 2026-10-19 on `codex/easycommands-shared-mod` in its own managed worktree,
based on master merge `6861252`. AI support was already merged by PR #284 with successful
build/docs checks. This refactor adds no AI features and leaves the original local checkout intact.

## Implementation

The existing interpreter is compiled into the standalone PB and source mod. Only its host
initialization, active-context restoration and PB invocation seam changed. PB inheritance
and the parameterless constructor are in `EasyCommands/Hosting/PbProgram.cs`; mod inheritance
and the parameterized constructor are in `EasyCommands.Mod/Hosting/ModProgram.cs`.
The default incremental parser budgets remain 1 parsing task and 100 parameters per call.

`EasyCommands.Mod` is in the solution. `build/Set-GameReferences.ps1` configures all targets;
the generated mod-local reference file is ignored by Git. `build/Build-Mod.ps1` evaluates
the mod project's Compile items, copies those exact 67 sources byte-for-byte, verifies
hashes, builds the linked-source mod, and emits `EasyCommands.zip`, `bridge.cs.txt` and a
manifest. There are no source substitutions. CI builds both targets and uploads both artifacts.
The bridge protocol is `EasyCommands.Bridge.v1`.

## Completed verification

- Both target builds passed without warnings/errors.
- Full standalone unit suite passed: VSTest reported 2,063 passed, zero failed, including
  three new constructor/Main context-restoration regression cases.
- The exact standalone package passed the C# 6/game API analyzer: 96,119 characters,
  3,881 characters of headroom, SHA256
  `C95833FCBE38BD1CD23F4B2BF6E073EB2736D15B80038F4E61625CC5FD92355D`.
- The new source package passed the game's actual Mod-target compiler: 67 files, no
  diagnostics, approximately 14.66 seconds for the compatibility run.
- Four compact in-game scenarios passed: gyro/projector axes, lazy/list expressions,
  recipe queueing and actual meal production.
- Ten compatibility/lifecycle checks passed: instance isolation and nested requests,
  Custom Data edits, bridge recompile, waits/async, disable/resume, cross-grid vanilla IGC,
  ten-bridge batch, block removal, rejected disabled target and re-enabled target.
- All fifty distinct mod-backed PB workloads passed final checks for checksums, state
  isolation, exact messages/requests, async work and incremental reloads.
- The refactored standalone PB passed the real-game light-toggle control in 16 ticks.
- `git diff --check` passed. Disposable server processes were stopped after each run.

The focused compatibility mode uses `-SkipFullBatch` to omit only the ten-full-PB
compilation timing batch, whose earlier evidence is retained. It does not skip functional
compatibility checks. The first new rejected-target fixture incorrectly required brief
readiness output from two PBs on the same tick; the harness was corrected to latch both
markers independently. The failed attempt is retained; the corrected run passed.

| Fifty-PB phase | Mean combined script time/update | 95th percentile | Maximum | Observed updates/s |
| --- | ---: | ---: | ---: | ---: |
| Steady | 2.329 ms | 3.986 ms | 7.797 ms | 60.00 |
| Reload ten PBs while others execute | 2.228 ms | 4.648 ms | 10.518 ms | 60.17 |
| Steady after reload | 2.269 ms | 4.263 ms | 6.866 ms | 60.00 |

Timing methodology and instrumentation limits are described in `mod-stress-results.md`.
Cold first-use costs remain: the fifty-PB messaging/call warm-up had a roughly 404 ms sample.
Do not interpret the steady values as stall-free startup or exact server CPU usage.

## Evidence in this worktree

- Unit results: `.build/test-results/shared-host.trx`.
- Final package: `.build/mod-poc/staged/52db65875d124828b6fb299d7c7bce4c/`.
- Compact scenarios: `.build/mod-poc/runs/ba011724d172453baabc8cdbe76410eb/`.
- Corrected compatibility: `.build/mod-poc/runs/8e09e9cdb3a349cbbed11cd83fe9dc77/`.
- Retained readiness-fixture failure: `.build/mod-poc/runs/cf6f40c30fec4596a8ab25c957deec6a/`.
- Fifty PBs: `.build/mod-poc/stress/59988368b5e44b059cec0f69be5e0e8a/`.
- Standalone control: `.build/mod-poc/standalone-controls/c75609a9aba84e6bbb5b556a1f29e688/`.

Earlier broad-suite and timing reports refer to the earlier PoC, not this revision's
complete validation. All retained game compiles use the explicitly identified test bootstrap:
`PackageLoadingVerified: false`. Normal mod discovery/loading, client lifecycle/access
validation and explicit mod execution budgets remain gates before public release. No
Workshop publication or player-world installation was performed by this implementation.
