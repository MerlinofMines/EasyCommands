# Initial AI support measurement

## Current measurement with additional controls (2026-10-19)

| Release package | Characters | Remaining |
| --- | ---: | ---: |
| Rebased initial AI support | 95,514 | 4,486 |
| With target/destination, altitude/alignment and repeat controls | **96,043** | **3,957** |
| Increment for additional controls | **529** | |

The total AI increment over latest master is 1,554 characters. Release build,
all **2,060 unit tests**, and the exact packed-script C# 6/API check passed.
Current package SHA-256:
`1904883B9550C6BD4EA693D236514AD1C401BEBAF3CCC4539D2CB369403823AC`.

Added read-only target-presence flags on both combat blocks; Defensive retreat
position override and clear; Flight minimum altitude and gravity alignment;
Recorder repeat-path toggle. Only repeat needs a new property type. `repeat path`
and `loop path` preserve the existing standalone repeat/loop control commands.

Live testing caught a distinction between the typed `MinimalAltitude` getter and
the terminal slider: the typed getter adjusts for grid size. EC deliberately uses
the `MinHeightAboveTerrain` terminal property for configured-value readback.

Live verification of this exact package:

- `ai-extended-settings`: pass, 40 ticks. Altitude configured/read as 150 m while
  the typed API reports 153.75 m on this rig; alignment and repeat toggle/readback,
  destination override/clear and absent target flags passed. Artifacts:
  `.build/integration/6567239844df40158135d496e9ba0316/`.
- `ai-detection-and-retarget`: pass, 5,064 ticks. Offensive and Defensive naturally
  acquired the separately owned target. EC replaced a deliberately wrong retreat
  point, requested flee and travelled 144.65 m, stopping 7.19 m from the intended
  point at 0 m/s with six healthy thrust directions. Artifacts:
  `.build/integration/6c0482c4ead24d6fbe6b2f40fab884ea/`.

Additional physical verification (2026-10-19), with the same packed script:

- `ai-repeat-route-lifecycle`: pass, 13,817 ticks. Eight outbound visits and eight
  returns over a sustained 60 m route; disabling repeat let playback finish and
  stop within 20 m of the return point at 0 m/s, with healthy thrust directions.
  Artifacts: `.build/integration/684e5b0e50694aa7b826e398bd8bb3d4/`.
- `ai-planet-altitude-alignment`: pass, 6,314 ticks, on a generated 6 km Moon.
  EC enabled gravity alignment and set the minimum altitude to 200 m, then 100 m.
  Initial bank 36.12 degrees reduced to 0.51 degrees. Across 80 observations in
  the protected 20–60 second window, minimum clearance was 206.21 m. After EC
  lowered the floor, the drone reached the lower waypoint with 10.14 m error,
  112.64 m terrain clearance and 0 m/s final speed. All thrust directions remained
  healthy. Artifacts: `.build/integration/f032174332d64652b1b7ee7f3d128568/`.

- `ai-obstacle-avoidance`: pass, 6,614 ticks. With collision avoidance enabled,
  the drone stopped before a solid armor obstacle, with playback off, no AABB
  overlap and 37.68 m minimum PB-to-obstacle clearance. The fixture then removed
  the obstacle and EC restarted playback: 304.19 m maximum travel, 10.00 m final
  waypoint error, 0 m/s final speed and healthy thrust directions. The AI stopped
  instead of finding a detour; the original detour/arrival expectation failed and
  was replaced with explicit safe-stop/recovery assertions, not treated as proof
  of pathfinding. Artifacts: `.build/integration/e4f4eac3021740899f2766a25d33bd35/`.

- `ai-target-loss`: pass, 3,383 ticks. Offensive and Defensive both acquired
  the powered grid; after the harness removed it, EC read false on both blocks
  and the independent game-component checks confirmed no remaining target IDs.
  Artifacts: `.build/integration/5898fa2dafdb4562aced96ac415a0171/`.

The final harness also passed all four stationary AI scenarios: behavior/power
separation (58 ticks), flight settings (83), playback/flee prerequisites (24), and
extended property settings (40). Artifacts:
`.build/integration/fd33463dae99453aae70e886afee821d/`.

The final-harness motion regression run passed all four established scenarios:

