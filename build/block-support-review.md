# EasyCommands block support review

Reviewed 2026-10-17 at `478c291`. This is a planning review; no runtime code changed.

## Character budget

A fresh `dotnet build EasyCommands/EasyCommands.csproj -c Release -p:Platform=x64 --no-restore`
succeeded with zero warnings/errors, using the locally configured dedicated-server assemblies.

| Measurement | Characters |
| --- | ---: |
| Current packaged script (`File.ReadAllText(...).Length`) | 98,176 |
| Programmable-block ceiling | 100,000 |
| Available | **1,824** |

Artifact: `EasyCommands/bin/Release/MDK/EasyCommands/script.cs`.
SHA-256: `DB0AF9090D806D1D931A1E58D2DFBE6BC7B66F34CC6B9C9E3380774E48F610C0`.
The 7,817 remaining in `minification.md` is an older measurement and should not
be used for current planning. File bytes also differ from character count.

Recommendation: retain approximately 800 characters for fixes and spend at most
approximately 1,000 on the first addition. These are budget allocations, not
measured feature costs. Measure each candidate through the same Release packager;
identifier renaming and trimming make source length an unreliable cost estimate.

## What counts as missing support?

The registry contains 61 selectors, including virtual selectors such as grid,
thread, display, and inventory. That is not a count of distinct game blocks.

There are three useful levels of support:

1. **Native:** a typed selector and meaningful EasyCommands properties.
2. **Inherited:** a broader existing handler already selects the block.
3. **Generic:** named/grouped terminal selection, exposed terminal properties,
   actions, inventory access, and displays where the block provides them.

`TerminalBlockHandler` already lists properties/actions and falls back to raw
terminal properties. Its converters support Boolean, Single, StringBuilder,
Int64, and Color. Other terminal value types are a real limitation. Its Int64
conversion passes through float, so it is unsuitable for preserving large IDs.
Actions currently execute without action parameters.

New typed handlers can inherit `FunctionalBlockHandler<T>` to get enable/power
and the shared terminal properties immediately. This is routine wiring; the
planning work is the additional block-specific properties and operations.

## Prioritized gaps

Relative cost means expected scope, not a measured minified delta. All rows lack
a dedicated selector today; generic terminal access may already cover controls.

