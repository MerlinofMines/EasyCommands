# Block Handlers

## Overview

Block Handlers consist of two concepts: A Block Type, and Block Properties.

There are block handlers for almost all block types in the game, giving you the ability to read from and control just about everything you can think of.  It's also possible to interact with blocks that don't have explicit BlockType keywords (see Universal Block Properties below).

### Block Types
Block Types define a mapping between specific keywords and a specific type of block to look for.  Block Types enable you to create [Selector's](https://spaceengineers.merlinofmines.com/EasyCommands/selectors "Selectors") based on specific block types, which also might have a specific name or be part of a specific group.  See Selectors for more information on accessing specific blocks withing a grid.

### Block Properties

Block Properties allow you to interact with properties of a given set of blocks, based on the [Selector's](https://spaceengineers.merlinofmines.com/EasyCommands/selectors "Selectors") block type.  Many blocks share the same properties, but the behavior is different for that block type.  For example, the "Range" property on a Gatling Turret will set the Gatling Turret's firing range, whereas "Range" on an antenna will set it's broadcasting distance.

## Getting and Setting Block Properties

Before I get into the supported Block Handlers, let's go over some basics of Block Properties.  These basics apply to all Block Handlers, so as you are reading through specific BlockHandlers, keep the behavior described below in mind.  The BlockHandler descriptions reference these concepts without explaining them in detail.

To indicate that you are attempting to read or update a Block Property, you need a Selector and a Property.  The typical format is ```selector property```.  

This format is used for reading from and updating properties.  It is also used for some [Variables](https://spaceengineers.merlinofmines.com/EasyCommands/variables "Variables") to create aggregations or conditions.

```
#Read a property value
Print 'Antenna Range: ' +  the "Base Antenna" range

#Update a property value
set the "Base Antenna" range to 2000

#Increase a property value
increase the "Base Antenna" range by 1000

#Decrease a property value
```

## Read only Properties
Not all properties can be updated.  These properties will be labeled "Read-only" and can only be retrieved.  Examples include be "Position", "Direction", "Properties", etc.

## Moving Properties

In addition to reading, updating and incrementing properties, you can also move some properties.  This is useful for things like extending pistons, rotating rotors, etc where you want to move a property in a direction but don't have a specific target value.

Moving a property requires a direction to supplied, and typically has the form ```action selector property direction```.  If property is omitted the default direction based property is used (see Default Properties below).

```
#Extends the piston's height
extend the "test piston"

#Moves the rotor's angle clockwise
rotate the "test rotor" clockwise
```

## Directional Properties

### Supported Directions

The below lists out supported directions.  Note that not all block handlers nor all block properties support all directions.  The supported directions for a given Block Property depend on the BlockHandler and will be listed next to the property for that block type.  For example, a piston has an "upper" and a "lower" limit.  An antenna has a limit.  The "Limit" property has directional support on a piston but not on an antenna.

* Up: ```up, upwards, upward, upper```
* Down: ```down, downward, downwards, lower```
* Left: ```left, lefthand```
* Right: ```right, righthand```
* Forward: ```forward, forwards, front```
* Backwards: ```backward, backwards, back```
* Clockwise: ```clockwise, clock```
* Counter Clockwise: ```counter, counterclock, counterclockwise```

There are also some words that indicate ```action direction```, such as:

Up: ```raise, extend```
Down: ```retract```

## Reversing Properties

Some properties can be reversed, including those that can be moved.  To reverse a given block property, specify the ```reverse``` keywords.

The general form is ```reverse selector property```.  If property is omitted, the BlockHandler's default property is used.

For most properties, reverse has the effect of inverting the property value.  For booleans, this toggles between on/off.  For numerics, vectors, this multiplies the property value by -1.  For colors, it inverts the color.  For Strings & Lists, the result reverses the item ("String" -> "gnirtS", [1,2,3] -> [3,2,1]).

There are a couple exceptions to this, namely pistons, rotors and hinges.  For these blocks, reverse causes the block to switch "directions", effectively by reversing the block's "velocity" attribute.

```
reverse the "Elevator Pistons"
reverse the "Elevator Pistons" velocity
tell the "Elevator Pistons" to reverse
reverse the "Crane Rotor"
reverse the "Crane Rotor" velocity
```

## Attribute Based Properties

Some properties have an attribute, meaning the block contains multiple kinds of the property and you need a specifier to indicate which.

For these properties, the general form is ```variable property``` where "variable" indicates the specifier for the attribute value.

A good example is the "Amount" property of inventories.  As you can see, the below gets the "gold ingot" quantity in the inventories.

```
Print "Gold Amount: " + the "gold ingot" amount in the "Treasure Chests" inventories
```

The supported Attribute based properties vary based on the block type, as does the behavior.  The only exceptions to this are "actions" and "properties" which are supported on all TerminalBlocks.  See the Terminal Block Handler for more information on these properties.

For more information in Items and Blueprints, see [Items](https://spaceengineers.merlinofmines.com/EasyCommands/items "Items")

## Default Properties 

Block Handlers define default properties, which are by Primitive Type as well as by Direction.  If you read or update a property of a Block Handler, but don't explicitly specify the property, the Block Handler will attempt to resolve the default property.  If direction is supplied, the default property by direction is used.  If a value is supplied, the values Primitive Type is used.  If neither is supplied, the default property for the block's default direction (usually Up) is used.

If a property is specified but a value is not, the Block Handler assumes the given property is a boolean, and assumes the intend value is "true".  So the following will set the "trigger" property of alarm to true.
```
trigger the "Thread Detected" alarm
```

### Default By Direction

Some examples of default directional property being used:

```
#Raise is "action Up".  Default "Up" property on a piston is Height, so we are raising the piston's height.  This is implemented by calling "extend" on the piston.
raise the piston

#Default property for "Clockwise" on a rotor is Angle.  Rotating the angle clockwise is done by setting the rotor velocity to the abs (velocity).
rotate the rotor clockwise
```

### Default By Primitive Type

Some examples of default primitive type property being used:

```
#Default numeric property for piston is height, so this sets the piston's height to 5
set the "Elevator Piston" to 5

#Default boolean property for piston is power, so this set's the piston's power property to on (Enables the piston)
turn on the "Elevator Piston"
```

## Common Block Handler Properties
Most block handlers have a shared set of properties in addition to specific properties for that block type.  See [Terminal Blocks](https://spaceengineers.merlinofmines.com/EasyCommands/blockHandlers/terminal "Terminal Block Handler") for more information, as well as information on how to get and set properties for unsupported block types.

## Managing Inventories and Production Queues
You can also manage inventories and production queues using the [Inventory](https://spaceengineers.merlinofmines.com/EasyCommands/blockHandlers/inventory "Inventory Handler") and [Assembler](https://spaceengineers.merlinofmines.com/EasyCommands/blockHandlers/assembler "Assembler Block Handler") Block Handlers.

Also check out [Items & Blueprints](https://spaceengineers.merlinofmines.com/EasyCommands/items "Items & Blueprints") for information on how to specify items and blueprints to these Block Handlers.

## Supported Block Handlers (still documenting)
* [Air Vents](https://spaceengineers.merlinofmines.com/EasyCommands/blockHandlers/airvent "Air Vent Handler")
* [Antennas](https://spaceengineers.merlinofmines.com/EasyCommands/blockHandlers/antenna "Antenna Handler")
* [Assemblers](https://spaceengineers.merlinofmines.com/EasyCommands/blockHandlers/assembler "Assembler Handler")
* [Batteries](https://spaceengineers.merlinofmines.com/EasyCommands/blockHandlers/battery "Battery Handler")
* [Beacons](https://spaceengineers.merlinofmines.com/EasyCommands/blockHandlers/beacon "Beacon Handler")
* [Cameras](https://spaceengineers.merlinofmines.com/EasyCommands/blockHandlers/camera "Camera Handler")
* [Cockpits](https://spaceengineers.merlinofmines.com/EasyCommands/blockHandlers/cockpit "Cockpit Handler")
* [Collectors](https://spaceengineers.merlinofmines.com/EasyCommands/blockHandlers/collector "Collector Handler")
* [Connectors](https://spaceengineers.merlinofmines.com/EasyCommands/blockHandlers/connector "Connector Handler")
* [Decoys](https://spaceengineers.merlinofmines.com/EasyCommands/blockHandlers/decoy "Decoy Handler")
* [Displays](https://spaceengineers.merlinofmines.com/EasyCommands/blockHandlers/display "Display Handler")
* [Doors](https://spaceengineers.merlinofmines.com/EasyCommands/blockHandlers/door "Door Handler")
* [Drills](https://spaceengineers.merlinofmines.com/EasyCommands/blockHandlers/drill "Drill Handler")
* [Ejectors](https://spaceengineers.merlinofmines.com/EasyCommands/blockHandlers/ejector "Ejector Handler")
* [Gravity Generators](https://spaceengineers.merlinofmines.com/EasyCommands/blockHandlers/gravityGenerator "Gravity Generator Handler")
* [Grinders](https://spaceengineers.merlinofmines.com/EasyCommands/blockHandlers/grinder "Grinder Handler")
* [Guns](https://spaceengineers.merlinofmines.com/EasyCommands/blockHandlers/gun "Gun Handler")
* [Gyroscopes](https://spaceengineers.merlinofmines.com/EasyCommands/blockHandlers/gyroscope "Gyroscope Handler")
* [Hinges](https://spaceengineers.merlinofmines.com/EasyCommands/blockHandlers/hinge "Hinge Handler")
* [Hydrogen Engines](https://spaceengineers.merlinofmines.com/EasyCommands/blockHandlers/engine "Hydrogen Engine Handler")
* [Inventories](https://spaceengineers.merlinofmines.com/EasyCommands/blockHandlers/inventory "Inventory Handler")
* Jump Drives
* [Landing Gears](https://spaceengineers.merlinofmines.com/EasyCommands/blockHandlers/landingGear "Landing Gear / Magnet Handler")
* [Laser Antennas](https://spaceengineers.merlinofmines.com/EasyCommands/blockHandlers/laserAntenna "Laser Antenna Handler")
* [Lights](https://spaceengineers.merlinofmines.com/EasyCommands/blockHandlers/light "Light Handler")
* [Magnets](https://spaceengineers.merlinofmines.com/EasyCommands/blockHandlers/landingGear "Landing Gear / Magnet Handler")
* [Merge Blocks](https://spaceengineers.merlinofmines.com/EasyCommands/blockHandlers/merge "Merge Block Handler")
* [O2/H2 Generators](https://spaceengineers.merlinofmines.com/EasyCommands/blockHandlers/generator "O2/H2 Generator Handler")
* [Ore Detectors](https://spaceengineers.merlinofmines.com/EasyCommands/blockHandlers/detector "Ore Detector Handler")
* [Parachutes](https://spaceengineers.merlinofmines.com/EasyCommands/blockHandlers/parachute "Parachute Handler")
* [Pistons](https://spaceengineers.merlinofmines.com/EasyCommands/blockHandlers/piston "Piston Handler")
* [Programmable Blocks](https://spaceengineers.merlinofmines.com/EasyCommands/blockHandlers/program "Programmable Block Handler")
* [Projectors](https://spaceengineers.merlinofmines.com/EasyCommands/blockHandlers/projector "Projector Handler")
* [Reactors](https://spaceengineers.merlinofmines.com/EasyCommands/blockHandlers/reactor "Reactor Handler")
* [Refineries](https://spaceengineers.merlinofmines.com/EasyCommands/blockHandlers/refinery "Refinery Handler")
* [Remote Controls](https://spaceengineers.merlinofmines.com/EasyCommands/blockHandlers/remote "Remote Control Handler")
* [Rotors](https://spaceengineers.merlinofmines.com/EasyCommands/blockHandlers/rotor "Rotor Handler")
* [Sensors](https://spaceengineers.merlinofmines.com/EasyCommands/blockHandlers/sensor "Sensor Handler")
* [Solar Panels](https://spaceengineers.merlinofmines.com/EasyCommands/blockHandlers/solar "Solar Panel Handler")
* [Sorters](https://spaceengineers.merlinofmines.com/EasyCommands/blockHandlers/sorter "Sorter Block Handler")
* [Sound Blocks](https://spaceengineers.merlinofmines.com/EasyCommands/blockHandlers/sound "Sound Block Handler")
* [Spherical Gravity Generators](https://spaceengineers.merlinofmines.com/EasyCommands/blockHandlers/gravitySphere "Spherical Gravity Generator Handler")
* [Tanks (Oxygen and Hydrogen)](https://spaceengineers.merlinofmines.com/EasyCommands/blockHandlers/tank "O2/H2 Tank Handler")
* [Terminal Blocks](https://spaceengineers.merlinofmines.com/EasyCommands/blockHandlers/terminal "Terminal Block Handler")
* [Thrusters](https://spaceengineers.merlinofmines.com/EasyCommands/blockHandlers/thruster "Thruster Block Handler")
* [Timer Blocks](https://spaceengineers.merlinofmines.com/EasyCommands/blockHandlers/timer "Timer Block Handler")
* [Turrets](https://spaceengineers.merlinofmines.com/EasyCommands/blockHandlers/turret "Turret Handler")
* [Warheads](https://spaceengineers.merlinofmines.com/EasyCommands/blockHandlers/warhead "Warhead Handler")
* [Welders](https://spaceengineers.merlinofmines.com/EasyCommands/blockHandlers/welder "Welder Handler")
* [Wheel Suspension](https://spaceengineers.merlinofmines.com/EasyCommands/blockHandlers/suspension "Wheel Suspension Handler")
* [Wind Turbines](https://spaceengineers.merlinofmines.com/EasyCommands/blockHandlers/turbine "Wind Turbine Handler")
