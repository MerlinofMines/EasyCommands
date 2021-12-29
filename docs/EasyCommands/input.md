# Handling User Input

One of the cool things you can do with EasyCommands is control block behavior based on user input coming from the mouse or keyboard.

This allows you to build things like cranes, rotating user controlled turrets/cameras, user controlled walking machines, and more.

User Input can be fetched from any Cockpit Block.  When that cockpit is occupied, input can be retrieved based on the button(s) the user is pressing.

There are two types of input supported by EasyCommand: Movement and Rotation.  Each support 6 directions of freedom.  Mouse movement is also considered rotational input.

When getting user input, the value will be between 0 (no input) and 1 (max input).  When pressing on the keyboard, the value will be 0 - 1.

Finer grained control can retrieved when using a mouse or joystick.

Check out the [Crane-Like Drill Arm With Mouse and Keyboard](https://spaceengineers.merlinofmines.com/EasyCommands/examples/craneDrill "Crane-Like Drill Arm With Mouse and Keyboard") example to see handling user input in action.

## Handling Movement Input

Movement refers to WASDE + Space, corresponding to the 6 movement directions, respectively.

Keywords: ```input, user, pilot```

### Supported Directions
Assuming default controls:
* Up - W key
* Down - S key
* Left - A key
* Right - D key
* Up - Space key
* Down: C Key

## Handling Rotational Input

Rotation refers to the arrow keys + QE, corresponding to the 6 rotational directions, respectively.

Keywords: ```roll, rollInput, rotation```

### Supported Directions
* Up - Up Arrow / Mouse Down
* Down - Down Arrow / Mouse Down
* Left - Left Arrow / Mouse Left
* Right - Right Arrow / Mouse Right
* Clockwise - E key
* Counterclockwise - Q key

### Mouse Input

Mouse input is considered Rotation input for the Up/Down/Left/Right directions.

Unlike keyboard input, mouse input can return values between 0-1, based on the speed at which the mouse is moving.

This works great for controlling rotating turrets, etc where you have a rotor and a hinge.  By binding rotational input to these blocks, you can effectively

Here's a simple example that does just that:
```
:main
set sensitivity to 0.5
async call aimTurret

:aimTurret
set the "Turret Hinge" velocity to ("Turret Cockpit" up roll) * sensitivity
set the "Turret Rotor" velocity to ("Turret Cockpit" right roll) * sensitivity
replay
```