| Priority | Block family | Useful first support | Expected scope / constraints |
| --- | --- | --- | --- |
| 1 | AI Flight (Move) | Speed limit, minimum altitude, collision avoidance, precision, gravity alignment, autopilot state | Small to medium; several direct primitive properties. Strong drone utility without implementing a flight system. |
| 1 | Action Relay (`IMyTransponder`) | Channel, send signal, allowed sender relations | Small; a compact direct API. Channel is Int32, so check generic conversion coverage. A relay signal is a game action signal, not an EasyCommands data message. |
| 2 | Broadcast Controller | Send custom chat text or GPS, antenna use, audience | Medium; behavior is on `IMyChatBroadcastControllerComponent`. Reuse the existing component helper. Valuable if chat notifications are a priority. |
| 2 | Event Controller | Threshold, AND mode, comparison direction | Small; only these three settings are on its direct PB interface. Do not promise event-type selection, monitored-block editing, or toolbar editing from this interface. |
| 2 | AI Recorder (Task) | Play/stop, record/stop, recording/playing state, repeat | Medium; `IMyPathRecorderBlock` is a marker interface. Controls are on `IMyPathRecorderComponent`; reuse component lookup. Defer full route editing. |
| 2 | Artificial Mass / Space Ball | Typed selection and enable; virtual mass; optionally ball friction/bounce | Very small for selection/shared controls, larger for ball-specific settings. Useful for gravity drives and specialized mechanisms. |
| 3 | AI Basic (Task) | Mission selection and basic follow settings | Medium; direct block exposes mission ID, while follow behavior uses components. Raw numeric mission IDs are poor public syntax. |
| 3 | AI Defensive (Combat) | Flee command/state, trigger, retreat destination | Medium to large; vectors, beacon/GPS choices and enemy search expand scope. Useful after Flight/Recorder. |
| 3 | AI Offensive (Combat) | Target telemetry, priority, scan interval, selected attack pattern | Large for comprehensive support; search and attack-pattern components need deliberate mapping. Favor a minimal telemetry slice first. |
| 3 | Survival Kit | Production state/queue through shared production behavior | Medium; existing assembler handler targets `IMyAssembler`, not general `IMyProductionBlock`. Inventory is already accessible. Do not reuse assembler-only modes or progress properties blindly. |
| 4 | Safe Zone | Selector and exposed terminal controls | Small for convenience only. PB interface is a marker; zone enable methods found on the mod interface are not PB capabilities. |
| 4 | Store / Vending Machine | List, insert, cancel offers/orders | Large for meaningful support; item/price/quantity/ID records and result handling. Generic inventory remains useful. |
| 4 | Medical Room, Contract Block | Generic controls and any provided inventory/displays | Low incremental automation value until a concrete PB-accessible use case is identified. Contract management must not be inferred from the existence of the block. |
| 4 | Button/control panels, Target Dummy, Emotion Controller | Selector convenience and exposed actions/settings | Generally small but low new capability. Target Dummy and Emotion Controller PB interfaces are markers. No implied button press subscriptions or toolbar editing. |
| 4 | Exhaust blocks, neon/emissive blocks | Effects, power dependency, color where exposed | Cosmetic automation; inspect terminal controls before adding subtype-specific wrappers. No dedicated handler currently. |
| 4 | Upgrade modules, kitchen/planter/service-terminal variants | Inspect actual inherited interfaces/components first | Generic/inherited coverage varies. Definition names alone do not prove a new behavior or a missing handler is needed. |

These priorities favor general automation per character. For a drone-focused
release, Flight and Recorder should lead; for base alerts, Broadcast may lead.

## Existing coverage that should not be counted as new block work

- Farming is already present: farm plots, algae farms, oxygen farms, food
  processors and irrigation. Food processors reuse assembler behavior;
  irrigation reuses gas-generator behavior.
- Weapons should first be checked against `IMyUserControllableGun` and
  `IMyLargeTurretBase`. Additional weapon models do not automatically require
  new handlers. Test specific newer weapons before claiming full coverage.
- Door variants share `IMyDoor`; rotor variants and hinges share stator behavior;
  landing gear/magnetic plates share landing-gear behavior.
- Hydrogen engines and wind turbines are selected through power-producer
  handlers plus subtype filters. Solar panels/reactors already have selectors.
- Spotlights inherit lighting behavior; Searchlight has its own handler.
- Cockpits, remote controls and cryopods already have handlers. Model variants
  should be checked for interface compatibility rather than listed as gaps.
- Inventories are selected from every terminal block's `InventoryCount`, not
  only cargo containers. Displays similarly use surface-provider interfaces.
- Jukeboxes may inherit sound/display support; verify actual interfaces before
  treating them as a separate feature. Decorative models are not separate APIs.
- Armor, plain conveyors, unattached mechanical heads, wheels, ladders and other
  passive blocks generally offer little dedicated terminal automation. A block
  definition or interface in the game does not guarantee terminal discoverability.

## Getting the most from the remaining characters

1. **Publish zero-runtime-cost recipes first.** Document named/grouped terminal
   access for the missing families, using property/action IDs captured in game.
   Never invent IDs from terminal captions. Use the existing property/action
   discovery commands documented in `docs/EasyCommands/blockHandlers/terminal.md`.
2. **Inherit existing shared behavior.** New functional handlers already get
   enable/power and terminal behavior. Investigate missing converter types such
   as Int32/String only where an actual property dump demonstrates a need.
   Avoid float conversions for identifiers.
