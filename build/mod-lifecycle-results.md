# Power and destructive PB lifecycle verification

Measured 2026-10-20 using the same packaged mod and bridge supplied for local single-player
tryout. No interpreter, mod-host or bridge changes were required. Only the disposable-server
test harness, runner switches and documentation changed.

Both runs passed: five focused lifecycle checks, then all fifteen combined compatibility/
lifecycle checks. The fixture uses a battery-powered victim PB and an armed large warhead
five metres away. A powered survivor PB on another grid 100 metres away runs independently.
The harness switches the battery, not the victim PB's Enabled flag, and detonates the actual
game warhead. It does not Close/remove/damage the original victim programmatically.

## Observed behavior

| Event | Independent assertions | Result |
| --- | --- | --- |
| Battery switched off | Victim stayed enabled and functional, became unpowered; invocation count and output froze for 120 ticks; survivor continued | Pass |
| Battery switched on | Original runtime object and seed/counter resumed, without recompile or manual Run | Pass |
| Armed warhead detonated | Original PB closed; its runtime and close-subscription entries disappeared | Pass |
| 120 ticks after destruction | Old entries stayed absent; survivor continued executing | Pass |
| New PB with the same name | Different EntityId, fresh initialization from counter zero with a new seed; old state stayed absent | Pass |
| Replacement explicitly removed | Its runtime and close-subscription entries also disappeared | Pass |

In the focused run, the victim invocation count stayed exactly **68** across the complete
power-off observation window, while the survivor counter advanced from **61 to 181**.
After restoration the victim resumed the same runtime and reached 99 invocations before
detonation. The destroyed PB and its removed map entries were observed **three simulation
updates after detonation**. Runtime and subscription map counts both dropped from two to one,
stayed there while the survivor counter advanced from 215 to 335, returned to two for the
replacement, and returned to one after the replacement was removed.

The test bootstrap retains its actual loaded ModSession instance. Test-only reflection reads
the mod's existing `states` and `subscriptions` maps and invocation counter; no production
diagnostic endpoint, reflection or cleanup change was added to the shipped mod. These are
direct checks of released registry references, not a heap/garbage-collection profiling claim.
Cached flags such as IsWorking can retain old values on already-closed game objects; the
destruction assertion uses the entity's Closed state and absence from both maps.

## Reproduce

```powershell
./build/Test-ModPoc.ps1 -StagedMod '<staged>/Mods/EasyCommands' -LifecycleOnly
# Includes earlier language/IGC/invocation checks before the same lifecycle sequence:
./build/Test-ModPoc.ps1 -StagedMod '<staged>/Mods/EasyCommands' -CompatibilityOnly -SkipFullBatch
```

Evidence in this worktree:

- Focused run: `.build/mod-poc/runs/1a09d85bd30341128bbc68fdeddcbf16/`.
- Combined run: `.build/mod-poc/runs/700e25c98c104e0dadba9be9a066575c/`.
- Each contains `results.json`, `lifecycle.jsonl`, compiler diagnostics, source manifest,
  exact script/request inputs and server logs.
- Package remains `.build/mod-poc/staged/52db65875d124828b6fb299d7c7bce4c/`.

These are real game-power and explosion tests on the disposable dedicated server. Normal
single-player mod-package discovery, multiplayer access/lifecycle, save/reload persistence,
and explicit mod execution budgets are separate outstanding release gates. No player world
or locally installed mod was modified, and no Workshop publication was performed.
