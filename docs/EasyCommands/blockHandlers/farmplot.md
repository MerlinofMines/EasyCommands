# Farm Plot

Keywords: `farmplot`, `farm plot` (single); `farmplots`, `farm plots` (group).

Selects the vanilla Farm Plot. It inherits functional/terminal properties such as power, name, data, position, and structural damage.

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

The numeric default is water ratio. String/Boolean defaults remain name/enabled. Bare `complete` still means structural completion. Assignments to read-only properties have no effect.

```
print "Bed" farmplot percentage
if "Bed" farmplot is occupied and "Bed" farmplot crop alive and "Bed" farmplot crop complete
  print "Mature living crop"
```

Turning off a plot does not pause crop growth. These commands read crop status; use the game's interactions for planting, harvesting, and removing plants.
