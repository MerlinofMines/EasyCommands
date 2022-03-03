# Searchlight Block Handler
This block handler handles Searchlights.

* Block Type Keywords: ```searchlight```
* Block Type Group Keywords: ```searchlights```

Default Primitive Properties:
* Bool - Enabled
* Numeric - Range
* Color - Color

Default Directional Properties
* Up - Range

## "Enabled" Property
* Primitive Type: Bool
* Keywords: ```enable, enabled```
* Inverse Keywords: ```disable, disabled```

Enables or Disables the given block

```
#Enable Block
enable "My Searchlight"
set "My Searchlight" to enabled
turn on "My Searchlight"

#Disable Block
disable "My Searchlight"
set "My Searchlight" to disabled
turn off "My Searchlight"
```

## "Power" Property
* Primitive Type: Bool
* Keywords: ```power, powered```

Turns on or off power to the block.  Effectively the same as the Enabled property.

```
#Turn on
turn on power to "My Searchlight"
power on "My Searchlight"

#Turn off
turn off "My Searchlight"
power off "My Searchlight"
```

## "Radius" Property
* Primitive Type: Numeric
* Keywords: ```radius, radii```

Gets/Sets the light radius, in meters, of the Searchlight.

```
Print "Range: " + "My Searchlight" radius

set "My Searchlight" radius to 200
```

## "Range" Property
* Primitive Type: Numeric
* Keywords: ```range, ranges, distance, distances, limit, limits```

Gets/Sets the AI aiming distance, in meters, of the Searchight.

```
Print "Range: " + "My Searchlight" range

set "My Searchlight" range to 200
```

## "Color" Property
* Primitive Type: Color
* Keywords: ```color, colors```

Gets/Sets the color of the Searchlight.

You can specify one of the known [Colors](https://spaceengineers.merlinofmines.com/EasyCommands/cheatsheet#colors) or provide a HEX value for your own color.

```
Print "Searchlight Color: " + "My Searchlight" color

set "My Searchlight" color to red

#Magenta
set "My Searchlight" to #FF00FF
```

## "Intensity" Property
* Primitive Type: Numeric
* Keywords: ```intensity, intensities, output, outputs```

Gets/Sets the intensity of the Searchlight.

```
Print "Intensity: " + "My Searchlight" intensity

set "My Searchlight" intensity to 50
```

## "Interval" Property
* Primitive Type: Numeric
* Keywords: ```interval, intervals```

Get/Sets the blink interval for the given Searchlight, in seconds.

```
Print "Blink Interval: " + "My Searchlight" interval

#Set to 2 times per second
set "My Searchlight" interval to 0.5
```

## "Length" Property
* Primitive Type: Numeric
* Keywords: ```length, lengths```

Gets/Sets the Searchlights Blink Length, as a percentage of it's interval.  Values are between 0-100.

```
#Set searchlight to blink once per second using Length and Interval properties
set "My Searchlight" interval to 1 second
set "My Searchlight" length to 50
```

## "Offset" Property
* Primitive Type: Numeric
* Keywords: ```offset, offsets```

Get/Sets the offset of the Searchlight light source, in meters.  This is not to be confused with "Blink Offset", which is not (yet) mapped directly to a property.  You can still set the Blink Offset using [Terminal Properties](https://spaceengineers.merlinofmines.com/EasyCommands/blockHandlers/terminal#property-property).

```
Print "Offset: " + "My Searchlight" offset

#Set Searchlight Offset to 3 meters above the Searchlight
set "My Searchlight" offset to 3
```

## "Falloff" Property
* Primitive Type: Numeric
* Keywords: ```falloff, falloffs```

Gets/Sets the falloff for the given Searchlight, in seconds.

```
Print "Falloff: " + "My Searchlight" falloff

set "My Searchlight" falloff to 3
```

## "Rotation" Property
* Primitive Type: Bool
* Keywords: ```rotation, rotations```

Gets/Sets whether idle movement is enabled for the given Searchlight.

```
Print "Idle Movement: " + "My Searchlight" rotation is true

turn off "My Searchlight" rotation
```

## "Locked" Property
* Primitive Type: Bool
* Keywords: ```lock, locked, locking```

Gets/Sets whether Target Locking is enabled for the given Searchlight.

```
Print "Target Locking: " + "My Searchlight" locking is true

turn off "My Searchlight" locking
```