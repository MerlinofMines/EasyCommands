# Selectors

Selectors are used to specify when you want to retrieve a value from, or act on, a block or group of blocks.  A "block" represents anything you can build/place on a Grid which is able to be interacted with (Pistons, Rotors, Antennas, Warheads, Displays, Inventories, etc).

Selectors are used in several [Commands](https://spaceengineers.merlinofmines.com/EasyCommands/commands "Commands") and [Variables](https://spaceengineers.merlinofmines.com/EasyCommands/variables "Variables").  In general, if you ware interacting with a block, you'll need to specify a Selector to indicate which blocks.

There a few different kinds of Selectors that you can choose from, which are described below.  

## Block Selector

The most commonly used Selector is a BLock Selector.  This selector is useful when you want to interact with a specific block, or group of blocks.

There are a few forms a Selector can take, but the general form is ```<selector> <blockType>```, where ```selector``` represents the name of the block or block group, and ```blockType``` is one of the reserved [Block Types](https://spaceengineers.merlinofmines.com/EasyCommands/blockHandlers "Block Handlers").  Note that most blockTypes have a corresponding "blocktype + group" keyword meaning "this selector is a block group".  Typically the plural form of the block type indicates it is a block group, though there are a couple of exceptions to this (like displays).  

Supported ways to specify a Selector include:

```
#Explicit blocktype
open the "Main Base" door

#Explicit block type with explicit string
open the 'Main Base' door

#Explicit block type & group
extend the "Elevator Pistons" piston group

#Implicit block type & group
"Elevator Pistons" pistons
```

### Implicit Block Types

It's often the case that the name of the block, or block group, you are selecting contains what kind of block it is.  For these cases, it is OK to emit the block type.  EasyCommands will infer the block type (and if it is a group) based on the name of the entity.
```
#Assume Elevator Pistons is a Block Group of Pistons
extend the "Elevator Pistons"

#Assume Outer Door is a single door
open the "Outer Door"
```

When inferring the block type from the name, EasyCommands uses the **last most** block type + group.

```
#Assumes this is a single door
open the "Battery Room Door"

#Assumes this is a group of Pistons
extend the "Outer Door Pistons"
```

### Using Block Types for Groups containing multiple types of blocks

Sometimes it's useful to create groups of related blocks with different types.  Maybe you want to organize blocks by "rooms" in your base.  For these scenarios, you can use an explicit block type group to get specific blocks from a group containing multiple types.

```
turn on the "Battery Room" lights
if the avg "Battery Room" batteries power < 0.5
  turn on the "Generator Room" generators
```

## Variable Selectors

It's often the case that you want to create a variable first, and then use the variable in multiple places as a Selector. You can do this by specifying a blockType (and optional group) after the Variable

```
set myBatteries to "My Base Batteries"

turn on myBatteries batteries
```

You can also specify "$" in front of the Variable to indicate to EasyCommands that the given Variable is being used as a Selector.

```
set myBatteries to "My Base Batteries"
turn on $myBatteries batteries
```

Block Type & group can also be inferred using Variable Selectors, so if variable itself contains the block type, you can omit the block type.

```
set myBatteries to "My Base Batteries"
turn on $myBatteries
```

This is by far the quickest and most common method of creating variables to represent selectors and re-use them in your script.

Variable Selectors are resolved before other modifiers on selectors (such as indexes and conditions).  So the following will resolve "myBatteries" and then grab the first item.

```
#The following turns on the first battery in My Batteries
set myBatteries to "My Batteries"
turn on $myBatteries[0]
```

If instead you intend to use the first item of a list as a selector, you need to wrap that part in parentheses to clarify the intent:

```
#The following turns on all batteries in "My Batteries"
set mySelectors to ["My Batteries", "My Generators"]
turn on $(mySelectors[0])
```

## Index Selector

Sometimes you might want to turn on only specific blocks in a given selector, either by index or by name.  You can do this using an Index Selector.  Index Selectors follow the same syntax as [Collection](https://spaceengineers.merlinofmines.com/EasyCommands/collections "Collection"), namely "[]".  You can also use "@".  Items in the specified List of Indexes can be numeric or String.  If numeric, the Selector will attempt to fetch the block by index value.  If a string, the Selector will attempt to fetch blocks with the same name as the given index.  

```
#Turn on the first battery in Base Batteries
turn on "Base Batteries"[0]

#Turn on the first 3 batteries in Base Batteries
turn on "Base Batteries"[0..2]

#Turn on the first 3 batteries, and any batteries named "Extra Battery"
turn on "Base Batteries"[0..2, "Extra Battery"]
```

If a supplied index is greater than the number of blocks, or if the selector does not include a block named for the string selecctor, then the selector returns an empty set of blocks to act on.

Index Selectors can be used in combination with any other Selector modifier, such as Conditions.  I recommend parentheses to make it clear what you intend as the final result.

```
#Recharge the first battery in "My Batteries" that is less than 1/2 way charged.
recharge ("My Batteries" whose ratio < 0.5)[0]
```

## All Selector

Sometimes you might want to get all blocks of a specific type, regardless of their name or groups.  You can do this with an All Selector.  An All selector is created if you specify only a block type, and optionally the "all" keyword.  This selector requires that you specify the block type explicitly.

Since you are grabbing all blocks of the given type, plurality of the block type keywords is irrelevant.

```
#Turns on all of the generators
turn on all of the generators

#Also turns on all of the generators.  
turn on the generator
```

## Self Selector

The Self Selector is mostly used for referencing the Programmable Block which is currently running EasyCommands.  it's useful for things like setting the current display to some text, or getting the position and orientation of the programmable block

```
#Sets the display text to Hello World.
set my display text to "Hello World!"

Print "My Position is: " + my position
```

### My as an All Selector

If you specify a block type that is not "Program" or "Display", then the Self Selector acts like an All Selector. This is mostly for convenience.

```
#Turns on all of the batteries
turn on my batteries
```

## Conditional Selector

Conditional selectors allow you to filter the blocks in another selector based on a condition.  Conditional Selectors are indicated using keywords ```that, whose, which```

The general format of a Conditional Selector is ```<selector> <that/which/whose> <property> <comparison> <value>```.  "Property" is optional and can be inferred using the [Block Type's](https://spaceengineers.merlinofmines.com/EasyCommands/blockHandlers "Block Handlers") default property for the given comparison value.  the "value" can be omitted if a boolean property is supplied.  One of Property and Value must be supplied, along with the comparison.


```
set batteryCount to the count of my batteries

set rechargingBatteryCount to the count of my batteries that are recharging

Print "Battery Count: " + batteryCount
Print "Recharging Battery Count: " + rechargingBatteryCount
```

You can chain multiple conditions together to perform even more filtering

```
#Get a count of fully recharged batteries that are still recharging
set fullyRechargedBatteries to the count of batteries whose ratio > 0.99 and that are recharging
Print "Fully Recharged Batteries: " + fullyRechargedBatteries

#All batteries that are fully recharged can be set to auto
set to auto batteries whose ratio > 0.99 and that are recharging
```
