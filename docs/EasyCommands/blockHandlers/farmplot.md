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
| `crop output`, `crop volume` | Numeric | Potential harvest item count reported by the game; may be nonzero before maturity or for dead crops |
| `crop input` | Numeric | Seeds required to plant this plot |
| `crop` | String | Output item in `TypeId.SubtypeId` format; empty string when unplanted |
| `percentage`, `ratio`, `progress` | Numeric | Water fill from 0 to 1, **not crop growth** |
| `capacity`, `range` | Numeric | Water storage capacity in liters |
| `level` | Numeric | Water stored in liters |

The numeric default is water ratio. String/Boolean defaults remain name/enabled. Bare `complete` still means structural completion. Assignments to read-only properties have no effect.

```
print "Bed" farmplot percentage
if "Bed" farmplot is occupied and "Bed" farmplot crop alive and "Bed" farmplot crop complete
  print "Mature living crop"
```

Turning off a plot does not pause crop growth. These commands read crop status; use the game's interactions for planting, harvesting, and removing plants.

Use `crop output` with `ready` and `crop alive` when deciding whether a living crop is ready to collect. The potential yield can change as crop health changes. `crop input` reports the seed requirement; it does not plant seeds. Numeric growth and health are not exposed.

## Plot lighting

The built-in lamp uses the same property words as a [Light](https://spaceengineers.merlinofmines.com/EasyCommands/blockHandlers/light):

| Property words | Meaning (read/write) |
| --- | --- |
| `color` | Lamp color |
| `radius` | Light radius in meters |
| `intensity`, `output`, `volume` | Lamp intensity |
| `falloff` | Light attenuation setting |
| `interval` | Blink interval in seconds |
| `interval length`, `interval level` | Blink on-time as a percentage of the interval, 0–100 |
| `offset` | Blink phase offset, 0–100 percent |

Use `interval length` for blinking: bare `length` / `level` still means stored water, and `range` / `capacity` still means water capacity. `crop output` is crop yield; bare `output` is lamp intensity. Lamp settings do not change crop growth requirements.

```
set "Bed" farm plot color to green
set "Bed" farm plot radius to 8
set "Bed" farm plot intensity to 2
set "Bed" farm plot interval to 3
set "Bed" farm plot interval length to 50
```
