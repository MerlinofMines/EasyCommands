# Cockpit Block Handler
This BlockHandler can support any block which allows you to control ships.  This includes cockpits, control seats, control stations, and even cryo chambers.
It can also be used to interact with Remote Control objects, though you should consider using the [Remote Control Handler](https://spaceengineers.merlinofmines.com/EasyCommands/blockHandlers/remote "Remote Control Handler") instead, as Remote Control inherits all of this BlockHandlers properties and also has some more properties you can modify.

This handler enables you to listen for user input, which has all kinds of potential for things like factories, cranes, drill arms, manned turrets, tanks...

This Block Handler will only briefly touch on handling user input.  For much more information, see [Handling User Input](https://spaceengineers.merlinofmines.com/EasyCommands/input "Handling User Input")

* Block Type Keywords: ```ship, rover, cockpit, seat, station```
* Block Type Group Keywords: ```ships, rovers, cockpits, seats, stations```

Default Primitive Properties:
* Boolean - Enabled
* Numeric - Velocity

Default Directional Properties
* Up - Velocity 

## "Enabled" Property
* Primitive Type: Bool
* Keywords: ```enable, enabled```
* Inverse Keywords: ```disable, disabled```

Gets/Sets whether the given cockpit(s) are considered the Main Cockpit (meaning can control the ship).

```
#Enable Block
enable "My Cockpit"
set "My Cockpit" to enabled
turn on "My Cockpit"

#Disable Block
disable "My Cockpit"
set "My Cockpit" to disabled
turn off "My Cockpit"
```

## "Power" Property
* Primitive Type: Bool
* Keywords: ```power, powered```

Effectively the same as the Enabled property.

```
#Turn on
turn on power to "My Cockpit"
power on "My Cockpit"

#Turn off
turn off power to "My Cockpit"
power off "My Cockpit"
```

## "Override" Property
* Primitive Type: Bool
* Keywords: ```dampener, dampeners, override, overrides, overriden```

Gets/Sets whether the dampeners are currently on (meaning overriding)

```
turn on "My Cockpit" dampeners
set "My Cockpit" overrides to true
override "My Cockpit"

"My Cockpit" dampeners off
```

## "Occupied" Property
* Read-only
* Primitive Type: Bool
* Keywords: ```use, used, occupy, occupied, control, controlled```
* Inverse Keywords: ```unused, unoccupied, available```

Returns whether the Cockpit is currently occupied.

```
Print "Occupied: " + "My Cockpit" is occupied
Print "In Use: " + "My Cockpit" is in use
Print "Controlled: " + "My Cockpit" is being controlled
```

## "Auto" Property
* Primitive Type: Bool
* Keywords: ```auto, cooperate, cooperating```
* Inverse Keywords: ``` , ```

Identical to Override property.  Gets/Sets whether the dampeners are currently on.

```
set "My Cockpit" to auto
#My personal favorite
tell "My Cockpit" to cooperate
```

```
if "My Cockpit" is cooperating
disable "My Cockpit"
Print "Time to chill out and enjoy the ride"
```

## "Lock" Property
* Primitive Type: Bool
* Keywords: ```lock, locked, freeze, frozen, brake, handbrake```
* Inverse Keywords: ```unlock, unlocked, unfreeze```

Gets/Sets the Handbrake.  This is only useful for Rovers, specifically when wheels are present.

```
#Whoa nelly!
tell "My Cockpit" to brake
set the "Rover Cockpit" handbrake
lock "My Cockpit"

turn off the "Rover Cockpit" handbrake
unlock "My Cockpit"
```

## "Height" Property
* Read-only
* Primitive Type: Numeric
* Keywords: ```height, heights, level, levels```

Returns the parachute's current height above the ground, in meters.  If not in a gravity well, will return -1.

```
Print "Cockpit Height: " + "My Cockpit" height
```

## "Altitude" Property
* Read-only
* Primitive Type: Numeric
* Keywords: ```altitude, altitudes, elevation, elevations```

Returns the parachute's current elevation above sea level, in meters.  If not in a gravity well, will return -1.

```
Print "Cockpit Altitude: " + "My Cockpit" altitude
```

## "Gravity" Property
* Read-only
* Primitive Type: Vector
* Keywords: ```gravity, gravities, strength, force```

Gets the current Total Gravity force on the ship as a Vector, in World Coordinates.  Note that this includes planet AND artificial gravity combined.

Use the "abs" [Operation](https://spaceengineers.merlinofmines.com/EasyCommands/operations "Operations") to get the Gravity Strength.

```
Print "Gravity: " + "My Cockpit" gravity
Print "Gravity Strength: " + abs "My Cockpit" gravity
```

## "NaturalGravity" Property
* Read-only
* Primitive Type: Vector
* Keywords: ```naturalGravity, naturalGravities, planetGravity, planetGravities```

Gets the current Natural (i.e., Planet's) Gravity force on the ship as a Vector, in World Coordinates. 

Returns 0:0:0 if not within a planet's gravity well.

Use the "abs" [Operation](https://spaceengineers.merlinofmines.com/EasyCommands/operations "Operations") to get the Natural Gravity Strength.

```
Print "Natural Gravity: " + "My Cockpit" naturalGravity
Print "Natural Gravity Strength: " + abs "My Cockpit" naturalGravity
```

## "ArtificialGravity" Property
* Read-only
* Primitive Type: Vector
* Keywords: ```artificialGravity, artificialGravities```

Gets the current Artificial Gravity force on the ship as a Vector, in World Coordinates.

Returns 0:0:0 if no artifical gravity is currently active.

Use the "abs" [Operation](https://spaceengineers.merlinofmines.com/EasyCommands/operations "Operations") to get the Artificial Gravity Strength.

```
Print "Artificial Gravity: " + "My Cockpit" artificialGravity
Print "Artificial Gravity Strength: " + abs "My Cockpit" artificialGravity
```

## "Weight" Property
* Read-only
* Primitive Type: Numeric
* Keywords: ```weight, mass, level```

Returns the current weight of the ship, in Kg.  This includes the weight of the ship and the weight of all onboard cargo.

```
Print "Ship Total Mass: " + "My Cockpit" mass
```

## "Ratio" Property
* Read-only
* Primitive Type: Bool
* Keywords: ```ratio, ratios, percentage, percentages, percent, percents```

Gets the current Oxygen Ratio of the cockpit, as a value from 0 - 1 where 1 = 100%.

```
Print "Oxygen Ratio: " + "My Cockpit" ratio
```

## "Capacity" Property
* Read-only
* Primitive Type: Numeric
* Keywords: ```limit, limits, capacity, capacities```

Gets the Maximum Oxygen Capacity of the cockpit, in L.  

```
Print "Oxygen Capacity: " + "My Cockpit" capacity
```

## "Velocity" Property
* Read-only
* Primitive Type: Numeric
* Takes an optional direction attribute
* Keywords: ```velocity, velocities, speed, speeds, rate, rates, pace, paces```

Returns the current velocity of the ship, in m/s.  If no direction is specified, returns the magnitude of the ship's velocity.

If a direction is included, then returns the velocity of the ship with respect to the given direction, with respect to the cockpit's orientation.

Supported Directions:
* Up, Down, Left, Right, Forward, Backward

```
Print "Ship Velocity: " + "My Cockpit" velocity

if "My Cockpit" upwards velocity > 0
  Print "Drifting upwards"
```

## "Input" Property
* Read-only
* Primitive Type: Vector/Numeric
* Takes an optional Direction attribute
* Keywords: ```input, inputs, user, users, pilot, pilots```

If a direction is not included, Returns a Vector (Right:Up:Forwards) representing the magnitude of movement input (values between 0 - 1) from the engineer sitting in the cockpit.

If a direction is included, then returns a value between 0 - 1 representing the user movement input in the specified direction.
Specifically, this input is mapped to WASDC + Space for the 6 input directions (not including rotation).

Supported Directions:
* Up, Down, Left, Right, Forward, Backward

See [Handling User Input](https://spaceengineers.merlinofmines.com/EasyCommands/input "Handling User Input") for more information.

```
if "My Cockpit" input > 0
  Print "Someone is messing with my ship!"

if "My Cockpit" left input > 0
  Print "The boss says move to the left"
```

## "Rotation" Property
* Read-only
* Primitive Type: Vector/Numeric
* Takes an optional Direction attribute
* Keywords: ```roll, rolls, rollInput, rollInputs, rotation, rotations```

If a direction is not included, Returns a Vector (Up:Right:Clockwise) representing the magnitude of rotation input (values between 0 - 1) from the engineer sitting in the cockpit.

If a direction is included, then returns a value between 0 - 1 representing the user roll input in the specified direction.
Specifically, this input is mapped to WASDC + Space for the 6 input directions (not including rotation).

Also note that mouse input is considered rotational input (Left/Right/Up/Down), so this property allows you to control rotation based on mouse input.

Supported Directions:
* Up, Down, Left, Right, Clockwise, CounterClockwise

See [Handling User Input](https://spaceengineers.merlinofmines.com/EasyCommands/input "Handling User Input") for more information.

```
if "My Cockpit" rotation > 0
  Print "Someone is trying to rotate my ship!"

if "My Cockpit" left rotation > 0
  Print "I guess the boss wants to look left"
```

## Examples

Here's a full example to print out user input to the Cockpit display:

```
:main
set myCockpit to "My Cockpit"

async updateDisplay
:updateDisplay
set $myCockpit display text to ""
checkMovement
checkRotation
replay

:checkMovement
if the cockpit upwards input > 0
  setDisplay "Moving Up"
else if the cockpit downwards input > 0
  setDisplay "Moving Down"
else if the cockpit left input > 0
  setDisplay "Moving Left"
else if the cockpit right input > 0
  setDisplay "Moving Right"
else if the cockpit forward input > 0
  setDisplay "Moving Forward"
else if the cockpit backward input > 0
  setDisplay "Moving Backwards"
else
  setDisplay "Movement: " + the cockpit input

:checkRotation
if the cockpit upwards roll > 0
  setDisplay "Rolling Up"
else if the cockpit downwards roll > 0
  setDisplay "Rolling Down"
else if the cockpit left roll > 0
  setDisplay "Rolling Left"
else if the cockpit right roll > 0
  setDisplay "Rolling Right"
else if the cockpit clockwise roll > 0
  setDisplay "Rolling Clockwise"
else if the cockpit counter roll > 0
  setDisplay "Rolling Counter Clockwise"
else
  setDisplay "Roll Input: " + the cockpit roll

:setDisplay myText
increase the $myCockpit display text by myText + "\n"
```