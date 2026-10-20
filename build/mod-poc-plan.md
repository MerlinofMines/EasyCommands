# EasyCommands mod proof of concept: implementation handoff

Prepared 2026-10-19 for execution with GPT Sol. Start with [the feasibility assessment](mod-feasibility.md). This is an implementation plan, not a completed mod.

Implementation status: the runtime proof of concept and timing measurements are now available in [mod-poc-results.md](mod-poc-results.md). The installed dedicated server rejects unpublished local packages, so automated execution uses a clearly identified test bootstrap invoking the game's real Mod-target compiler; normal package discovery/lifecycle validation is still pending. See [the runnable instructions](../integration/ModPoc/README.md).

Confirmed requirement (2026-10-19): preserve incremental program parsing across ticks. Parsing latency is acceptable; protecting execution of other PBs takes priority. Keep each PB's parsing work bounded and independently scheduled. Do not replace this with an eager whole-program parse on load or Custom Data edits.

The proposed permanent dual-target source layout and migration steps are in [mod-shared-core-plan.md](mod-shared-core-plan.md). The PoC's generated source substitutions are temporary; the permanent design uses shared engine sources and explicit host adapters.

## Outcome

Produce a disposable, locally runnable **source mod plus a tiny PB bridge**, demonstrate that the existing EasyCommands interpreter can run in that mod, and measure how much faster the bridge installs into a PB. The demonstration must use the real parser and interpreter; a delegate that returns a string is only the first checkpoint.

Keep the standalone EasyCommands build and published behavior intact. A production core extraction, new block models, custom editor, native mod scheduler, persistence of execution state, and Workshop publication are outside this proof of concept.

## What stays in the PB

[easycommands-mod-bridge.cs.txt](prototypes/easycommands-mod-bridge.cs.txt) is a complete proposed PB editor body: **1,747 characters / 51 newline-delimited lines** including comments and error handling as initially written. The current standalone package measured 96,043 characters. Do not interpret that ratio as a measured speedup.

The bridge does six things:

1. Starts with `Update1`, then discovers the versioned `EasyCommands.Poc.v1` property.
2. Retrieves and caches a mod delegate bound to its PB.
3. Signals a fresh registration on the first successful call after PB construction.
4. Forwards the Run argument, update source, terminal system, IGC context, and elapsed milliseconds.
5. Applies the returned update frequency and displays the returned output.
6. Retries slowly if the mod is absent, or stops with a useful error if a call fails. Arguments received while disconnected are explicitly rejected rather than silently replayed.

It does not tokenize EasyCommands, interpret commands, scan blocks, maintain language variables, or copy Custom Data into a second storage location. The mod's block-bound endpoint reads the PB's Custom Data directly. The first-call registration identifies the PB as an EasyCommands instance; block renaming is unnecessary.

The draft calls a synchronous delegate:

```csharp
Func<bool, string, UpdateType, IMyGridTerminalSystem,
     IMyIntergridCommunicationSystem, double,
     MyTuple<UpdateFrequency, string>>
```

The first argument means reset/register this bridge generation. The remaining arguments describe this invocation. The result contains the next update frequency and bounded diagnostic output. All boundary types come from the framework/game, not separately compiled custom classes.

Keep host references valid only during the call. Do not schedule work that later uses the PB's terminal wrapper or IGC context. Clear those references on exit. Cache the block identity and interpreter state, not a live invocation context.

The bridge has passed ordinary C# 6/.NET Framework 4.8 compilation and execution through the game's PB whitelist, connected to the separately Mod-target-compiled interpreter. Normal package discovery/lifecycle validation remains pending. The mod API name and signature are experimental and may change during implementation; update the draft and measurements together.

## Stage A: make the bridge real

Suggested files:

```text
integration/ModPoc/
  ModPoc.csproj
  ModSession.cs
  ModHostContext.cs
  README.md
build/Build-ModPoc.ps1
build/Test-ModPoc.ps1
```

Use a normal mod session component to register the hidden terminal property. Keep the property object for cleanup and remove it on unload. Avoid duplicate registration. Register only once terminal controls are ready; the bridge already tolerates delayed availability. Execute the endpoint only on the server, and reject invalid/closed/unpowered or disabled blocks.

