# Broader mod-backed compatibility results

Measured 2026-10-19 against dedicated-server Steam build `24675709`, game `01_210_014`.
Twenty-six distinct existing non-motion integration scenarios now have passing results
through the bridge and the real Mod-target-compiled EasyCommands engine. No additional
engine or block-handler adaptations were needed. Production interpreter source remains unchanged.

| Suite | Coverage | Outcome |
| --- | --- | --- |
| `apex` | Farming/food/irrigation selectors, inventory seed transfers, recipe queueing, water units, oxygen output, crop state/yield, algae countdown, actual food production, lighting/color/intensity | 15/16 initially; corrected new-crop assertion passed in focused rerun |
| `compact-refactors` | Signed gyro/projector axes, lazy expressions, lists/indexing/aggregations, blueprint selection, actual meal production | 4/4 passed |
| `connector-production` | Connector target discovery, connect/disconnect/reconnect, actual refinery production and stopping | 2/2 passed |
| `ai` | Behavior enablement, flight settings, playback/flee preconditions, extended settings | 4/4 passed |

The focused rerun also repeated `crop-mature-and-dead`, which passed without assertion changes.
Thus all 26 distinct scenarios have passing evidence with the corrected assertion; this was
not a fresh single run of the entire Apex suite after the correction. These are existing
expected-outcome tests, not a complete full-script/mod differential run of every scenario.

## Corrected test assumption

The initial `crop-input-output` test required a newly planted crop to report a strictly positive
output amount. Before the interpreter executed, the harness's direct native component reading
already reported `HarvestAmount: 0`, crop health 100 percent, growth progress zero, and seed
requirement one. The interpreter correctly read that zero.

Inspection of the installed game's `MyFarmPlotLogic` explains the timing sensitivity:
`OutputItemAmount` uses the stored yield for a crop at exactly 100 percent health; that yield
is populated on maturation. A crop below full health instead uses its seed definition's
minimum output. A new healthy plant can therefore report zero, while a later update changes
the value. A fresh original full-script control passed with output two, rather than reproducing
the complete preceding scenario history. That control alone does not establish host parity;
the direct native reading and getter implementation establish that zero was legitimate.

The new-crop assertion now accepts a nonnegative output while retaining occupied/alive state
and the exact seed requirement. The separate controlled mature/dead fixtures still require
their known yield of two. This changes only a test assertion and adds a harness comment;
it does not change the language, handler, game behavior, or mod source. The corrected case
and controlled mature/dead case both passed through the mod-backed engine.

## Evidence

Directories below are under `.build/mod-poc/runs/`:

- Apex initial: `fb48de1eaf3c469a873d01dc24820fd6` (retain the initial failure).
- Compact regressions: `66badcb75a014db19c1ec3ff12b9ed9c`.
- Connector/production: `8d62357c507c43a1b1a57ee3d9e8b2d4`.
- AI controls: `30d82c0e78514575a6895e945cb35754`.
- Corrected crop + unchanged mature/dead rerun: `3bcabd12e25b40f1bfe67b06d5a18d8e`.

The original full-script crop control is under
`.build/mod-poc/broad-controls/11b3ccc500fc4159a5287225c17c4041`.
Each run retains exact request/scenario inputs, results, native fixture measurements and
server logs. Mod-backed runs retain the source manifest/diff, copied mod source and real
Mod-target compiler diagnostics. The source-mod bootstrap limitation is unchanged:
`PackageLoadingVerified: false`.

## Scope and next gates

These checks complement the [language/concurrency tests](mod-stress-results.md); they do not
cover the entire handler catalog. Physical AI flight/arrival, planetary navigation, obstacle
avoidance and long soaks were not run in this pass. Timer/sensor/toolbar activation, rejected
PB calls, power/lifecycle/save-reload behavior, ownership/access control, normal mod-package
loading and multiplayer joining/reconnection remain explicit gates. The proposed permanent
build/host separation is in [mod-shared-core-plan.md](mod-shared-core-plan.md).

Run individual suites with `build/Test-ModPoc.ps1 -Suite <name> -StagedMod <path>`.
Use `-ScenarioName` to select cases. The runner preserves the existing scenario inputs and
uses sequential disposable servers; it does not install into player saves or publish a mod.
