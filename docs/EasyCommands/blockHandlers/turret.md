# Turret Block Handler

This Block Handler handles turrets.  It extends from the [Gun Handler](https://spaceengineers.merlinofmines.com/EasyCommands/blockHandlers/gun "Gun Handler") so it has all of those properties in addition to those below.

* Block Type Keywords: ```turret```
* Block Type Group Keywords: ```turrets```

Default Primitive Properties:
* Boolean - Enabled
* Numeric - Range
* Vector - Target

Default Directional Properties
* Up - Range

## "Range" Property
* Primitive Type: Numeric
* Keywords: ```range, ranges, limit, limits, distance, distances```

Gets/Sets the turret's AI aiming distance, in meters.

```
print "Turret Range: " + "My Turret" range

#Set AI aiming distance to 800
set "My Turret" range to 800
```

## "Locked" Property
* Primitive Type: Bool
* Keywords: ```lock, locked, locking```
* Inverse Keywords: ```unlock, unlocked```

Gets/Sets whether the turret has target locking enabled.  

```
if "My Turret" locking is on
  Print "Target Locking is on!"

#Disable Target Locking
turn off "My Turret" locking
```

## "Occupied" Property
* Read-only
* Primitive Type: Bool
* Keywords: ```use, used, occupy, occupied, control, controlled```
* Inverse Keywords: ```unused, unoccupied, available```

Returns whether the Turret is currently under control.

```
Print "Occupied: " + "My Turret" is occupied
Print "In Use: " + "My Turret" is in use
Print "Controlled: " + "My Turret" is being controlled
```

## "Rotation" Property
* Primitive Type: Bool
* Keywords: ```rotation```

Gets/Sets whether the turret has idle movement enabled

```
if "My Turret" rotation is on
  Print "Moving about aimlessly.."

#Turn off Idle Movement
"My Turret" rotation off
```

## "Target" Property
* Primitive Type: Vector/String/Bool
* Keywords: ```target, targets```


Gets/Sets the turret's current target, if one exists.  When getting, will return either the manually set target, or the position of the targeted entity.  If the turret has no target, it will return 0:0:0.  When setting this property to a vector, the turret's target will be set to the static coordinates.  So if you set this to a moving target, it *will not auto track*.


If compared to a boolean when getting, will return whether the turret currently has a target.  When set to a boolean, will reset the turret's current target if the boolean is false, and does nothing otherwise.

If compared to a string when getting, will return the turret's current targeting option.  If set to a string, this property will set the turret's targeting option.  Currently supported targeting options are: ```"Default", "Weapons", "Propulsion", "Power Systems"```.  Attempting to set the targeting option to any other string value will cause an exception.


This property makes use of custom data on the Turret itself (to keep track of manual target).

```
#Returns the vector of the target's location
Print "Target Location: " + "My Turret" target

#Maybe not a good idea...
set "My Turret" target to my position

#Check if turret has a target
if "My Turret" target is true
  Print "Target Acquired!"

#Reset turret's target
turn off "My Turret" targeting

#Check if turret is targeting weapons
if "My Turret" is targeting "Weapons"
  print "Targeting Enemy Weapons"

#Set targeting option to weapons
tell "My Turret" to target "Weapons"
```

## "Target Velocity" Property
* Primitive Type: Vector
* Keywords: ```target velocity, target velocities```

Gets/Sets the velocity of the turret's current target, in world coordinates.  If the turret has no target, it will return 0:0:0.

When setting this property, the turret will be set to track the target assuming it maintains the velocity vector you've specified.  This is intended to be used in combination with the target property for manually tracking targets.

```
Print "Target Velocity: " + "My Turret" target velocity

#Definitely not a good idea!
set "My Turret" target to my position
set "My Turret" target velocity to my ship velocity
```

## "Angle" Property
* Primitive Type: Numeric
* Keywords: ```angle, angles, azimuth, azimuths```

Gets/Sets the turret's current Azimuth, in degrees

```
print "Turret Azimuth: " + "My Turret" azimuth

set "My Turret" azimuth to 90
```

## "Elevation" Property
* Primitive Type: Numeric
* Keywords: ```elevation, elevations```

Gets/Sets the turret's current Elevation, in degrees

```
print "Turret Elevation: " + "My Turret" elevation

set "My Turret" elevation to 45
```