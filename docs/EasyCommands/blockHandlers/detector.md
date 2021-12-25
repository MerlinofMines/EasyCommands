# Ore Detector Block Handler
This block handler gives you some basic control over Ore Detectors, including setting their range, whether or not they are enabled, and whether or not they are broadcasting.

* Block Type Keywords: ```detector```
* Block Type Group Keywords: ```detectors```

Default Primitive Properties:
* Bool - Enabled
* Numeric - Range

Default Directional Properties
* Up - Range

## "Enabled" Property
* Primitive Type: Bool
* Keywords: ```enable, enabled```
* Inverse Keywords: ```disable, disabled```

Enables or Disables the given block

```
#Enable Block
enable "My Ore Detector"
set "My Ore Detector" to enabled
turn on "My Ore Detector"

#Disable Block
disable "My Ore Detector"
set "My Ore Detector" to disabled
turn off "My Ore Detector"
```

## "Power" Property
* Primitive Type: Bool
* Keywords: ```power, powered```

Turns on or off power to the block.  Effectively the same as the Enabled property.

```
#Turn on
turn on power to "My Ore Detector"
power on "My Ore Detector"

#Turn off
turn off "My Ore Detector"
power off "My Beacon"
```

## "Range" Property
* Primitive Type: Numeric
* Keywords: ```range, ranges, distance, distances, limit, limits, radius, radii```

Gets/Sets the range, in meters, of the Ore Detector, meaning the radius in which it will detect nearby ores.

```
Print "Range: " + "My Ore Detector" range

set "My Ore Detector" range to 200
```

## "Broadcast" Property
* Primitive Type: Bool
* Keywords: ```broadcast, broadcasting```

Gets/Sets whether the Ore Detector is broadcasting, meaning that detected ores will be shared via any connected antenna.

```
if "My Ore Detector" is broadcasting
	Print "Sharing Ore Locations"

tell "My Ore Detector" to broadcast
```