| Scenario | Maximum travel | Final destination error | Final speed |
| --- | ---: | ---: | ---: |
| Recorder route arrival | 152.26 m | 9.97 m | 0 m/s |
| Stop, wait, resume, arrive | 152.58 m | 10.00 m | 0 m/s |
| Detected-threat retreat | 144.91 m | 7.08 m | 0 m/s |
| Detect and replace retreat destination | 144.74 m | 7.15 m | 0 m/s |

All four retained healthy thrust in six directions; stop/resume independently
observed stopping during the pause, and both retreats observed natural target
acquisition and positive flee state after the EC request. Artifacts:
`.build/integration/892faddac7274b55a3faa1172ce29f15/`.
Across the four stationary, six motion, planetary and obstacle scenarios, all
**12 AI E2E scenarios** have now passed with the exact current packed script.
These verification-only changes leave its hash and 96,043-character count unchanged.

These are independent physical assertions beyond the script's pass message.
Minimum altitude is a safety floor, not a requested hover height. Gravity alignment
respects terminal maximum pitch/roll limits; the fixture sets both to zero and
starts banked. Its valid GPS-style waypoint poses have forced rotation disabled,
so recorded orientation cannot substitute for EC's gravity-alignment control.
The fixture uses real gyros, reactors and sufficient vertical ion thrust; it does
not force motion or orientation during execution. Earlier fixture attempts exposed
zero-axis waypoint matrices and insufficient lift, both corrected before the
passing runs.

## Rebase checkpoint (2026-10-19)

Rebased onto `origin/master` at `41a9eca`, retaining visibility redaction,
compact registrations/lazy expressions, and connector/shared-production updates.

| Current Release package | Characters | Remaining |
| --- | ---: | ---: |
| Latest master (same tree as measured `f09b478`) | 94,489 | 5,511 |
| Rebased AI support | **95,514** | **4,486** |
| AI increment | **1,025** | |

This is 3,703 characters smaller than the original AI package below. Release build,
all **2,053 unit tests**, and standalone packed-script C# 6/API checks passed with
zero warnings/errors. Current package SHA-256:
`DE0CB2F17C278BC98EA731BF9B49614247BE241C492DB1D52A24359503B48B88`.

Rebase resolutions preserve both AI and upstream integration scenarios and
telemetry. Mature/dead farm fixtures moved to x=110/116 on an extended deck so
they do not overlap the upstream gyro/projector fixtures.

The rebased exact package passed live detected-threat retreat: 144.90 m travel,
6.92 m final destination error, zero final speed, observed enemy/flee state and
six healthy thrust directions. Artifacts:
`.build/integration/a1455b6ebf9546f58051fdacfca9f7da/`.
The relocated mature/dead farm fixture check also passed (14 simulation ticks):
`.build/integration/a715305dfa5346ca9969a84233f74c29/`.

## Original measurement (2026-10-17)

Measured 2026-10-17 with the existing MDK2 full-minification Release configuration
and the configured local dedicated-server assemblies. Counts use
`File.ReadAllText(scriptPath).Length`.

| Package | Characters | Remaining |
| --- | ---: | ---: |
| Baseline at `478c291` | 98,176 | 1,824 |
| Initial AI support | 99,217 | 783 |
| Increase | **1,041** | |

Included:

- All five AI selectors, with spaced and compact names.
- Shared AI behavior read/write using the game's `ActivateBehavior` terminal
  property, independently of inherited block power.
- Flight speed limit, precision and collision avoidance.
- Recorder playback start/stop and playback state through its component.
- Defensive flee command and fleeing state. False does not cancel fleeing;
  the PB interface supplies no stop-flee method.

The shared behavior property's ID was checked against the installed game's
`MyAiBlockComponent.CreateTerminalControls`, rather than inferred from its caption.
The combat search API exposes `FoundEnemyId` as nullable Int64. A boolean target
presence property is a possible subsequent addition; numeric ID exposure and
richer target telemetry were not included in this measurement.

Release source build/package passed with no warnings/errors. The full unit suite
passed: 1,833 tests, including four new AI tests covering all five selectors,
power versus behavior, flight settings, playback, and flee state/commands.
The exact packed script also passed the C# 6 compiler and MDK game API analyzer
with no warnings/errors. `git diff --check` passed.
Live bindings were subsequently checked with the dedicated-server E2E harness
as described below. Actual navigation and docking were not exercised.

