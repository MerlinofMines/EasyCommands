# Turret Block Handler

This Block Handler handles Custom Turret Controllers.  

* Block Type Keywords: ```turretcontroller```
* Block Type Group Keywords: ```turretcontrollers```

Default Primitive Properties:
* Boolean - Enabled
* Numeric - Range

Default Directional Properties
* Up - Range

## "Range" Property
* Primitive Type: Numeric
* Keywords: ```range, ranges, limit, limits, distance, distances```

Gets/Sets the turret's AI aiming distance, in meters.

```
print "Turret Range: " + "My TurretController" range

#Set AI aiming distance to 800
set "My TurretController" range to 800
```

## "Locked" Property
* Primitive Type: Bool
* Keywords: ```lock, locked, locking```
* Inverse Keywords: ```unlock, unlocked```

Gets/Sets whether the turret has target locking enabled.  

```
if "My TurretController" locking is on
  Print "Target Locking is on!"

#Disable Target Locking
turn off "My TurretController" locking
```

## "Auto" Property
* Primitive Type: Bool
* Keywords: ```auto```

Gets/Sets whether the turret has AI enabled, meaning whether the turret will automatically detect, target, and shoot at detected entities.

```
if "My TurretController" is on auto
  Print "Sentry Active!"

#Disable Target Locking
turn off "My TurretController" auto
```

## "Occupied" Property
* Read-only
* Primitive Type: Bool
* Keywords: ```use, used, occupy, occupied, control, controlled```
* Inverse Keywords: ```unused, unoccupied, available```

Returns whether the Turret is currently under control.

```
Print "Occupied: " + "My TurretController" is occupied
Print "In Use: " + "My TurretController" is in use
Print "Controlled: " + "My TurretController" is being controlled
```

## "Target" Property
* Primitive Type: Vector/String/Bool
* Keywords: ```target, targets```

When retrieving, gets the turret's current target, if one exists.  If the turret has no target, it will return 0:0:0.  Setting the target to a vector does nothing.

If compared to a boolean when getting, will return whether the turret currently has a target.  Setting the target to a boolean does nothing.

If compared to a string when getting, will return the turret's current targeting option.  If set to a string, this property will set the turret's targeting option.  Currently supported targeting options are: ```"Default", "Weapons", "Propulsion", "Power Systems"```.  Attempting to set the targeting option to any other string value will cause an exception.

```
#Returns the vector of the target's location
Print "Target Location: " + "My TurretController" target

#Check if turret has a target
if "My TurretController" target is true
  Print "Target Acquired!"

#Check if turret is targeting weapons
if "My TurretController" is targeting "Weapons"
  print "Targeting Enemy Weapons"

#Set targeting option to weapons
tell "My TurretController" to target "Weapons"
```

## "Angle" Property
* Primitive Type: Numeric
* Keywords: ```angle, angles, azimuth, azimuths```

Gets/Sets the turret's deviation angle, in degrees.  The turret will only shoot if the target's direction vector is within the deviation angle of the turret's shooting direction.

```
print "Turret Deviation Angle: " + "My TurretController" angle

#Set Deviation Angle to 5 degrees
set "My TurretController" angle to 5
```


## "Shoot" Property
* Primitive Type: Bool
* Keywords: ```shoot, shooting, fire, firing, trigger, triggered```

Gets/Sets whether the turret is shooting.  If any gun on the custom turret is shooting, this property will return true.  When set, this property tell the all guns associated with the custom turret controller to continuously fire (vs shooting once).


```
if "My TurretController" is shooting
  Print "Drop some lead!"

#Start Shooting
tell "My TurretController" to shoot

#Stop Shooting
tell "My TurretController" to stop shooting
```

## "Velocity" Property
* Primitive Type: Numeric
* Supported Directions: Up/Down/Left/Right

This property will set the custom turret's azimuth and elevation velocity multipliers.  If Left or Right direction is passed, or if no direction is passed, then Gets/Sets the Azimuth Velocity Multiplier.

If Up or Down direction is passed, then Gets/Sets the Elevation Velocity Multiplier.

```
print "Azimuth Velocity Multiplier: " + "My TurretController" right velocity

#Set Azimuth Velocity Multiplier to 10
set "My TurretController" right velocity to 10

print "Elevation Velocity Multiplier: " + "My TurretController" upwards velocity

#Set Elevation Velocity Multiplier to 10
set "My TurretController" upwards velocity to 10
```


