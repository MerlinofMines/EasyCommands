# Camera Block Handler
This block handler enables you to programatically scan and detect entities from a camera.

This block handler makes significant use of the Camera Block's Custom Data, so take heed in case you have another script/mod that needs Camera Custom Data.

* Block Type Keywords: ```camera```
* Block Type Group Keywords: ```cameras```

Default Primitive Properties:
* Boolean - Trigger (Overrides Power from Terminal Block)
* Numeric - Range
* Vector - Target

## "Enabled" Property
* Primitive Type: Bool
* Keywords: ```enable, enabled```
* Inverse Keywords: ```disable, disabled```

Enables or Disables the given block

```
#Enable Block
enable "My Camera"
set "My Camera" to enabled

#Disable Block
disable "My Camera"
set "My Camera" to disabled
```

## "Power" Property
* Primitive Type: Bool
* Keywords: ```power, powered```

Turns on or off power to the block.  Effectively the same as the Enabled property.

```
#Turn on
turn on power to "My Camera"
power on "My Camera"

#Turn off
turn off "My Camera"
power off "My Camera"
```

## "Range" Property
* Primitive Type: Numeric
* Keywords: ```range, ranges, distance, distances, limit, limits```

This property Get/Sets the range, in meters, for which the camera will scan when looking for entities.  Since cameras do not have a property to store this value, setting this property will add a custom property to the Camera's CustomData for this value.

```
Print "Camera Range: " + "My Camera" range

set "My Camera" range to 2000
```

## "Trigger" Property
* Primitive Type: Bool
* Keywords: ```trigger, triggered, trip, tripped, detect, detected```

This property returns when the camera detects an entity.  After an entity is detected you can use the "Target" and "Target Velocity" properties to get information about the detected entity.
The triggered property obeys the "Range" property when attempting to detect entities.  In other words, it will automatically wait until it can scan the requested distance.

Once an entity is detected, the entity's coordinates and velocity are stored as Custom Data in the Camera.

If you set this property to true, it enables raycast on the camera.

```
#When is used so that the camera continually scans the requested range
set "My Camera" range to 2000
when "My Camera" is triggered
  Print "Detected an Entity!"
  Print "Target Coords: " + "My Camera" target
  Print "Target Velocity: " + "My Camera" targetVelocity

#Enable Raycast
tell "My Camera" to detect
```

## "Target" Property
* Read-only
* Primitive Type: Vector
* Keywords: ```target, coords, coordinates```

Returns the target coordinates, in World Coordinates, of the last detected entity.  If no entity has been detected, returns 0:0:0.

```
when "My Camera" is triggered
  Print "Target Coords: " + "My Camera" target
```

## "Target Velocity" Property
* Read-only
* Primitive Type: Vector
* Keywords: ```targetVelocity```

Returns the target velocity, oriented in World Coordinates, of the last detected entity.  If no entity has been detected, returns 0:0:0.

```
when "My Camera" is triggered
  Print "Target Velocity: " + "My Camera" targetVelocity
```