3. **Add small native slices where generic access cannot do the job.** Favor
   method calls, component state and useful typed selection. Avoid spending the
   budget on aliases for raw properties that already work comfortably.
4. **Reuse properties with consistent meanings.** Speed/altitude, range,
   state, repeat, and target can reuse existing machinery when semantics fit.
   Do not save characters by conflating block power with AI behavior activation.
5. **Reclaim duplicated implementation before deleting user vocabulary.** Look
   at shared production behavior and repeated component/selection patterns.
   Existing alias/default helpers have already been optimized; any further
   savings need fresh measurement. Do not assume generic/table-driven code is
   smaller until MDK packages it.
6. **Consider optional builds only if broader support becomes a requirement.**
   A core/AI/economy build could trade breadth for maintenance and user complexity.
   This requires a separate design; it is disproportionate for one small feature.

Proposed first implementation experiment: independently measure minimal AI Flight
and Action Relay handlers inheriting existing functional behavior. Choose
one useful slice that fits the approximately 1,000-character allocation. If none
fits, reclaim space before expanding runtime functionality. Keep full AI mission,
attack-pattern and store management out of that first slice.

## Handler and property inventory

Names below are proposed classes/properties, not existing EasyCommands syntax.
Every functional handler inherits enable/power, name, data, position, integrity,
terminal-property discovery and actions. These shared properties are omitted.
RO means read-only; commands are operations rather than persistent settings.
Direct API candidates below were checked against the configured DLL XML inventory.
Terminal-only candidates still require an in-game dump to confirm availability,
exact IDs, types, units and writability.

