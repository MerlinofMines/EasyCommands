# Light Block Handler
This block handler handles lights and spotlights.

* Block Type Keywords: ```light, spotlight```
* Block Type Group Keywords: ```lights, spotlights```

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
enable "My Light"
set "My Light" to enabled
turn on "My Light"

#Disable Block
disable "My Light"
set "My Light" to disabled
turn off "My Light"
```

## "Power" Property
* Primitive Type: Bool
* Keywords: ```power, powered```

Turns on or off power to the block.  Effectively the same as the Enabled property.

```
#Turn on
turn on power to "My Light"
power on "My Light"

#Turn off
turn off "My Light"
power off "My Light"
```

## "Range" Property
* Primitive Type: Numeric
* Keywords: ```range, ranges, distance, distances, limit, limits, radius, radii```

Gets/Sets the range, in meters, of the Light.

```
Print "Range: " + "My Light" range

set "My Light" range to 200
```

## "Color" Property
* Primitive Type: Color
* Keywords: ```color, colors```

Gets/Sets the color of the Light.

You can specify one of the known [Colors](https://spaceengineers.merlinofmines.com/EasyCommands/cheatsheet#colors) or provide a HEX value for your own color.

```
Print "Light Color: " + "My Light" color

set "My Light" color to red

#Magenta
set "My Light" to #FF00FF
```

## "Intensity" Property
* Primitive Type: Numeric
* Keywords: ```intesity, intensities, output, outputs```

Gets/Sets the intensity of the Light.

```
Print "Intensity: " + "My Light" intensity

set "My Light" intensity to 50
```

## "Interval" Property
* Primitive Type: Numeric
* Keywords: ```interval, intervals```

Get/Sets the blink interval for the given light(s), in seconds.

```
Print "Blink Interval: " + "My Light" interval

#Set to 2 times per second
set "My Light" interval to 0.5
```

## "Length" Property
* Primitive Type: Numeric
* Keywords: ```length, lengths```

Gets/Sets the Lights Blink Length, as a percentage of it's interval.  Values are between 0-100.

```
#Set light to blink once per second using Length and Interval properties
set "My Light" interval to 1 second
set "My Light" length to 50
```

## "Offset" Property
* Primitive Type: Numeric
* Keywords: ```offset, offsets```

Get/Sets the blink offset (when during each interval does the light start blinking?) for the given light(s), in seconds. Values are between 0 - 100.

```
Print "Blink Offset: " + "My Light" offset

set "My Light" offset to 50
```

## "Falloff" Property
* Primitive Type: Numeric
* Keywords: ```falloff, falloffs```

Gets/Sets the falloff for the given lights(s), in seconds.

```
Print "Falloff: " + "My Light" falloff

set "My Light" falloff to 3
```