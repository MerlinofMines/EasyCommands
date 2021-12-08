# Primitive Types

Primitive Types in EasyCommands represent the different kinds of [Variables](https://spaceengineers.merlinofmines.com/EasyCommands/variables "Variables") that you can create and use as part of your script.  There only a few supported Primitive Types in EasyCommands, discussed below.

See [Operations](https://spaceengineers.merlinofmines.com/EasyCommands/operations "Operations") for a list of all the operations that you can perform on the various primitive types.

## Boolean
The most basic primitive type is boolean, which can either be true or false.  Booleans are used when evaluating conditions and setting true/false like "Enabled".
```
set myValue to true
set the "Outer Lights" power to myValue
if myValue
  print "My Value is true!"
  set myValue to false
```
Booleans can be represented using the following reserved keywords.
True: true, on, begin, start, started, resume, resumed
False: off, terminate, cancel, end, false, stop, stopped, halt, halted

## String

String primitives allow you to specify human readable things to render on screen.  These are useful for messages and labels

```
set myMessage to "This is my message"
print "My Message is: " + myMessage
```

String do not have to be wrapped in quotes, but best practice would be to wrap things you intend to be strings.

### Explicit Strings

By default, items in double quotes are considered as possible [Selectors](https://spaceengineers.merlinofmines.com/EasyCommands/selectors "Selectors").  If you want to create a string that should not be mistaken for a selector, you can use single quotes instead of double quotes.

```
set myBlocks to the 'Outer Lights'
```

Explicit Strings can also be used for wrapping other strings containing double quotes (such as commands sent to other grids)

```
set myCommand to 'turn on the "Outer Lights"'
send myCommand to myChannel
```

## Numbers
Numbers are used for many properties that are numeric values.  Numbers are used for both integers and decimals, there is only 1 type for both.

```
set the "Elevator Piston" velocity to 2.5
set the "Gatling Turret" range to 800
if the "Gatling Turret" range < 600
  Print "Close Quarters only"
```

Internally numbers are stored using floating point precision (as is most of EasyCommands) so keep this in mind when trying to set really precise values.

## Vectors

Vectors represent 3D coordinates and are used for a variety of purposes, such as for positions, directions, target locations, detected entity locations, and some other interesting properties.  They take the form "x:y:z" where x,y,z represent the x,y,z coordinates, respectively.

```
set myCoords to "4:5:6"
set the "Remote Control" waypoint to myCoords
set myPosition to my position
```

## Colors

There are some blocks types that support colors, such as text colors on screen.  You can create colors using special reserved keywords, or by explicitly declaring hex values.

```
set myColor to orange
set myColor to #FFA500
set my display color to myColor
```

## Collections

Sometimes you need to deal with collections of things.  Some properties end up being collections, and you can also create collections yourself.  EasyCommands only supports 1 kind of Collection (called a KeyedList) which serves as both a Dictionary and as a List.  Note that some performance is sacrificed to support this, so don't expect to create lists of hundreds or thousands of items.   See [Collections](https://spaceengineers.merlinofmines.com/EasyCommands/collections "Collections") for a full description of what you can do with Collections.  Below is just a simple example.

```
#List
set myList to ["one", "two", "three"]
for each item in myList
  print "Item: " + item

#Dictionary
set myDictionary to ["one" -> 1, "two" -> 2, "three" -> 3]
set myKeys to myDictionary keys
for each key in myKeys
  print "Value: " + myDictionary[key]
```
