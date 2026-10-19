# AI Blocks

AI handlers let EasyCommands coordinate AI blocks configured in the game terminal.
They inherit functional block properties, including power,
and the shared [terminal properties](terminal).

| Selector | Additional properties |
| --- | --- |
| `ai flight` / `aiflight` | `behavior`, `speed`, `precision`, `avoidance`, `altitude`, `gravity alignment` |
| `ai basic` / `aibasic` | `behavior` |
| `ai recorder` / `airecorder` | `behavior`, `play` / `playing`, `repeat path` / `loop path` |
| `ai defensive` / `aidefensive` | `behavior`, `flee` / `fleeing`, `target`, `target position`, `target override` |
| `ai offensive` / `aioffensive` | `behavior`, `target` |

`behavior` (also `behaviour`, `auto`, or `autopilot`) reads/writes the AI Behavior
switch. It is separate from `power`: powering a block does not necessarily activate
its AI behavior. Reading behavior reports the switch, not successful movement or
mission completion.

Flight `speed` reads/writes the configured speed limit in metres per second, not
the grid's current speed. `precision` and `avoidance` read/write the precision-mode
and collision-avoidance switches. A blocked route may stop playback instead of
finding a detour; check position as well as playback state. Speed increments
default to 1 m/s.
`altitude` is the configured minimum altitude in metres, not measured altitude;
its default increment is 1 m. `gravity alignment` is a boolean for alignment to
planetary gravity. These reuse existing altitude, gravity and alignment words.
The terminal's maximum pitch and roll settings determine how far the ship may tilt
while gravity alignment is enabled.
Altitude uses the terminal setting so its readback excludes the grid-size
adjustment applied by the game's typed `MinimalAltitude` getter.

Recorder `play` starts playback when true and stops playback when false. Reading
`playing` returns playback state. A false result alone does not prove arrival.
Configure the recorded route and its reference beacon in the game terminal.
`repeat path` (also `loop path`) is a read/write boolean controlling route repeat,
independently of behavior and playback. The two-word spelling preserves EC's
existing standalone `repeat`/`loop` commands.

On both combat blocks, `target` is a read-only boolean indicating whether the
enemy-search component has found a target. It does not return an entity ID,
promise a target lock, or provide enemy coordinates. Writing it does nothing.

Defensive `target position` is a world-coordinate vector. Reading returns the
resolved retreat destination (or `0:0:0` when unavailable); writing sets a custom
retreat destination that takes priority over the configured GPS/beacon. It does
not change the selected flee mode: choose GPS/beacon retreat in the terminal.
`target override` reads whether a custom destination is set. Set it **false** to
clear the override and restore the terminal GPS/beacon choice; setting true does
nothing. Supply `target position` to create an override, including an origin target.

Defensive `flee` requests fleeing when set true; reading `fleeing` reports the current
flee state. The game requires a detected threat and an active movement AI, plus
a usable flee configuration; this is not an unconditional return-home command.
Setting it false does nothing: the PB API has no matching stop-flee
method. Disable its behavior to hand control back to another configured behavior.
Configure the retreat behavior/destination in the terminal.

```
set "Drone" ai flight power to true
set "Drone" ai flight behavior to true
set "Drone" ai flight speed to 10
set "Drone" ai flight precision to true
set "Drone" ai flight avoidance to false
set "Drone" ai flight altitude to 150
set "Drone" ai flight gravity alignment to true
set "Return Route" ai recorder repeat path to false
set "Return Route" ai recorder play to true

print "Return Route" ai recorder playing

# Stop this route before switching to a different configured task
set "Return Route" ai recorder play to false
set "Drone" ai defensive behavior to true
set "Drone" ai defensive target position to 100:200:300
set "Drone" ai defensive flee to true

print "Drone" ai defensive target
# Restore the GPS/beacon destination configured in the terminal
set "Drone" ai defensive target override to false
```

Use collision avoidance appropriate to the route. Route arrival, docking and cargo
conditions can be checked through the existing connector, position and inventory
handlers. Mission selection, waypoint editing and attack-pattern configuration
remain in the terminal.
