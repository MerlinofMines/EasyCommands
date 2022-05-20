# Sensor Block Handler
This Block Handles Sensors, which you can use to automatically do things when you detect entities.

* Block Type Keywords: ```sensor```
* Block Type Group Keywords: ```sensors```

Default Primitive Properties:
* Vector - Target

## "Enabled" Property
* Primitive Type: Bool
* Keywords: ```enable, enabled```
* Inverse Keywords: ```disable, disabled```

Enables or Disables the given block

```
#Enable Block
enable "My Sensor"
set "My Sensor" to enabled
turn on "My Sensor"

#Disable Block
disable "My Sensor"
set "My Sensor" to disabled
turn off "My Sensor"
```

## "Power" Property
* Primitive Type: Bool
* Keywords: ```power, powered```

Turns on or off power to the block.  Effectively the same as the Enabled property.

```
#Turn on
turn on power to "My Sensor"
power on "My Sensor"

#Turn off
turn off "My Sensor"
power off "My Sensor"
```

## "Trigger" Property
* Read-only
* Primitive Type: Bool
* Keywords: ```trigger, triggered, detect, detected, trip, tripped```

Returns whether the Sensor is currently detecting an entity.

```
if "My Sensor" is triggered
  Print "Gotcha!"
```

## "Silence" Property
* Primitive Type: Bool
* Keywords: ```silent, silence```

Gets/Sets whether the sensor will play an audible beep when an entity is detected.

```
if "My Sensor" is silent
  Print `I see you but you can't hear me`

silence "My Sensor"
```

## "Target" Property
* Read-only
* Primitive Type: Vector
* Keywords: ```target, targets```

Gets whether the sensor's current target.  When getting, will return either the manually set target, or the position of the targeted entity.  If the sensor has not detected anything, it will return 0:0:0.

```
Print "Target Location: " + "My Sensor" target
```

## "Target Velocity" Property
* Read-only
* Primitive Type: Vector
* Keywords: ```target velocity```

Gets the velocity of the sensor's last detected entity, in world coordinates.  If the sensor has no detected entity, it will return 0:0:0.

```
Print "Target Velocity: " + "My Sensor" target velocity
```