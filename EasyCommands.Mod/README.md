# EasyCommands source mod and shared engine

This target links the interpreter from `../EasyCommands`. The standalone PB keeps its
`MyGridProgram` host in `EasyCommands/Hosting/PbProgram.cs`; this target selects its own
constructor, host facade and PB Run adapter. There is no duplicated interpreter and no
source rewriting. Parser budgets and PB-driven scheduling are unchanged.

Build with `build/Build-Mod.ps1 -ServerRoot <server-root> -PythonPath <python-executable>`.
Reference-only builds can pass `-GameBin <assembly-directory>` instead. Output under
`.build/mod-poc/staged/<id>/` contains a `Mods/EasyCommands` source package, `bridge.cs.txt`,
and a byte-hash manifest. The game loads the source package, not the ordinary build DLL.
Paste `EasyCommands.Bridge/script.cs.txt` into a powered PB and put EC code in Custom Data.
The versioned terminal-property protocol is `EasyCommands.Bridge.v3`.

Use `build/Test-ModPoc.ps1` for actual Mod-target sandbox compilation and execution in an
isolated dedicated server. Its bootstrap bypasses normal package discovery only; it does
not bypass the Mod whitelist. Use `-CompatibilityOnly`, `-Suite`, or `-ScenarioName` for
specific checks. `build/Test-ModStress.ps1` runs concurrent PB workloads.

This is an implementation for validation, not a published Workshop release. Normal package
loading, multiplayer/client lifecycle, access validation and explicit execution budgets
remain release gates. Runtime state does not persist across restarts. Test results and
the design are under `build/mod-*.md`; earlier PoC timings describe the earlier source
adaptation and API, not a guarantee about this revision.

Current validation: [shared-host implementation results](../build/mod-implementation-results.md).
For a focused lifecycle run, use `-CompatibilityOnly -SkipFullBatch`; the latter omits
only the expensive full-PB compilation batch. Standard unit and build checks run in CI;
actual Mod-target execution is explicitly verified in the disposable-server harness.

Power interruption/recovery and actual warhead-destruction cleanup are now verified:
see [lifecycle results](../build/mod-lifecycle-results.md). Use `-LifecycleOnly` for the focused battery/warhead test.
The complete compatibility run includes these checks after its earlier scenarios.

Current bridge: v3. Set `public UpdateFrequency updateFrequency` at its top (Update1,
Update10, Update100 or None). The requested value is forwarded to the engine. Missing mod
stops scheduling instead of polling; Run again after enabling the matching mod. Replace
both mod and bridge when upgrading from v1 or v2. Cadence-only checks use `-SchedulingOnly`.

Bridge contract tests compile the exact shipped bridge body into a generated test host.
All 26 bridge/output cases and the full 2,089-test suite passed; see
[bridge unit results](../build/mod-bridge-unit-results.md) for coverage and the focused command.

Set `public LogLevel logLevel = LogLevel.SCRIPT_ONLY;` in the bridge to suppress
engine status while keeping script prints and errors; INFO is the standalone default.
The engine returns individual Echo arguments, which the bridge replays through the PB's
native Echo. The mod adds no output truncation, newline or empty placeholder. Output
therefore appears in the PB terminal as before, subject to the game's normal limits.
See [logging/output results](../build/mod-output-results.md).
