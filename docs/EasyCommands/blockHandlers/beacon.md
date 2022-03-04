# Beacon Block Handler
This Block Handler governs beacons, and is pretty straightforward.  There are separate block handlers for [Antennas](https://spaceengineers.merlinofmines.com/EasyCommands/blockHandlers/antenna "Antenna Block Handler") and [Laser Antennas](https://spaceengineers.merlinofmines.com/EasyCommands/blockHandlers/laserAntenna "Laser Antenna Block Handler").

* Block Type Keywords: ```beacon```
* Block Type Group Keywords: ```beacons```

Default Primitive Properties:
* String - Text
* Numeric - Range

Default Directional Properties
* Up - Range

## "Enabled" Property
* Primitive Type: Bool
* Keywords: ```enable, enabled```
* Inverse Keywords: ```disable, disabled```

Enables or Disables the given block

```
#Enable Block
enable "My Beacon"
set "My Beacon" to enabled
turn on "My Beacon"

#Disable Block
disable "My Beacon"
set "My Beacon" to disabled
turn off "My Beacon"
```

## "Power" Property
* Primitive Type: Bool
* Keywords: ```power, powered```

Turns on or off power to the block.  Effectively the same as the Enabled property.

```
#Turn on
turn on power to "My Beacon"
power on "My Beacon"

#Turn off
turn off "My Beacon"
power off "My Beacon"
```

## "Broadcast" Property
* Primitive Type: Bool
* Keywords: ```broadcast, broadcasting```

Gets/Sets whether the beacon is broadcasting.  Effectively the same as the Enabled property.

```
if the "Base Beacon" is broadcasting
  Print "People can see me!"

tell the "Base Beacon" to broadcast
```

## "Text" Property
* Primitive Type: String
* Keywords: ```test, message```

Get/Sets the text broadcast by the beacon, which is what you see in your HUD.

```
Print "Current Message: " + the "Base Beacon" message

set the "Base Beacon" text to "Come and get me!"
```

## "Range" Property
* Primitive Type: Numeric
* Keywords: ```radius, radii```

Get/Sets the radius that the beacon broadcasts to, in meters.  So 2000 = 2000m = 2km

```
Print "Broadcasting Radius: " + the "Base Beacon" radius

set the "Base Beacon" radius to 3000
```
## "Range" Property
* Primitive Type: Numeric
* Keywords: ```range, ranges, limit, limits, distance, distances```

Same as Radius. Get/Sets the range that the beacon broadcasts to, in meters.  So 2000 = 2000m = 2km

```
Print "Broadcasting Range: " + the "Base Beacon" range

set the "Base Beacon" range to 3000
```