# Farm Plot

Keywords: `farmplot`, `farmplots`.

Selects the vanilla Farm Plot. It inherits functional/terminal properties such as power, name, data, position, and structural damage. Crop and water readings come from the block's attached PB components.

| Property words | Type | Meaning (read-only) |
| --- | --- | --- |
| `occupied`, `use` | Boolean | A plant is present, including a dead plant |
| `ready`, `able` | Boolean | Harvestable; may also be true for dead plants |
| `crop complete` | Boolean | Plant is fully grown |
| `crop alive` | Boolean | A living plant is present |
| `crop dead` | Boolean | Inverse of crop alive; also true when empty, so check occupied first |
| `crop` | String | Output item in `TypeId.SubtypeId` format; empty string when unplanted |
| `percentage`, `ratio`, `progress` | Numeric | Water fill from 0 to 1, **not crop growth** |
| `capacity`, `range` | Numeric | Water storage capacity in game resource units |
| `level` | Numeric | Water stored, in the same units as capacity |

The numeric default is water ratio. String/Boolean defaults remain name/enabled. Bare `complete` still means structural completion. Read-only writes retain EasyCommands' existing no-op behavior.

```
print "Bed" farmplot percentage
if "Bed" farmplot is occupied and "Bed" farmplot crop alive and "Bed" farmplot crop complete
  print "Mature living crop"
```

Turning off a plot does not pause crop growth. Farm Plots do not expose a seed inventory through this handler. Planting, harvesting, and removing plants are not exposed by the inspected Ingame farm component; this handler does not call mod-only methods. Use the game's supported farming interactions and collector arrangements. Numeric health/growth, plot-light properties, and automatic planting are not implemented.

Missing crop/water components and storage for the wrong resource produce errors instead of fabricated readings. Requires game references with the Apex component interfaces; current build validation is described in [Apex support](https://spaceengineers.merlinofmines.com/EasyCommands/apex-survival).
