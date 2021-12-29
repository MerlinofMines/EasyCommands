# Piston Block Handler

This handler lets you control pistons, including setting the height, velocity, and limits.

* Block Type Keywords: ```piston```
* Block Type Group Keywords: ```pistons```

Default Primitive Properties:
* Bool - Enabled
* Numeric - Height

Default Directional Properties
* Up - Height
* Down - Height

## "Enabled" Property
* Primitive Type: Bool
* Keywords: ```enable, enabled```
* Inverse Keywords: ```disable, disabled```

Enables or Disables the given block

```
#Enable Block
enable "My Piston"
set "My Piston" to enabled
turn on "My Piston"

#Disable Block
disable "My Piston"
set "My Piston" to disabled
turn off "My Piston"
```

## "Power" Property
* Primitive Type: Bool
* Keywords: ```power, powered```

Turns on or off power to the block.  Effectively the same as the Enabled property.

```
#Turn on
turn on power to "My Piston"
power on "My Piston"

#Turn off
turn off "My Piston"
power off "My Piston"
```

## "Height" Property
* Primitive Type: Numeric
* Keywords: ```height, heights, limit, limits, length, lengths```

When retrieving, this property will return the current position of the piston.

When set, this property will set the height of the piston directly.  Internally, this is done by setting the maximum or minimum height of the piston to the desired value, and then setting the velocity to either +/- to move the piston to the desired height.  If the piston length is currently less than the desired height, the max limit is set as the desired height and velocity set to +velocity.  If the piston length is farther than the desired height, the minimum limit is set as the desired height and the velocity is set to -velocity.

Setting this property will not change the velocity of the piston directly.  So if the velocity of the piston is 0, setting this property will not work correctly.


```
#Get the Current Height
Print "Current Height: " + "My Piston" height

#Set the Height
set "My Piston" height to 5
```

## "Velocity" Property
* Primitive Type: Numeric
* Keywords: ```velocity, velocities, speed, speeds, rate, rates```

Get/Sets the Piston's velocity, in m/s.

```
#Get the Piston Velocity
Print "Current Velocity: " + "My Piston" velocity

#Set the Piston Velocity
set "My Piston" velocity to 5
```

## "Limit" Property
* Primitive Type: Numeric
* Takes an optional direction (Upper/Lower)
* Keywords: ```limit, limits, range, ranges, distance, distances```

Get/Sets the Piston's upper/lower limits.  Takes in an optional direction to indicate whether you are getting or setting the upper or lower limit.  If no direction is specified, the upper limit is used.

This property can also be moved and reversed.  If this property is moved upwards, the piston is extended.  If the property is moved downward, the piston is retracted.

### Supported Directions
Up - Upper Limit
Down - Lower Limit
Forward - Forward Limit
Backwards - Lower Limit

```
#Get the upper Limit
Print "Upper Limit: " + "My Piston" upper limit

#Get the lower limit
Print "Lower Limit: " + "My Piston" lower limit

#set the upper limit
set "My Piston" upper limit to 6

#Set the lower limit
set "My Piston" lower limit to 4

#Extend the Piston
extend "My Piston"

#Retract the piston
retract "My Piston"

#Reverse the piston
reverse "My Piston"
```