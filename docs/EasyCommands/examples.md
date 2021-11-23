## Control a Crane-Like Drill Arm With Mouse and Keyboard

Imagine you want to build a ground-based vehicle with a crane-arm for drilling, and we want to be able to have a cockpit for the crane that lets the engineer rotate it, raise and lower the arm, and extend it. Something like this:

![Drillbert, the Ground Vehicle with a Crane Drill](https://i.imgur.com/Jms5v3r.gif)

Writing a script in EasyCommands to control the above is as easy as:

```
:main
set drillCockpit to "[DB] Drill Cockpit"
set rotationSensitivity to 0.25
async controlDrill

:controlDrill
set the "[DB] Drill Rotor" velocity to (rotationSensitivity * $drillCockpit right roll)
set the "[DB] Drill Middle Hinge" velocity to (rotationSensitivity * $drillCockpit downwards roll)
set the "[DB] Drill Bottom Piston" velocity to the $drillCockpit forwards input
set the "[DB] Drill Middle Pistons" velocity to the $drillCockpit clockwise roll
set the "[DB] Drill Front Hinge" velocity to the $drillCockpit upwards input
set the "[DB] Drill Front Piston" velocity to the $drillCockpit right input
replay
```

Let's break down how this script works:

```
:main
set drillCockpit to "[DB] Drill Cockpit"
set rotationSensitivity to 0.25
```

First, we set up our main function. It doesn't need to be called `main` - whatever the first function is will be considered the main one, and that'll be invoked by default by the Programmable Block.

This main function sets a variable for the drill cockpit name, as we use that many times later, and then also sets a variable for the rotation sensitivity - this is just a multiplier so that our drill arm doesn't move TOO fast.

```
async controlDrill
```

Then, the main function invokes controlDrill in its own thread. This allows controlDrill to do whatever it needs to do without blocking the main program.

:controlDrill
set the "[DB] Drill Rotor" velocity to (rotationSensitivity * $drillCockpit right roll)
set the "[DB] Drill Middle Hinge" velocity to (rotationSensitivity * $drillCockpit downwards roll)
set the "[DB] Drill Bottom Piston" velocity to the $drillCockpit forwards input
set the "[DB] Drill Middle Pistons" velocity to the $drillCockpit clockwise roll
set the "[DB] Drill Front Hinge" velocity to the $drillCockpit upwards input
set the "[DB] Drill Front Piston" velocity to the $drillCockpit right input
replay
```

The controlDrill function is constantly replaying itself, so its always (every tick) updating the velocity of our rotor, hinges, and pistons based on the user's input. Note us using `roll` to indicate the camera (e.g. mouse, or arrow keys), and `input` to indicate movement (e.g. WASD). A rough breakdown of what this means specifically:
* `right/left roll` - left and right on arrow keys or mouse
* `downwards/upwards roll` - up and down on arrow keys or mouse
* `clockwise roll` - q and e
* `forwards/backwards input` - w and s movement keys
* `upwards/downwards input` - c and space movement keys
* `right/left input` - a and d movement keys

We can obviously mix and match these however we like!

Note that the choice of which direction we specify (e.g. `forwards input` vs. `backwards input`) tells us the sign of the velocity. e.g. `forwards input` would mean that when we press `w`, we're increasinge velocity, and decreasing with `s`. If we specified `backwards input`, it'd behave in the reverse.

Enjoy your new drill arm!

For a full video tutorial, see here (TBD LINK).

## Automatic Brake and Reverse Lights & Beeper
This script will add some fun to your rover by adding brakes, reverse lights, and reverse beeper.  

```
:turnOn
turn on the "rover signal lights"
until "rover light program" is off
  async call "rearLights"
  async call "reverseSirens"

:rearLights
if "rover cockpit" is locked
  async turn on the "rear lights"
  set the "rear lights" color to "red"
else if "rover cockpit" backwards input > 0
  if "rover cockpit" forward velocity > 1
    async turn on the "rear lights"
    set the "rear lights" color to "red"
  else
    async turn on the "rear lights"
    set the "rear lights" color to "yellow"
else if "rover cockpit" forwards input > 0 and "rover cockpit" backwards velocity > 1
  async turn on the "rear lights"
  set the "rear lights" color to "red"  
else
  turn off the "rear lights"

:reverseSirens
if "rover cockpit" backwards velocity > 1
  if "reverse sirens" are off
    turn on the "reverse sirens"
else
  turn off the "reverse sirens"

:turnOff
turn off the "rover signal lights"
```


## Simple Airlock Manager using Sensor at External Door:
The following script controls a simpler airlock with 2 doors and a single sensor which detects wanting to move through the external door (to exit or enter):

```
:main
async call "runAirlock"
wait 3
restart

:runAirlock
if "Airlock Sensor" is triggered
  call "goOut"
else
  call "comeIn"

:comeIn
if "Airlock Exterior Door" is open
  close the "Airlock Exterior Door"
else if "Airlock Vent" is not pressurized
  pressurize the "Airlock Vent"
else if "Airlock Interior Door" is closed
  open the "Airlock Interior Door"

:goOut
if "Airlock Interior Door" is open
  close the "Airlock Interior Door"
else if "Airlock Vent" is pressurized
  depressurize the "Airlock Vent"
else if "Airlock Exterior Door" is closed
  open the "Airlock Exterior Door"
```

## Garage Door Opener
The following script can control 2 (or more if you want) doors for your garage or ship hanger by listening on the "garageDoors" channel.  From your vehicle, use the following command to open your door remotely!

Vehicle Opener Script:

```
:wait
wait 1 tick

:toggleDoor
send "goto toggleDoor" to "garageDoors"

:openDoor
send "goto opendoor" to "garageDoors"

:closeDoor
send "goto closedoor" to "garageDoors"
```

Garage Door Controller Script:

```
:setup
listen "garageDoors"
goto "listen"

:listen
wait 1
restart

:toggleDoor1
if "door1" doors are closed
  goto "openDoor1"
else
  goto "closeDoor1"
goto "listen"

:toggleDoor2
if "door2" doors are closed
  goto "openDoor2"
else
  goto "closeDoor2"
goto "listen"

:openDoor1
open "door1" doors
goto "listen"

:closeDoor1
close "door1" doors
goto "listen"

:openDoor2
open "door2" doors
goto "listen"

:closeDoor2
close "door2" doors
goto "listen"
```
