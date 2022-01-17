# Laser Antenna Block Handler
This Block Handler handles laser antennas. There are separate block handlers for [Beacons](https://spaceengineers.merlinofmines.com/EasyCommands/blockHandlers/beacon "Beacons") and [Antennas](https://spaceengineers.merlinofmines.com/EasyCommands/blockHandlers/antenna "Antenna Block Handler").

* Block Type Keywords: ```laser, laserAntenna```
* Block Type Group Keywords: ```lasers, laserAntennas```

Take special caution when trying to select Laser Antennas.  It's easy to misuse "Laser Antenna" as the selector will be "Antenna" unless you explicitly specify the block type:

```
#Interpreted as an Antenna
turn on "My Laser Antenna"
turn on "My Laser" antenna
turn on "My Laser Antenna" antenna

#Interpreted as a Laser Antenna
turn on "My Laser"
turn on "My LaserAntenna"
turn on "My Antenna Laser"
turn on "My Laser Antenna" laser
turn on "My Laser Antenna" laserAntenna
```

Default Primitive Properties:
* Vector - Target

## "Enabled" Property
* Primitive Type: Bool
* Keywords: ```enable, enabled```
* Inverse Keywords: ```disable, disabled```

Enables or Disables the given block

```
#Enable Block
enable "My Laser"
set "My Laser" to enabled
turn on "My Laser"

#Disable Block
disable "My Laser"
set "My Laser" to disabled
turn off "My Laser"
```

## "Power" Property
* Primitive Type: Bool
* Keywords: ```power, powered```

Turns on or off power to the block.  Effectively the same as the Enabled property.

```
#Turn on
turn on power to "My Laser"
power on "My Laser"

#Turn off
turn off "My Laser"
power off "My Laser"
```

## "Target" Property
* Primitive Type: Vector
* Keywords: ```target, targets, coords, coordinates```

Gets or Sets the Target Coordinates for the Laser Antenna, i.e. where to point the laser antenna in order to communicate with another laser antenna.

```
Print "Target Coords: " + "My Laser" coordinates

#Obviously not a real coordinate, but you get the idea.
set "My Laser" target to "12.3:45.6:78.9"
```

## "Range" Propertyw
* Primitive Type: Numeric
* Keywords: ```range, ranges, limit, limits, distance, dinstances, radius, radii```

Get/Sets the range of the laser antenna, in meters.  So 2000 = 2000m = 2km

```
Print "Range: " + "My Laser" range

set "My Laser" range to 3000
```

## "Locked" Property
* Primitive Type: Bool
* Keywords: ```lock, locked, permanent```
* Inverse Keywords: ```unlock, unlocked```

Get/Sets whether the laser is permanent, meaning the laser will automatically try to re-connect if/when a connection is lost.

```
if "My Laser" is locked
  Print "Connection is permanent"

lock "My Laser"
```

## "Connected" Property
* Primitive Type: Bool
* Keywords: ```connect, connected, attach, attached```
* Inverse Keywords: ```disconnect, disconnected, detach, detached```

Get/Sets whether the laser is currently connected.  When reading, will return whether the Laser's status is Connected.  When updating, will attempt to connect to the laser antenna's target coordinates if the passed in Bool is true.

```
if "My Laser" is connected
  Print "Connection established"

#Attempt to connect to target coordinates.
connect "My Laser"
```