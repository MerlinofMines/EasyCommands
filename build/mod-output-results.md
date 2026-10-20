# Logging and PB output parity

Verified 2026-10-20 with bridge protocol `EasyCommands.Bridge.v3`.
Replace both mod and bridge when upgrading from earlier versions.

The bridge exposes `public LogLevel logLevel = LogLevel.INFO;`, matching the standalone
script's default. SCRIPT_ONLY suppresses engine status but retains script prints and
errors. Each invocation forwards the setting to its own engine without resetting it.

The mod previously joined Echo messages into one string capped at 4,096 characters.
It now returns an array of the original Echo arguments. The PB bridge invokes native
Echo once per argument, preserving boundaries, embedded newlines and blank calls.
It adds no truncation, newline or placeholder when there is no output. The game's own
PB output limits still apply. Borrowed services and buffered messages are released at
the end of the invocation; the returned snapshot remains valid for the bridge.

## Validation

- All 2,089 unit cases passed, including 25 bridge cases compiled from the shipped
  source and one mod-output transport case. Null/empty/multiline/12,000-character Echo
  arguments and logging changes on an already attached bridge are covered.
- The game's real Mod-target compiler accepted all 67 sources without diagnostics.
- Six live checks passed: four scheduling cases and two logging/output comparisons.
  Two mod bridges and two standalone PBs were driven manually at INFO and SCRIPT_ONLY.
  All 64 comparisons of native PB DetailedInfo matched exactly: 30 invocations per
  mode through incremental parsing and completion, plus long prints and runtime errors.
- The initial error fixture used an unknown bare word, which EasyCommands validly
  treats as literal text. That fixture failed its error assertion, while its output
  comparisons passed. The corrected invalid-break fixture passed on the retained run.

Evidence: `.build/mod-poc/runs/1c6a06f482b443eb889975195f00a825/`, especially
`results.json`, `output-parity.json`, `scheduling.json` and `mod-compile.json`.
Initial fixture failure: `.build/mod-poc/runs/6884902f689e4cd4991636920a075313/`.
Unit results: `.build/test-results/bridge-v3-unit.trx` and `bridge-v3-full.trx`.
Matching package: `.build/mod-poc/staged/b6eacd5b1de14104991f0abb48dcc88f/`.
The local tryout bundle was refreshed with that package and bridge; v2 backups remain.

Run with `build/Test-ModPoc.ps1 -SchedulingOnly -SkipFullBatch` (and the usual server
and staged-mod paths). This uses the documented test compiler bootstrap. Normal
package discovery and multiplayer release gates remain unchanged.