The getter binds a delegate to the actual PB supplied by the terminal-property API. Maintain runtime state per `EntityId`, reset it on the registration flag, and unsubscribe/remove state when the block closes. Do not trust an arbitrary entity ID submitted as a request. Only the getter's bound block may supply source and receive the result. A general public-mod authorization design is outside the quick prototype; confine the test to a private fixture and do not expose arbitrary world-entity access.

For this checkpoint, return an identity, independent invocation counter, received argument, and source length. Demonstrate two blocks have separate counters. Exercise real IGC synchronously through the passed interface. A normal PB should send to the mod-backed PB and receive from it; test on two powered grids with antennas, not just two PBs on one grid. Retain normal antenna/reachability semantics. Report any runtime restrictions instead of replacing IGC with unrestricted multiplayer messages.

**Exit:** a real source mod loads under the game's mod whitelist; the PB bridge compiles under its whitelist; two blocks call independent endpoints; IGC crosses the boundary; missing-mod behavior is understandable. A DLL running as a privileged plugin is not an acceptable substitute for the source mod.

## Stage B: run the existing interpreter with a small hosting adaptation

Aim for an isolated experiment rather than a broad production refactor. Prefer linked/generated staging from current source so the PoC executes the actual implementation and does not acquire a maintained fork.

One concrete approach is for `Build-ModPoc.ps1` to stage the current production `.cs` files into `.build/mod-poc/<build-id>/Mods/EasyCommandsPoc/Data/Scripts/EasyCommandsPoc/`, excluding generated/build/debug-only files. Adapt only the following hosting seams in the staged copy:

- Replace the engine's inheritance from `MyGridProgram` with a small mod host base. Supply the host before initialization using an explicit constructor argument.
- Provide the members current engine code actually uses: `Me`, `GridTerminalSystem`, `IGC`, `Echo`, and a runtime facade containing `TimeSinceLastRun` and `UpdateFrequency`. Do not fabricate PB instruction counters or use the reflection-based test factory inside the mod.
- Save and restore the previous static `PROGRAM` around the constructor and `Main` using `try/finally`. This is a temporary single-threaded hosting adaptation. Prevent background entry and test nested calls. Explicit per-instance context remains the preferred production extraction.
- Buffer `Echo` output with a size cap in the host. On each call, provide fresh invocation services and elapsed time, run the engine, return the requested frequency/output, and clear host references in `finally`.
- Adapt `ProgramBlockHandler` to supported mod `Run` calls with clear acceptance semantics. Never leave the current `TryRun` usage executing from mod code. Test nested A-to-B calls, a guarded A-to-B-to-A cycle, and restoration of A's current execution context.
- Keep each PB's command graph, variables, queues, caches, and random generator independent. Do not introduce shared parse caching in this prototype.

If generating adaptations, require exact expected match counts or use syntax-aware transformations. Fail on unexpected input; preserve original and staged hashes and a readable generated diff. Avoid broad string replacements that silently alter unrelated code. Alternatively, a very small source-host abstraction is acceptable if it keeps the standalone build behavior unchanged and is clearer than the generator.

Compile-check staged code against the installed game DLLs, but always load its `.cs` files through a real source mod for the live test. No arbitrary runtime code generation, private PB-instance reflection, or full-engine insertion into `ProgramData`.

Use the existing parser settings initially so timing comparisons distinguish relocation from tuning. Run only bounded, known fixture programs during this experiment; a production release needs explicit execution/recursion/allocation budgets beyond the current PB limits. Do not run adversarial infinite-loop tests until yielding/budget enforcement exists.

**Exit:** the bridge runs the existing `light-toggle` scenario, a wait/function/async scenario, IGC commands, and independent programs in two PBs. Custom Data changes reset/reparse as they do today. PB recompilation resets/rebinds the right instance. Disabling or removing a PB halts its work; re-enabling/reloading behaves consistently with the standalone version.

## Stage C: measure loading instead of guessing

Use the existing dedicated-server fixture mechanism as a starting point. The installed configuration currently resolves to dedicated server Steam build `24675709` / logged version `01_210_014`. Resolve the actual configured binary path when implementing; do not hardcode a developer-specific worktree path.

Create a separate runner or an opt-in harness mode. Keep the existing suites' default behavior unchanged. Use fresh disposable worlds and isolated local mod directories under the run's `-path`; do not install into the player's normal Mods folder or edit a real save. Start helper processes hidden and stop only the process owned by that run. Use a separate port if the existing integration port is occupied.

