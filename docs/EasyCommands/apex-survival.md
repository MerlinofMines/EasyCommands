# Apex Survival support

This update adds five selectors by extending existing handlers and composing public Ingame components. It introduces no new inventory or recipe language construct.

- [Farm Plot](https://spaceengineers.merlinofmines.com/EasyCommands/blockHandlers/farmplot): occupancy, harvestability, crop identity/liveness/maturity, and water storage.
- [Algae Farm / Half Algae Farm](https://spaceengineers.merlinofmines.com/EasyCommands/blockHandlers/algaefarm): item rate and batch countdown.
- [Food Processor](https://spaceengineers.merlinofmines.com/EasyCommands/blockHandlers/foodprocessor): an AssemblerBlockHandler subclass with specialized selection.
- [Irrigation System](https://spaceengineers.merlinofmines.com/EasyCommands/blockHandlers/irrigation): a GasGeneratorHandler subclass with specialized selection.
- [Oxygen Farm / Half Oxygen Farm](https://spaceengineers.merlinofmines.com/EasyCommands/blockHandlers/oxygenfarm): enabled and production-readiness controls.

Existing Type II tools/detectors, Survival Kits, storage inventories, and terrarium/inset-planter lights continue to use their existing interface-based handlers. New `running` properties extend assemblers and generators; assembler `progress` is also exposed. Shared implementation remains in the parent classes so future parent improvements are inherited.

## Items and recipes

[Item aliases](https://spaceengineers.merlinofmines.com/EasyCommands/items#apex-survival-items) cover ingredients, meals, seeds/spores, and medical items. Seeds use `MyObjectBuilder_SeedItem`; algae/grain use `MyObjectBuilder_PhysicalObject`; other food uses `MyObjectBuilder_ConsumableItem`. Inventory identities and recipe blueprints are mapped separately.

Existing inventory counting/transfers, assembler enqueue semantics, raw IDs, parent selectors, and read-only setter behavior are preserved. `food`, `ingredients`, `meals`, and `seeds` are inventory categories, not batch-production recipes. Loot-only aliases have no built-in blueprint mappings; unknown blueprint names retain the legacy raw-ID fallback.

## Compatibility and validation boundary

New global words are `crop`, `alive`, `dead`, and the five selector families (with plurals). Existing unquoted identifiers using those words may need quoting. Existing enums retain their numeric values; new values are appended.

Known vanilla subsets use exact subtype identities. Broad parent selectors remain interface-based and can discover additional variants. New reskins/mod variants may require adding their subtype to the specialized selector; arbitrary future API breaking changes cannot be guaranteed compatible.

The implementation uses public Ingame interfaces and `Components.TryGet<T>`, never concrete game classes or reflection. Definitions and API signatures were inspected from installed Steam build 24675677. Source and packed-script analyzer/compiler validation and mocked execution tests do not replace live-world testing. Historical 1.207 binary compatibility has not been established; compilation requires the component APIs in the referenced game build.

Before release, manually verify component lookup and PB execution; empty/dead/mature crop transitions; water units; sunlight/full-inventory algae behavior; and food/seed recipe production on the supported client/server builds. Oxygen output units, crop growth/health percentages, planting/harvesting actions, and plot-light aliases remain deferred. No world was modified during implementation.

Sources: [official Apex release](https://www.spaceengineersgame.com/space-engineers-apex-survival-live-now/), [PB farm API](https://keensoftwarehouse.github.io/SpaceEngineersModAPI/api/Sandbox.ModAPI.Ingame.IMyFarmPlotLogic.html), and [PB solar-food API](https://keensoftwarehouse.github.io/SpaceEngineersModAPI/api/Sandbox.ModAPI.Ingame.IMySolarFoodGenerator.html).
