# Apex Survival E2E tests

These scenarios ship with the Apex handlers in PR #281 and run locally on Windows:

    ./build/Test-InGame.ps1 -Suite apex

Run an individual check with existing builds:

    ./build/Test-InGame.ps1 -Suite apex -Scenario seed-alias-transfer -SkipBuild

The scenarios cover Farm Plot controls, algae output, Food Processor controls,
exact seed transfers with fruit as a decoy, Irrigation System controls, empty crop
state, and the kelp-crisp recipe blueprint queue. The harness creates the required
powered fixture automatically; no player connection is needed.

Additional scenarios check quarter-full water (ratio 0.25, capacity 180, level 45),
read-only water writes, standard/half oxygen output fractions and disabled output,
newly planted crop seed requirements and potential yield, mature/dead saved crop states,
actual food production and pause/resume through the parent assembler handler, algae countdowns, and Farm Plot light settings (including blink percentages).
`Setup` optionally names a deterministic harness preparation: `water-quarter`,
`crop-planted`, `crop-states`, `food-ingredients`, or `algae-blocked`. These use ModAPI only inside the disposable
fixture plugin; the deployed script uses Ingame APIs exclusively. Preparation runs after
PB compilation so exposure does not kill a planted fixture while compilation is pending.
Mature/dead fixtures load serialized crop state; they do not simulate 20 minutes of growth.

`measurements.jsonl` records raw component readings and terminal detail text at
startup, fixture preparation, and scenario completion. Use this alongside
`metadata.json` and `results.json` when evaluating units. Oxygen `GetOutput()` is
the solar output factor (0–1); the game's displayed L/min is a separate conversion
using the block definition's maximum gas output. Do not equate those quantities.
Water capacity/level are liters: a live fruit plant reports 20 L/min and removes
0.55556 capacity units per 100 simulation ticks (1.6667 seconds), matching the
installed game's consumption-to-storage conversion. Algae output is items/minute and its raw
countdown is seconds; unavailable raw NaN/infinity estimates become -1 in EasyCommands.

The Ingame farm interface exposes state, harvest amount, and seed requirement,
but no numeric growth/health or plant/harvest mutation methods. The mod fixture's
ability to plant is not a capability of the programmable-block script.

GitHub Actions validates both smoke and Apex scenario syntax as part of unit tests.
It does not install or launch a dedicated server for E2E execution.
See README.md for custom saves, separate script artifacts, diagnostics, and timeouts.

The installed game's `OutputItemAmount` documentation claims zero before maturity,
but the live getter reports potential yield even for growing/dead crops. Tests assert
that behavior and keep `crop complete`, `crop alive`, and `ready` separate.

Production assertions allow progress zero while running: the value resets at each
job boundary. The game can retain `IsProducing = true` while disabled, so assembler `running` also requires `IsWorking`. The scenario requires the cooked-item count to increase after resuming and checks that disabling
the processor stops production before resuming through the assembler parent selector.

## Local validation record (2026-10-16)

Dedicated-server Steam build `24675709`; packed script 98,176 characters (1,824 remaining).
SHA-256: `DB0AF9090D806D1D931A1E58D2DFBE6BC7B66F34CC6B9C9E3380774E48F610C0`.

All 16 Apex scenarios have passing results on this artifact: 15 passed in run
`95af22a07c0745aea19fcc044e6507c7`; `food-production` passed in targeted run
`e3c7763362234f77a5d66dc9340bfa4e` after correcting its assertion to read the
output inventory (`inventories[1]`). The earlier full-run failure was an input-inventory
assertion, not failed meal production. The targeted rerun used the same script and harness.
Results and raw measurements are under `.build/integration/<run-id>/` locally.

The functional suite passes all 1,829 tests. Release packaging and the exact packed-script
C# 6/API-analyzer compilation both complete with zero warnings/errors.

The existing light-toggle smoke scenario also passed on the same artifact in run
`f81e4f4764954c678d32a8ea8f164ac8`.