The existing integration harness is a plugin. It may create fixtures, time calls, and record results; it must not implement the engine being evaluated. The saved world must actually reference the staged local source mod. Capture loader diagnostics proving the source mod compiled and registered its property.

Measure these separately:

| Measurement | Method |
| --- | --- |
| PB install/recompile wall time | On the server game thread, time assigning `ProgramData` on an otherwise ready PB. The inspected build synchronously recompiles through this setter. Label the result **installation including initialization**, not pure Roslyn compile time. Validate compile success afterward. |
| First engine call | Time the first explicit `Run` after installation, and record whether engine construction occurred there. The bridge moves engine initialization out of the PB constructor, so install time alone is an incomplete comparison. |
| Source parse/readiness | Record elapsed wall time and simulation ticks from the first invocation to a verified program marker/block state. Use the same Custom Data and parsing settings for both implementations. |
| Steady execution | Measure repeated calls of a known bounded program after initialization, with comparable output settings. |
| Mod/world startup | Record cold process start through mod registration/world readiness. Separate this from PB installation. Compare otherwise matched worlds with and without the mod; report whole-startup deltas as noisy observations rather than precise isolated compiler costs. |

Benchmark three PB sources: a minimal empty baseline, the draft bridge connected to the real engine mod, and the existing full packed EasyCommands script. Record source hashes and character counts. Do not use `dotnet build` duration, the existing harness's fixed 120-tick settling period, or string-length ratios as game loading measurements.

Start with one cold sample per fresh process and at least five warm samples per variant, alternating variants to reduce order bias. Report cold values individually, warm median and range, plus first-run/readiness figures. Do not label a maximum over five samples as a meaningful p95. Add a 10-PB batch to demonstrate scaling; a 100-PB load test is optional after the small run is stable.

Persist `metadata.json`, raw `measurements.jsonl`, `results.json`, original/staged source manifests, and server logs under the unique `.build/mod-poc/` run directory. Include game build, repo revision, source hashes, fixture/parse settings, sample order, and warm/cold classification. Produce a concise tracked report with the bridge's final size, actual numbers, compatibility results, and remaining gaps.

**Exit:** we can answer both “how long does this PB take to install?” and “how long until its EasyCommands program actually starts?” with measurements from the same machine/game build, and account for the mod's startup cost separately.

## Verification and scope

For the PoC, prioritize real sandbox loading, the existing smoke scenario, timing, two-instance isolation, Run arguments, waits, IGC interoperability, registration reset, disabled/removed blocks, and missing-mod behavior. Include source-change/reset and nested-call checks because they target the identified migration risks. Run the existing unit suite if shared production code changes; a standalone generated experiment does not require rewriting the test suite.

Dedicated-server automation is the first automated target. Provide short manual instructions to try the same staged mod/bridge in a disposable single-player world. If interactive validation is not performed, state that explicitly. Keep the engine in a single source-mod script assembly, keep the bridge small, and preserve user source in Custom Data.

A successful local PoC does not imply production readiness. Final hardening includes explicit interpreter budgets, comprehensive ownership/access checks, complete handler parity, multiplayer/client lifecycle coverage, restart policy, and a proper shared core. Record those as follow-up work instead of expanding this task into the entire migration.

## Ready-to-run handoff

> Implement the EasyCommands mod proof of concept described in `build/mod-poc-plan.md`, using `build/mod-feasibility.md` for background and `build/prototypes/easycommands-mod-bridge.cs.txt` as the starting bridge. Execute stages A through C in order. First prove the PB-to-source-mod bridge in the actual game sandbox, then host the existing EasyCommands interpreter with minimal isolated adaptations, then measure PB installation, first invocation, parsing readiness, and mod/world startup separately. Use the existing disposable dedicated-server harness for fixtures/measurement; the engine must run as a genuine source mod, not inside the harness plugin. Preserve the standalone edition and existing working changes. Keep generated/staged artifacts under `.build/mod-poc`, retain logs and raw timings, and document any untested behavior. Do not publish, modify player saves, install into the player's normal Mods directory, or undertake a production-wide refactor. Finish with runnable local instructions, the final bridge size, a measured comparison against the full script, passed/failed scenarios, and explicit remaining limitations. If a live test fails, diagnose it and continue with supported public APIs; do not claim completion based on compilation alone.
