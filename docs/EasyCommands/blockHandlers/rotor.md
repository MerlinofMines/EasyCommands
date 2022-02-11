# Rotor Block Handler

* Block Type Keywords: ```rotor```
* Block Type Group Keywords: ```rotors```

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
enable "My Rotor"
set "My Rotor" to enabled
turn on "My Rotor"

#Disable Block
disable "My Rotor"
set "My Rotor" to disabled
turn off "My Rotor"
```

## "Power" Property
* Primitive Type: Bool
* Keywords: ```power, powered```

Turns on or off power to the block.  Effectively the same as the Enabled property.

```
#Turn on
turn on power to "My Rotor"
power on "My Rotor"

#Turn off
turn off "My Rotor"
power off "My Rotor"
```

## "Angle" Property
* Primitive Type: Numeric
* Takes an optional Direction (Clockwise/Counterclockwise)
* Keywords: ```angle, angles, azimuth, azimuths```

Gets or sets the rotor's angle.  When setting, takes an optional direction (Clockwise, Counterclockwise) indicating the direction to move the rotor to get to the desired angle.

Setting the rotor's angle is done by calculating the appropriate min/max angle, and then setting the rotor's velocity to +/- velocity to move the rotor to the desired angle.  If no direction is passed, the rotor uses the shortest angular distance to determine which direction to rotate.

This property can also be moved in order to to rotate the rotor clockwise or counterclockwise without specifying the limits.

### Supported Directions
Clockwise - Rotate Clockwise
Counterclockwise - Rotate Counterclockwise

```
#Get the rotor's angle
Print "Rotor Angle: " + "My Rotor" angle

#Set the rotor's angle to 30 taking the shortest path
rotate "My Rotor" angle clockwise to 30

#Set the rotor's angle clockwise to 30
rotate "My Rotor" angle clockwise to 30

#Rotate the rotor counterclockwise to 30
rotate "My Rotor" angle counterclockwise to 30

#Rotate the rotor clockwise
rotate "My Rotor" clockwise

#Rotate the rotor counterclockwise
rotate "My Rotor" counterclockwise
```

## "Velocity" Property
* Primitive Type: Numeric
* Keywords: ```velocity, velocities, speed, speeds, rate, rates```

Get/Sets the Rotors's angular velocity, in RPM.

```
#Get the Rotor Velocity
Print "Current Velocity: " + "My Rotor" velocity

#Set the Rotor Velocity
set "My Rotor" velocity to 5
```

## "Limit" Property
* Primitive Type: Numeric
* Takes an optional direction
* Keywords: ```limit, limits, range, ranges, distance, distances```

Get/Sets the Piston's upper/lower limits, in degrees.  Takes in an optional direction to indicate whether you are getting or setting the upper or lower limit.  If no direction is specified, the upper limit is used.

### Supported Directions
* Up - Upper Limit
* Down - Lower Limit
* Forward - Forward Limit
* Backwards - Lower Limit
* Clockwise - Upper Limit
* Counterclockwise - Lower Limit

```
#Get the upper Limit
Print "Upper Limit: " + "My Rotor" upper limit

#Get the lower limit
Print "Lower Limit: " + "My Rotor" lower limit

#set the upper limit
set "My Rotor" upper limit to 6

#Set the lower limit
set "My Rotor" lower limit to 4
```

## "Height" Property
* Primitive Type: Numeric
* Keywords: ```height, heights, level, levels```

Gets/Sets the Rotor's displacement, in meters.

```
#Get the Rotor Displacement
Print "Rotor Height: " + "My Rotor" height

#Set the Rotor Displacement
set "My Rotor" height to 0.4
```

## "Lock" Property
* Primitive Type: Bool
* Keywords: ```lock, locked```
* Inverse Keywords: ```unlock, unlocked```

Gets/Sets whether the rotor is locked, meaning whether it will move or not.

```
#Get whether rotor is locked
Print "Rotor Locked: " + "My Rotor" is locked

#Lock/Unlock
lock "My Rotor"
unlock "My Rotor"
```

## "Connected" Property
* Primitive Type: Bool
* Keywords: ```connect, connected, attach, attached```
* Inverse Keywords: ```disconnect, disconnected, detach, detached```

Gets/Sets whether the rotor's top part is attached.

```
if "My Rotor" is detached
  Print "I guess I blew my top..."

#Attach/Detach rotor head
attach "My Rotor"
detach "My Rotor"
```

## "Strength" Property
* Primitive Type: Numeric
* Keywords: ```strength, strengths, torque, torques, force, forces```

Gets/Sets the rotor's torque, in Nm

```
Print "Current Torque: " + "My Rotor" torque

#Set torque to 33MNm
set "My Rotor" torque to 33000000
```