| Blocks | Handler needed | Candidate block-specific support |
| --- | --- | --- |
| AI Flight (Move) | `FlightMovementBlockHandler : FunctionalBlockHandler<IMyFlightMovementBlock>` | **Settings:** speed limit, minimum altitude, collision avoidance, precision mode, gravity alignment, flight mode, look-at position. **RO:** autopilot active, current waypoint, waypoint list. Nullable look-at position needs explicit clear/unset semantics. |
| AI Basic (Task) | `BasicMissionBlockHandler : FunctionalBlockHandler<IMyBasicMissionBlock>` plus mission-component helpers | **Settings:** selected mission; follow-player distance; follow-home minimum/maximum range and wander. **RO:** mission name, selected state, details. Use readable mission names where possible; internal selection IDs need accurate handling. |
| AI Recorder (Task) | `PathRecorderBlockHandler : FunctionalBlockHandler<IMyPathRecorderBlock>` plus `IMyPathRecorderComponent` helper | **Commands:** play, stop playback, record, stop recording, remove reference beacon. **Settings:** repeat, show path, show selected points, recording interval, minimum recording distance. **RO:** playing, recording, beacon name/position/ID. Full waypoint mutation is not exposed by this PB component. |
| AI Defensive (Combat) | `DefensiveCombatBlockHandler : FunctionalBlockHandler<IMyDefensiveCombatBlock>` plus shared enemy-search helper | **Command:** flee. **Settings/candidates:** flee trigger, custom retreat coordinates, waypoint zone size, target locking. **RO/candidates:** fleeing, resolved retreat coordinates, retreat GPS/beacon, enemy ID. Confirm each flee-location member's accessors before mapping writes. Shared search adds targeting lock options and target subsystems. |
| AI Offensive (Combat) | `OffensiveCombatBlockHandler : FunctionalBlockHandler<IMyOffensiveCombatBlock>` plus attack-pattern and enemy-search helpers | **Settings:** selected attack pattern, target priority, target update interval. **RO/candidates:** last scan, enemy ID, pattern name, has enemy. **Pattern settings:** orbit distance/gravity/facing; hit-and-run retreat timeout/distance/angle, break-off distance, selected weapons; intercept guidance/collision override; stay-at-range minimum/maximum distance, evasion toggle/distance, facing priority. **RO:** orbiting and approach/retreat state. |
| Event Controller | `EventControllerBlockHandler : FunctionalBlockHandler<IMyEventControllerBlock>` | **Settings:** threshold, AND mode, lower-or-equal versus higher-or-equal comparison. Other configuration requires independently verified terminal exposure; the direct PB interface does not expose event/block/toolbar editing. |
| Action Relay | `TransponderBlockHandler : FunctionalBlockHandler<IMyTransponder>` | **Settings:** channel, allowed sender relations. **Command:** send on configured channel or supplied channel. Reuse an existing numeric property for channel only if the meaning remains clear. |
| Broadcast Controller | `BroadcastControllerBlockHandler : FunctionalBlockHandler<IMyBroadcastControllerBlock>` plus `IMyChatBroadcastControllerComponent` helper | **Settings:** audience, broadcast author name, use antenna, indexed stored messages. **Commands:** send text, send stored message, send random message, send GPS. **RO:** message capacity. Keep broadcast author separate from the terminal block's custom name. |
| Artificial Mass | `VirtualMassBlockHandler<T> : FunctionalBlockHandler<T>` constrained to `IMyVirtualMass`, or a concrete artificial-mass handler | **RO:** virtual mass. Functional power is the primary control. A shared base can also serve Space Ball if its interface inheritance fits. |
| Space Ball | `SpaceBallBlockHandler : FunctionalBlockHandler<IMySpaceBall>` | **Settings:** virtual mass, friction, restitution/bounce, broadcasting. Verify the getter/setter distinction for `IsBroadcasting` versus `Broadcasting`; expose one clear public property. |
| Survival Kit | `SurvivalKitBlockHandler` using a new shared `ProductionBlockHandler<T> : FunctionalBlockHandler<T>` constrained to `IMyProductionBlock`, with subtype selection | **RO:** producing, queue empty, queue contents/item amounts, supported-blueprint query. **Settings:** conveyor use. **Commands:** add/insert/remove/move/clear queue items. Refactor reusable assembler queue logic; assembly/disassembly, cooperative mode and assembler progress stay specialized. |
| Store, Vending Machine | `StoreBlockHandler : FunctionalBlockHandler<IMyStoreBlock>`; optional vending subtype selector | **Settings:** searchable. **RO:** player listings (ID, item, amount, price and offer/order type as available). **Commands:** insert offer, insert order, cancel listing; expose operation result. Confirm vending inheritance before promising shared typed selection. |
| Safe Zone | `FunctionalBlockHandler<IMySafeZoneBlock>` initially; dedicated subclass only for confirmed terminal properties | **Terminal candidates:** zone active, radius/shape, size, permissions, filters, upkeep state. The PB interface adds no direct members; these are investigation targets, not confirmed mappings. Block power and zone activation are distinct. |
| Target Dummy | `FunctionalBlockHandler<IMyTargetDummyBlock>` initially; subclass if terminal mappings warrant it | **Terminal candidates:** restore/reset command, automatic restoration, restoration delay, hit/destruction state. Marker PB interface; discover actual available controls before selecting properties. |
| Emotion Controller | `FunctionalBlockHandler<IMyEmotionControllerBlock>` initially | **Terminal candidates:** emotion/preset, playback/trigger, range and repeat where exposed. Marker PB interface; no confirmed specialized members. |
| Medical Room | Typed functional handler if its installed PB interface supports it, otherwise subtype functional handler | **Terminal candidates:** healing/refuelling settings, respawn settings, suit selection/restriction. No promise of direct healing or respawn commands. Inventory/displays already use generic selectors. |
| Contract Block | Subtype terminal/functional handler after checking actual inheritance | **Terminal candidates:** public/access settings and UI actions. No confirmed PB contract-management API in this review; avoid inventing contract CRUD properties. |
| Button panels, control panels | Reuse `TerminalBlockHandler<T>` or `FunctionalBlockHandler<T>` according to actual interface inheritance | **Terminal candidates:** anyone-can-use and button labels where exposed. Custom panel screens use existing display support. No assumed press events, programmable button activation or toolbar editing. |
| Exhaust Pipe / Exhaust Cap | `ExhaustBlockHandler : SubTypedBlockHandler<IMyFunctionalBlock>` unless a suitable PB component is found | **Terminal candidates:** effect/preset and power dependency. Shared enable/power handles on/off. |
| Neon tubes / emissive blocks | `EmissiveBlockHandler : SubTypedBlockHandler<IMyFunctionalBlock>` unless existing lighting/component support applies | **Terminal candidates:** color and emissive settings. Check whether light/component inheritance permits existing handlers before duplicating mappings. |
| Upgrade modules | Generic or typed terminal handler; dedicated class only for useful exposed telemetry | **Candidates:** upgrade effects and connection state only if PB-accessible. No assumed configurable production multipliers. |
| Kitchen, decorative planter, service terminal | Inspect interfaces/components; reuse production, farming, inventory/display handlers where compatible | No confirmed unique automation properties. Do not equate decorative planters with farm plots or kitchens with food processors solely from their names. |

