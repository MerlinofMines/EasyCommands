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

```
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
