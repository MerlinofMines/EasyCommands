# Remote Control Block Handler
This Block Handler handles Remote Controls.  It is an extension of the [Cockpit Handler](https://spaceengineers.merlinofmines.com/EasyCommands/blockHandlers/cockpit "Cockpit Handler"), so all properties from that handler also work for Remote Control Blocks, except those that are overridden.

Remote Control Blocks can be told where to go and how fast to go there, which makes them useful for remotely guiding ships using EasyCommands.

* Block Type Keywords: ```remote, drone, robot```
* Block Type Group Keywords: ```remotes, drones, robots```

Default Primitive Properties:
* Bool: Auto (overrides Enabled)
* Numeric: Velocity
* Vector: Waypoint
* List: Waypoints

Default Directional Properties
* Up: Velocity

## "Enabled" Property
* Primitive Type: Bool
* Keywords: ```enable, enabled```
* Inverse Keywords: ```disable, disabled```

Gets/Sets whether the given remote control(s) are considered the Main Remote Control (meaning can control the ship).

```
#Enable Block
enable "My Remote Control"
set "My Remote Control" to enabled

#Disable Block
disable "My Remote Control"
set "My Remote Control" to disabled
```

## "Power" Property
* Primitive Type: Bool
* Keywords: ```power, powered```

Effectively the same as "Enabled"

```
#Enable
turn on power to "My Remote Control"
power on "My Remote Control"

#Disable
turn off power to "My Remote Control"
power off "My Remote Control"
```

## "Auto" Property
* Primitive Type: Bool
* Keywords: ```auto, autopilot```

Gets/Sets whether autopilot is enabled.  When enabled, Autopilot will begin navigating to its specified waypoint(s) according to it's flight mode.

```
if "My Remote Control" is on autopilot
  Print "Ship is driving itself"

turn on "My Remote Control" autopilot
set "My Remote Control" to auto
```

This is the default boolean property, so you can also use the following:

```
#Turn on Autopilot
turn on "My Remote Control"
start "My Remote Control"
resume "My Remote Control"
tell "My Remote Control" to begin

#Turn off Autopilot
turn off "My Remote Control"
stop "My Remote Control"
tell "My Remote Control" to stop
halt "My Remote Control"
```

## "Run" Property
* Primitive Type: Bool
* Keywords: ```run, running, execute, executing```

Same as Auto Property. Get/Sets whether autopilot is enabled.

```
if "My Remote Control" is running
  Print "Ship is driving itself"

run "My Remote Control"
```

## "Velocity" Property
* Primitive Type: Numeric
* Takes an optional direction attribute (Read-only)
* Keywords: ```velocity, velocities, speed, speeds, rate, rates, pace, paces```

This property is the same as "Velocity" for the [Cockpit Handler](https://spaceengineers.merlinofmines.com/EasyCommands/blockHandlers/cockpit "Cockpit Handler"), but also allows you to set it
to a numeric value, which effectively sets the speed limit for the Auto Pilot.

Directions are not supported when specifying the velocity.

```
Print "Speed: " + "My Remote Control" speed

#Set speed limit to 50 m/s
set "My Remote Control" speed to 50
```

## "Limit" Property
* Primitive Type: Numeric
* Keywords: ```limit, limits```

Gets/Sets the speed limit for the Remote Control's autopilot

```
Print "Speed Limit: " + "My Remote Control" limit

#Set speed limit to 50 m/s
set "My Remote Control" limit to 50
```

## "Docking" Property
* Primitive Type: Bool
* Keywords: ```dock, docking```
* Inverse Keywords: ```undock, undocking```

Gets/Sets whether the Remote Control's autopilot is in Docking mode.  When in Docking Mode, the Remote Control will not rotate in order to proceed to it's next waypoint and only use movement.  Useful when you need a ship in a specific orientation in order to land but are trying to remotely guide it.

Note that this feature isn't very well documented or exposed in the Space Engineers UI, so no promises that it will work correctly.

```
if "My Remote Control" docking is on
  Print "Preparing to Dock!"

set "My Remote Control" docking to true
```

## "Waypoint" Property
* Primitive Type: Vector
* Keywords: ```waypoint, target, destination, coords, coordinates```

Gets/Sets the AutoPilot's current waypoint, represented as a Vector in World Coordinates.  When set, this property will clear all existing waypoints and add the given waypoint as the first waypoint.

Using a [Keyed Variable](https://spaceengineers.merlinofmines.com/EasyCommands/blockHandlers/collections "Collections"), you can optionally specify the name of the new waypoint.  The default name is "Waypoint 1"

```
Print "Current Waypoint: " + "My Remote Control" waypoint

#Obviously a fake destination
set "My Remote Control" waypoint to 0:1:2

#Named Waypoint
set "My Remote Control" waypoint to "My Destination" -> 0:1:2
```

## "Waypoints" Property
* Primitive Type: List
* Keywords: ```waypoints, destinations```

Gets/Sets the AutoPilot's current waypoints, where each item in the list represents a Vector in World Coordinates.  When set, this property will replace all existing waypoints with the given waypoints.

When retrieving, the waypoints returned will be in the order specified according to the auto-pilot's configuration.  It does not filter or order them based on the "current waypoint", etc.  The returned list will be keyed by the Waypoint names.

Using a [Keyed Variable](https://spaceengineers.merlinofmines.com/EasyCommands/blockHandlers/collections "Collections"), you can optionally specify the names of the new waypoints.  The default names are "Waypoint 1" .. "Waypoint x", where X is the size of the list.

```
#Current Waypoints: ["Waypoint 1"->1:2:3,"Waypoint 2"->4:5:6]
Print "Current Waypoints: " + "My Remote Control" waypoints

#Obviously a fake destination
set "My Remote Control" waypoints to [0:1:2, 4:5:6]

#Named Waypoint
set "My Remote Control" waypoints to ["The Mine" -> 0:1:2, "The Base" -> 4:5:6]
```