Package SHA-256:
`A8C1A92F846052FAFF4D5DCEA5C8DA3D9627A24836F0C23C3D7F3EEF03FB3CC9`.

See [AI usage](../docs/EasyCommands/blockHandlers/ai.md). Earlier support-review
budget figures describe the baseline before this implementation.

## Dedicated-server E2E verification

The exact package hash above was deployed through `build/Test-InGame.ps1` to a
fresh disposable world with five real, powered AI blocks. Three checks passed:

| Scenario | Result | Simulation ticks |
| --- | --- | ---: |
| All five behavior switches, independent of power | Pass | 58 |
| Flight speed, precision and avoidance, including reverse toggles | Pass | 83 |
| Recorder playback start/stop with a seeded waypoint; flee preconditions | Pass | 24 |

Behavior and playback/flee results are under
`.build/integration/79e51c2476bf4a7188d11ee378f35457/`.
The corrected Flight scenario's results are under
`.build/integration/d8ed91ddd7314f27af08247b3bb78d09/`.
Each retains `metadata.json`, `results.json`, raw API readings in
`measurements.jsonl`, the fixture and server logs.

The first sandboxed launch failed during network initialization. The successful
runs used the isolated loopback server outside the sandbox. The first Flight
scenario had a compound-assertion parsing error; it was corrected, all three
scenario suites passed the syntax tests, and Flight was rerun successfully.

Important limitation: PB `Flee()` requests normal defensive fleeing, which
requires a detected threat, an active movement AI and valid configuration. The
live negative test verifies it remains inactive without those prerequisites;
positive enemy-triggered fleeing has not been verified. This stationary fixture
also does not certify flight, route arrival, docking or combat maneuvers.

Repeat with an existing full server installation:

```powershell
./build/Test-InGame.ps1 -Suite ai -ServerRoot '<full dedicated-server root>'
```

These harness/scenario/documentation additions do not change the packed EC script:
it remains 99,217 characters, with 783 remaining.

## Physical motion verification

The `ai-motion` suite adds a dynamic drone and a separately owned powered target.
All motion is performed by the game's AI in response to EC commands; the fixture
does not teleport the drone, set its velocity, force a target, or force flee state
while a scenario is running. Setup clears residual velocity between scenarios.

The initial run at `.build/integration/3b01e3f005084cc6bd6bae6898bf76e4/`
passed route arrival, route stop/resume and detected-threat retreat. The retreat
recorded positive flee state after the EC request, travelled 144.70 m and stopped
7.23 m from the retreat point. The stop trace shows 0.11 m/s with playback off at
tick 630, during the requested pause.

The final harness strengthens stopping to require that state specifically during
the pause window, checks all six healthy thrust directions, and disables exhaust
damage on the compact test rig. The game reports a missing-thrust-direction
warning despite all six directions being functional and reporting 345,600 N each;
the warning is preserved in `motion.jsonl`. Dedicated-server status reporting is
the suspected cause; no workaround changes the actual EC controls.

See [physical test definitions](../integration/AI.md) for the fixture and exact
acceptance criteria. These tests do not cover obstacle avoidance, moving reference
beacons, connector docking or weapons engagements.

Final run: `.build/integration/78747c1b17e84e27bbb2b41343fec91b/`, Steam server build
24675709, exact EC package SHA-256 unchanged from above. All three scenarios passed
with the strengthened checks:

| Scenario | Travel from start | Final distance to destination | Final speed | Six healthy thrust directions |
| --- | ---: | ---: | ---: | --- |
| Route arrival | 150.26 m | 10.00 m | 0 m/s | Yes |
| Stop/resume and arrival | 150.31 m | 9.95 m | 0 m/s | Yes |
| Detected-threat retreat | 144.89 m | 6.83 m | 0 m/s | Yes |

Stop/resume observed playback off and 0.067 m/s at tick 630, inside the enforced
pause window. Retreat independently observed a detected target and positive flee
state after the EC request; automatic flee was disabled. These results supersede
the earlier lack of physical-flight and positive-flee verification. The remaining
untested cases above still apply. All four integration scenario syntax tests and
`git diff --check` passed. Only harness, scenario and documentation code changed
during this physical verification; packed EC character usage remains unchanged.

Before PR publication, the final full unit suite passed all **1,835 tests**,
including the four integration scenario syntax suites.
