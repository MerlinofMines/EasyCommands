# Antenna Block Handler
This Block Handler handles broadcasting antennas. There are separate block handlers for [Beacons](https://spaceengineers.merlinofmines.com/EasyCommands/blockHandlers/beacon "Beacons") and [Laser Antennas](https://spaceengineers.merlinofmines.com/EasyCommands/blockHandlers/laserAntenna "Laser Antenna Block Handler").

* Block Type Keywords: ```antenna```
* Block Type Group Keywords: ```antennas```

Default Primitive Properties:
* String: Text (overrides "Name" from Terminal Block)
* Boolean: Connected (overrides "Enabled" from Terminal Block)

Default Directional Properties
* Up: Range

## "Text" Property
* Primitive Type: String
* Keywords: ```text, message```

Gets or sets the HudText for the antenna.

```
set the "Base Antenna" text to "The Bat Cave"
```

## "Connect" Property
* Primitive Type: Bool
* Keywords: ```connect, connected,  ```
* Inverse Keywords: ```disconnect, disconnected```

Enables or disables broadcasting, meaning whether the antenna can be seen from your HUD when in range.

```
#All of these Enable Broadcasting
tell the "Base Antenna" to connect
tell the "Base Antenna" to broadcast
turn on the "Base Antenna"

#Disables Broadcasting
disconnect the "Base Antenna"
tell the "Base Antenna" to not broadcast
turn off the "Base Antenna"
```

## "Broadcast" Property
* Primitive Type: Bool
* Keywords: ```broadcast, broadcasting```

Enables or disables broadcasting, meaning whether the antenna can be seen from your HUD when in range.

```
if the "Base Antenna" is broadcasting
  Print "Whoops! Didn't mean to tell everyone I was here"
  turn off the "Base Antenna"
```

## "Radius" Property
* Primitive Type: Numeric
* Keywords: ```radius, radii```

Get or sets the Antenna's Radius (meaning how far it broadcast).  Value is in meters, so 2000 = 2000m = 2km

```
Print "Antenna Radius: " + the "Base Antenna" radius
set the "Base Antenna" radius to 3000
```

## "Range" Property
* Primitive Type: Numeric
* Keywords: ```range, ranges, distance, distances, limit, limits```

Same as Radius. Get or sets the Antenna's Range (meaning how far it broadcast).  Value is in meters, so 2000 = 2000m = 2km

```
Print "Antenna Range: " + the "Base Antenna" range
set the "Base Antenna" range to 3000
```

## "Show" Property
* Primitive Type: Bool
* Keywords: ```show, showing```
* Inverse Keywords: ```hide, hiding```

Gets or sets whether the Antenna shows the Ship Name as part of the Hud Text.  Does not control whether the ship is broadcasting or not, and doesn't control the Hud Text.

```
if the "Base Antenna" is showing
  Print "The world knows my ship's name!"
  
#Hide
hide the "Base Antenna"
Print "You know I'm here, but you don't know my name!"
```

Note that this overrides the "show" property of Terminal Block.  So if you want to hide an antenna from the terminal menu, specify "terminal" as the block type:

```
#Hides "Base Antenna" from tne terminal menu
hide the "Base Antenna" terminal

#Hides the ship name from Hud Text
hide the "Base Antenna"
```

