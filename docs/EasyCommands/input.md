# Handling User Input

One of the cool things you can do with EasyCommands is control block behavior based on user input coming from the mouse or keyboard.

This allows you to build things like cranes, rotating user controlled turrets/cameras, user controlled walking machines, and more.

User Input can be fetched from any Cockpit Block.  When that cockpit is occupied, input can be retrieved based on the button(s) the user is pressing.

There are two types of input supported by EasyCommand: Movement and Rotation.  Each support 3 directions of freedom.  Mouse input is also considered rotational input.

The values returned by mouse and keyboard input are dependent on the user's sensitivity settings (higher sensitivity means higher values).  With default sensitivity settings, when getting user input, the values will be between -1 (max negative input) and 1 (max positive input).  When pressing on the keyboard with default user sensitivity settings, the value will be -1, 0, or 1.

Finer grained control can retrieved when using a mouse or joystick.

Check out the [Crane-Like Drill Arm With Mouse and Keyboard](https://spaceengineers.merlinofmines.com/EasyCommands/examples/craneDrill "Crane-Like Drill Arm With Mouse and Keyboard") example to see handling user input in action.

## Handling Movement Input

Movement refers to WASDC + Space, corresponding to the 3 movement directions.

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

Rotation refers to the arrow keys + QE, corresponding to the 3 rotational directions.

Keywords: ```roll, rollInput, rotation```

### Supported Directions
* Up - Up Arrow / Mouse Up
* Down - Down Arrow / Mouse Down
* Left - Left Arrow / Mouse Left
* Right - Right Arrow / Mouse Right
* Clockwise - E key
* Counterclockwise - Q key

### Mouse Input

Mouse input is considered Rotation input for the Up/Down/Left/Right directions.

Unlike keyboard input, mouse input can return variable values based on the speed at which the mouse is moving.

This works great for controlling rotating turrets, etc where you have a rotor and a hinge.  By binding rotational input to these blocks, you can effectively create user controllable turrets that you built yourself using rotors, hinges and guns.

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

### Quantizing Input

Since input values can vary based on the speed of movement and sensitivity settings, it's difficult to pin down input values to a specific range.

However, you can quantize the input using the "Sign" operation.  This will let you map any input values to either -1, 0, or 1.

```
set forwardsInput to the sign of "My Cockpit" forwards input
if forwardsInput is 1
  print "W Key Pressed"
else if forwardsInput is -1
  print "S Key Pressed"
else
  print "Neither Key Pressed"
replay
```
