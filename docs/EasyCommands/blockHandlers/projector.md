# Projector Block Handler
This Block Handler gives you basic information on Projectors such as the build % and if it is complete.

Support for getting/setting the offsets and rotation will be added in a future release.

* Block Type Keywords: ```projector```
* Block Type Group Keywords: ```projectors```

## "Enabled" Property
* Primitive Type: Bool
* Keywords: ```enable, enabled```
* Inverse Keywords: ```disable, disabled```

Enables or Disables the given block

```
#Enable Block
enable "My Projector"
set "My Projector" to enabled
turn on "My Projector"

#Disable Block
disable "My Projector"
set "My Projector" to disabled
turn off "My Projector"
```

## "Power" Property
* Primitive Type: Bool
* Keywords: ```power, powered```

Turns on or off power to the block.  Effectively the same as the Enabled property.

```
#Turn on
turn on power to "My Projector"
power on "My Projector"

#Turn off
turn off "My Projector"
power off "My Projector"
```

## "Ratio" Property
* Read-only
* Primitive Type: Numeric
* Keywords: ```ratio, ratios, percentage, percentages, percent, percents```

Gets a value between 0 - 1 representing how close the Projector's blueprint is to completion.

```
Print "Percentage Done: " + "My Projector" percentage
```

## "Complete" Property
* Read-only
* Primitive Type: Bool
* Keywords: ```done, ready, finished, complete```

Gets whether the projector's blueprint is complete or not.

```
when "My Projector" is complete
  Print "My Creation is ready!"
```

## "Offset" Property
* Primitive Type: Vector/Numeric
* Keywords: ```offset, offsets```
* Supported Directions (Up, Down, Left, Right, Forwards, Backwards)

Gets/Sets the projector's blueprint offset.  If no direction is used then a Vector is returned or expected.  When set to a vector, this property sets the projector offset using the vector's components as Left:Upper:Forwards.

If a supported direction is passed, then this property returns and expects a numeric Value.

```
Print "Offset: " + "My Projector" offset

set "My Projector" offset to 1:2:3

print "Right Offset: " + "My Projector" right offset

set "My Projector" right offset to 4
```

## "Rotation" Property
* Primitive Type: Vector/Numeric
* Keywords: ```rotation, rotations, roll, rolls```
* Supported Directions (Up, Down, Left, Right, Clockwise, Counterclockwise)

Gets/Sets the projector's blueprint rotation.  If no direction is used then a Vector is returned or expected.  When set to a vector, this property sets the projector offset using the vector's components as Yaw:Pitch:Roll.

If a supported direction is passed, then this property returns and expects a numeric Value.  Up/Down is pitch, Left/Right is Yaw, Clockwise/Counter is Roll.

Expected Values are between -2 and 2 for all values.  0 = 0%, 1 = 90%, 2 = 180%, -1 = -90%, -2 = -180%.

If a value < -2 or greater than 2 is passed the value is clamped to -2 or 2, respectively.

```
Print "Rotation: " + "My Projector" rotation

set "My Projector" rotation to 1:2:3

print "Right Rotation: " + "My Projector" right rotation

set "My Projector" right rotation to 1

#Clamped to 2
set "My Projector" right rotation to 4
```

## "Show" Property
* Primitive Type: bool
* Keywords: ```show, showing```
* Inverse Keywords: ```hide, hiding```

Gets/Sets whether the projector's blueprint is currently showing.  When retrieving, will return whether the projection is currently being projected.  When setting, will set whether the projector shows only buildable when set to false.

```
Print "Showing: " + "My Projector" is showing

#Show entire projection
show "My Projector"

#Show only buildable
hide "My Projector"
```

## "Lock" Property
* Primitive Type: bool
* Keywords: ```lock, locked, permanent, freeze```
* Inverse Keywords: ```unlock, unlocked```

Gets/Sets whether the projection is kept after the build is complete.

```
#Keep Projection After Build Completion
lock "My Projector"

#Do Not Keep Projection After Build Completion
unlock "My Projection"
```

## "Scale" Property
* Primitive Type: numeric
* Keywords: ```scale, height```

Gets/Sets the Projector's Scale, as a value from 0 - 1, where 1 = 100% scale.  Note that not all Projectors support this property.