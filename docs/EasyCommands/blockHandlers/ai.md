# AI Blocks

AI handlers let EasyCommands coordinate AI blocks configured in the game terminal.
They inherit functional block properties, including power,
and the shared [terminal properties](terminal).

| Selector | Additional properties |
| --- | --- |
| `ai flight` / `aiflight` | `behavior`, `speed`, `precision`, `avoidance` |
| `ai basic` / `aibasic` | `behavior` |
| `ai recorder` / `airecorder` | `behavior`, `play` / `playing` |
| `ai defensive` / `aidefensive` | `behavior`, `flee` / `fleeing` |
| `ai offensive` / `aioffensive` | `behavior` |

`behavior` (also `behaviour`, `auto`, or `autopilot`) reads/writes the AI Behavior
switch. It is separate from `power`: powering a block does not necessarily activate
its AI behavior. Reading behavior reports the switch, not successful movement or
mission completion.

Flight `speed` reads/writes the configured speed limit in metres per second, not
the grid's current speed. `precision` and `avoidance` read/write the precision-mode
and collision-avoidance switches. Speed increments default to 1 m/s.

Recorder `play` starts playback when true and stops playback when false. Reading
`playing` returns playback state. A false result alone does not prove arrival.
Configure the recorded route and its reference beacon in the game terminal.

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
set "Return Route" ai recorder play to true

print "Return Route" ai recorder playing

# Stop this route before switching to a different configured task
set "Return Route" ai recorder play to false
set "Drone" ai defensive behavior to true
set "Drone" ai defensive flee to true
```

Use collision avoidance appropriate to the route. Route arrival, docking and cargo
conditions can be checked through the existing connector, position and inventory
handlers. These AI handlers do not configure missions, routes or attack patterns.