### Character-efficiency grouping

- **Existing functional handler plus registration:** marker-interface blocks and
  selectors whose only required behavior is enable/power. Their specialized
  terminal mappings can be added later without redesigning selection.
- **Small dedicated handlers:** Action Relay, Event Controller, AI Flight,
  artificial mass and Space Ball. Favor direct primitive properties.
- **Component-backed handlers:** Recorder, Broadcast and AI Basic. Reuse
  `RequireComponent<T>` from farming, while keeping the terminal block selected.
- **Shared helpers worth investigating:** production queue logic for assembler,
  refinery and Survival Kit; enemy-search logic for both combat AI blocks.
- **Potentially expensive properties:** nested listing/queue/waypoint data,
  weapon selection, attack-pattern dispatch, enum/name conversion and exact IDs.
  Evaluate these separately from the cheap selector and primitive settings.

## Evidence and limits

Reviewed the block registry, parser block registrations, shared terminal,
functional, inventory and specialized handlers; enumerated installed CubeBlocks
definition TypeIds; compared the configured game DLL XML API inventory with
Keen's public PB interface indexes. Selected interfaces were also decompiled from
the configured assemblies to distinguish PB methods from mod-only methods.
The installed Steam content and configured server binaries are separate sources;
the definitions are a cross-check, not proof of identical versions or runtime behavior.

Primary API references:

- [Sandbox PB interfaces](https://keensoftwarehouse.github.io/SpaceEngineersModAPI/api/Sandbox.ModAPI.Ingame.html)
- [Space Engineers PB interfaces](https://keensoftwarehouse.github.io/SpaceEngineersModAPI/api/SpaceEngineers.Game.ModAPI.Ingame.html)
- [AI Flight](https://keensoftwarehouse.github.io/SpaceEngineersModAPI/api/Sandbox.ModAPI.Ingame.IMyFlightMovementBlock.html)
- [Action Relay / transponder](https://keensoftwarehouse.github.io/SpaceEngineersModAPI/api/SpaceEngineers.Game.ModAPI.Ingame.IMyTransponder.html)
- [Event Controller](https://keensoftwarehouse.github.io/SpaceEngineersModAPI/api/Sandbox.ModAPI.Ingame.IMyEventControllerBlock.html)
- [Recorder component](https://keensoftwarehouse.github.io/SpaceEngineersModAPI/api/Sandbox.ModAPI.Ingame.IMyPathRecorderComponent.html)

This is a vanilla block-family/API review, not an exhaustive DLC-model certification
or a mod compatibility review. No live terminal property/action dump was captured,
and no new feature implementation was built to measure its incremental cost.
The Release source build/package passed; no live-game behavior was exercised.
