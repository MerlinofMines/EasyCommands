﻿# Gyroscope Block Handler
This Block Handler deals with Gyroscopes.  It allows you to get and set Gyroscope rotation values and power levels.

* Block Type Keywords: ```gyro```
* Block Type Group Keywords: ```gyros```

Default Primitive Properties:
* Bool - Enabled

## "Enabled" Property
* Primitive Type: Bool
* Keywords: ```enable, enabled```
* Inverse Keywords: ```disable, disabled```

Enables or Disables the given block

```
#Enable Block
enable "My Gyro"
set "My Gyro" to enabled
turn on "My Gyro"

#Disable Block
disable "My Gyro"
set "My Gyro" to disabled
turn off "My Gyro"
```

## "Power" Property
* Primitive Type: Numeric
* Keywords: ```power, powered```

Gets/Sets the Gyro Power limit.  Values are between 0 - 1, with 1 = 100% Power.

```
Print "Gyro Power: " + "My Gyro" power

set "My Gyro" power to 0.5
```

## "Auto" Property
* Primitive Type: Bool
* Keywords: ```auto, autopilot```

Gets/Sets whether the Gyroscope is on automatic, meaning is not on override but rather responding to user input.

```
if "My Gyro" is on auto
  Print "Rotation is governed by user input"

set "My Gyro" to auto
```

## "Limit" Property
* Primitive Type: Numeric
* Keywords: ```limit, limits```

Gets/Sets the Gyro Power limit.  Values are between 0 - 1, with 1 = 100% Power.

```
Print "Gyro Power: " + "My Gyro" limit

set "My Gyro" limit to 0.5
```

## "Roll" Property
* Primitive Type: Vector / Numeric / Bool
* Keywords: ```roll, rollInput, rotation```
* Supports Directions (Left, Right, Up, Down, Clockwise, CounterClockwise)

This is useful for automatically adjusting your orientation based on some other inputs.

If a direction is given, expects a Numeric and Gets/Sets the Gyro override in the given direction, in RPM.  

If no direction is given and a boolean is given, Gets/Sets whether Gyro Overrides are enabled.

If no direction is given and a vector is given, Gets/Sets the Gyro Overrides (Yaw:Pitch:Roll).

If no direction is given and no type is given, vector is assumed.

Note that Gyro overrides directions are based on the Gyro's orientation, not the cockpit's orientation.

```
Print "Gyro Left Rotation: " + "My Gyro" left rotation

#2 RPM rotating to the left
set "My Gyro" left rotation to 2

#3 RPM clockwise
set "My Gyro" clockwise roll to 3
```

## "Input" Property
* Primitive Type: Numeric
* Keywords: ```input, inputs```
* Supports Directions (Left, Right, Up, Down, Clockwise, CounterClockwise)

Same as Roll Property.

```
Print "Gyro Left Rotation: " + "My Gyro" left input

#2 RPM rotating to the left
set "My Gyro" left input to 2

#3 RPM clockwise
set "My Gyro" clockwise input to 3
```


## "Override" Property
* Primitive Type: Bool
* Keywords: ```override, overrides, overridden```

Same as Roll Property.

```
if "My Gyro" is overridden
  Print `Now I'm in control!`

#Turn on overrides
override "My Gyro"

#Yaw Override 5 RPM
set "My Gyro" left override to 5

#Print the Gyro Overrides, as a vector
Print "Gyro Overrides: " + "My Gyro" overrides
```
