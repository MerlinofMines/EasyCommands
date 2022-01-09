# Tank Block Handler
This Block Handler handles both Oxygen and Hydrogen tanks.  It enables you to get information about stored O2/H2 levels and set the tank to stockpile or not, and whether auto-refill bottles or not.

* Block Type Keywords: ```tank```
* Block Type Group Keywords: ```tanks```

Default Primitive Properties:
* Numeric - Ratio

Default Directional Properties
* Up - Ratio

## "Enabled" Property
* Primitive Type: Bool
* Keywords: ```enable, enabled```
* Inverse Keywords: ```disable, disabled```

Enables or Disables the given block

```
#Enable Block
enable "My Tank"
set "My Tank" to enabled
turn on "My Tank"

#Disable Block
disable "My Tank"
set "My Tank" to disabled
turn off "My Tank"
```

## "Power" Property
* Primitive Type: Bool
* Keywords: ```power, powered```

Turns on or off power to the block.  Effectively the same as the Enabled property.

```
#Turn on
turn on power to "My Tank"
power on "My Tank"

#Turn off
turn off "My Tank"
power off "My Tank"
```

## "Supply" Property
* Primitive Type: Bool
* Keywords: ```supply```
* Inverse Keywords: ```stockpile, stockpiling, collect, collecting```

Gets/Sets whether the tank is set to stockpile

```
if "My Tank" is collecting
  Print "Tank is stockpiling"

tell "My Tank" to stockpile
```

## "Limit" Property
* Read-only
* Primitive Type: Numeric
* Keywords: ```limit, limits, capacity, capacities```

Gets the capacity of the tank, in liters.

```
Print "Tank Capacity: " + "My Tank" capacity
```

## "Ratio" Property
* Read-only
* Primitive Type: Numeric
* Keywords: ```ratio, ratios, percentage, percentages, percent, percents```

Gets the percentage that the tank is filled, as a value from 0-1 (0 = empty, 1 = 100% full)

```
Print "Tank Fill Ratio: " + "My Tank" ratio
```

## "Level" Property
* Read-only
* Primitive Type: Numeric
* Keywords: ```level, levels```

Gets the approximate level of the tank, in L, by multiplying the tank's capacity by it's current fill ratio.  So if it has as 10000L capacity and is 40% full, would return 4000.

```
Print "Tank Level: " + "My Tank" level
```

## "Auto" Property
* Primitive Type: Bool
* Keywords: ```auto, refill```

Gets/Sets whether the tank is set to auto-refill bottles.

```
if "My Tank" is on auto
  Print "Tank is auto refilling"

#Set My Tank to auto-refill
tell "My Tank" to refill
```