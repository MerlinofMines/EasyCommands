# HeatVent Block Handler
This Block Handler can be used to control HeatVents

* Block Type Keywords: ```heatVent```
* Block Type Group Keywords: ```heatVents```

Default Primitive Properties:
* Boolean - Enabled
* Numeric - Ratio
* Color - Color

Default Directional Properties
* Up - Color

## "Enabled" Property
* Primitive Type: Bool
* Keywords: ```enable, enabled```
* Inverse Keywords: ```disable, disabled```

Enables or Disables the given block

```
#Enable Block
enable "My HeatVent"
set "My HeatVent" to enabled
turn on "My HeatVent"

#Disable Block
disable "My HeatVent"
set "My HeatVent" to disabled
turn off "My HeatVent"
```

## "Power" Property
* Primitive Type: Bool
* Keywords: ```power, powered```

Turns on or off power to the block.  Effectively the same as the Enabled property.

```
#Turn on
turn on power to "My HeatVent"
power on "My HeatVent"

#Turn off
turn off "My HeatVent"
power off "My HeatVent"
```

## "Color" Property
* Primitive Type: Color
* Supported Directions: Up, Down
* Keywords: ```color, colors```

Gets/Sets the HeatVent's max/min colors.  If a direction is passed, sets the max or min color depending on if you specify Up or Down.

If no direction is passed, the max color is Get/Set.

```
#Get the colors
print "My HeatVent" upper color
print "My HeatVent" lower color

#Set the colors
set "My HeatVent" color to red
set "My HeatVent" upper color to white
set "My HeatVent lower color to red
```

## "Radius" Property
* Primitive Type: Numeric
* Keywords: ```radius, radii```

Gets/Sets the HeatVent's light radius, in meters.

```
#Get the radius
print "My HeatVent" radius

#Set the radius
set "My HeatVent" radius to 5
```

## "Range" Property
* Primitive Type: Numeric
* Keywords: ```range, ranges, distance, distances, limit, limits```

Same as Radius. Gets/Sets the HeatVent's light radius, in meters.

```
#Get the range
print "My HeatVent" range

#Set the range
set "My HeatVent" range to 5
```

## "Intensity" Property
* Primitive Type: Numeric
* Keywords: ```intensity, intensities```

Gets/Sets the HeatVent's light intensity.

```
#Get the intensity
print "My HeatVent" intensity

#Set the intensity
set "My HeatVent" intensity to 5
```

## "Falloff" Property
* Primitive Type: Numeric
* Keywords: ```falloff, falloffs```

Gets/Sets the HeatVent's light falloff.

```
#Get the falloff
print "My HeatVent" falloff

#Set the falloff
set "My HeatVent" falloff to 5
```

## "Offset" Property
* Primitive Type: Numeric
* Keywords: ```offset, offsets```

Gets/Sets the HeatVent's offset.  Values are expected to be between 0 - 1.

```
#Get the offset
print "My HeatVent" offset

#Set the offset
set "My HeatVent" offset to 0.5
```

## "Ratio" Property
* Primitive Type: Numeric
* Keywords: ```ratio, ratios, percent, percents, percentage, percentages```

Gets/Sets the HeatVent's Power Dependency ratio, as a value between 0 - 100 representing the power dependency %.

```
#Get the Power Dependency
print "My HeatVent" ratio

#Set the Power Dependency to 50%
set "My HeatVent" ratio to 50
```