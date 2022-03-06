# Hinge Block Handler

* Block Type Keywords: ```hinge```
* Block Type Group Keywords: ```hinges```

Default Primitive Properties:
* Numeric - Angle

Default Directional Properties
* Up - Height
* Down - Height
* Clockwise - Angle
* Counterclockwise - Angle

## "Enabled" Property
* Primitive Type: Bool
* Keywords: ```enable, enabled```
* Inverse Keywords: ```disable, disabled```

Enables or Disables the given block

```
#Enable Block
enable "My Hinge"
set "My Hinge" to enabled
turn on "My Hinge"

#Disable Block
disable "My Hinge"
set "My Hinge" to disabled
turn off "My Hinge"
```

## "Power" Property
* Primitive Type: Bool
* Keywords: ```power, powered```

Turns on or off power to the block.  Effectively the same as the Enabled property.

```
#Turn on
turn on power to "My Hinge"
power on "My Hinge"

#Turn off
turn off "My Hinge"
power off "My Hinge"
```

## "Angle" Property
* Primitive Type: Numeric
* Takes an optional Direction (Clockwise/Counterclockwise)
* Keywords: ```angle, angles, azimuth, azimuths```

Gets or sets the hinge's angle.  When setting, takes an optional direction (Clockwise, Counterclockwise) indicating the direction to move the hinge to get to the desired angle.

Setting the hinge's angle is done by calculating the appropriate min/max angle, and then setting the hinge's velocity to +/- velocity to move the hinge to the desired angle.  If no direction is passed, the hinge uses the shortest angular distance to determine which direction to rotate.

This property can also be moved in order to to rotate the hinge clockwise or counterclockwise without specifying the limits.

### Supported Directions
* Clockwise - Rotate Clockwise
* Counterclockwise - Rotate Counterclockwise

```
#Get the hinge's angle
Print "Hinge Angle: " + "My Hinge" angle

#Set the hinge's angle to 30 taking the shortest path
rotate "My Hinge" angle clockwise to 30

#Set the hinge's angle clockwise to 30
rotate "My Hinge" angle clockwise to 30

#Rotate the hinge counterclockwise to 30
rotate "My Hinge" angle counterclockwise to 30

#Rotate the hinge clockwise
rotate "My Hinge" clockwise

#Rotate the hinge counterclockwise
rotate "My Hinge" counterclockwise
```

## "Velocity" Property
* Primitive Type: Numeric
* Keywords: ```velocity, velocities, speed, speeds, rate, rates```

Get/Sets the Hinges's angular velocity, in RPM.

```
#Get the Hinge Velocity
Print "Current Velocity: " + "My Hinge" velocity

#Set the Hinge Velocity
set "My Hinge" velocity to 5
```

## "Limit" Property
* Primitive Type: Numeric
* Takes an optional direction
* Keywords: ```limit, limits, range, ranges, distance, distances```

Get/Sets the Piston's upper/lower limits, in degrees.  Takes in an optional direction to indicate whether you are getting or setting the upper or lower limit.  If no direction is specified, the upper limit is used.

### Supported Directions
Up - Upper Limit
Down - Lower Limit
Forward - Forward Limit
Backwards - Lower Limit
Clockwise - Upper Limit
Counterclockwise - Lower Limit

```
#Get the upper Limit
Print "Upper Limit: " + "My Hinge" upper limit

#Get the lower limit
Print "Lower Limit: " + "My Hinge" lower limit

#set the upper limit
set "My Hinge" upper limit to 6

#Set the lower limit
set "My Hinge" lower limit to 4
```

## "Height" Property
* Primitive Type: Numeric
* Keywords: ```height, heights, level, levels```

Gets/Sets the Hinge's displacement, in meters.

```
#Get the Hinge Displacement
Print "Hinge Height: " + "My Hinge" height

#Set the Hinge Displacement
set "My Hinge" height to 0.4
```

## "Lock" Property
* Primitive Type: Bool
* Keywords: ```lock, locked```
* Inverse Keywords: ```unlock, unlocked```

Gets/Sets whether the hinge is locked, meaning whether it will move or not.

```
#Get whether hinge is locked
Print "Hinge Locked: " + "My Hinge" is locked

#Lock/Unlock
lock "My Hinge"
unlock "My Hinge"
```

## "Connected" Property
* Primitive Type: Bool
* Keywords: ```connect, connected, attach, attached```
* Inverse Keywords: ```disconnect, disconnected, detach, detached```

Gets/Sets whether the hinge's top part is attached.

```
if "My Hinge" is detached
  Print "I guess I blew my top..."

#Attach/Detach hinge head
attach "My Hinge"
detach "My Hinge"
```

## "Strength" Property
* Primitive Type: Numeric
* Keywords: ```strength, strengths, torque, torques, force, forces```

Gets/Sets the hinge's torque, in Nm

```
Print "Current Torque: " + "My Hinge" torque

#Set torque to 33MNm
set "My Hinge" torque to 33000000
```