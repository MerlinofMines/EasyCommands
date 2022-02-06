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

String primitives allow you to specify human readable things to render on screen.  These are useful for messages and labels.

```
set myMessage to "This is my message"
print "My Message is: " + myMessage
```

String do not have to be wrapped in quotes, but best practice would be to wrap things you intend to be strings.  Any random text is considered a string if and only if it does not map to a reserved keyword, function name, or variable name and cannot be parsed as a different primitive (like a vector or a color).  EasyCommands will attempt to parse any non-quoted strings as other primitives types before considering them as a string.

```
set a to red
print "a is: " + a
#a is: #FF0000

print "a is: " + "a"
#a is: a
```

That said, you can cast quoted strings to other primitive types using the cast operation.

Colors:

```
set myString to "red"
print myString
#red

print myString as color
#FF0000
```

Vectors:

```
set a to 4
Print "Vector: " + a:b:c
#Vector: 4:b:c

Print "String: " + "a:b:c"
#String: a:b:c
```

This is also handy for parsing GPS coordinates as vectors.

```
set myVector to "GPS:merlin waypoint:1:2:3:#FFFF0000"
print "myVector is: " + myVector
#myVector is: 1:2:3
```

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

### Reading and Writing to Separate Lines
Sometimes you may want to read, or write, the newline character in order to parse output by line, or to concatenate strings separated by lines.

You can do this by specifying "\n" anywhere inside a string value, or separately.  EasyCommands will convert this to the newline character internally.

```
#All of these are equivalent

print "Output Line 1\nOutput Line 2"

#Blank Line
print "\n"

set myOutput to "Output Line 1"
myOutput+= "\n" + "Output Line 2"
print myOutput

#Blank Line
print "\n"

set myOutputs to ["Output Line 1","Output Line 2"]
print myOutputs joined "\n"
```

Similarly, you can split input text by line number using the Split operation and "\n"

```
set myInputText to "My Display" text

set myOutputLines to myInputText split "\n"

for each outputLine in myOutputLines
  doSomething outputLine

:doSomething outputLine
print "Output Line: " + outputLine
#Your code here
```

See [Operations](https://spaceengineers.merlinofmines.com/EasyCommands/operations "Operations") for more information about the Split and Join operations.

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

### Vector Components
To access components of a given vector, you can use ```myVector.x, myVector.y, myVector.z```

```
set myVector to 0:1:2

Print "X: " + myVector.x
Print "Y: " + myVector.y
Print "Z: " + myVector.z
```

### Creating Vectors Using Variables

You can create vectors using [Variables](https://spaceengineers.merlinofmines.com/EasyCommands/variables "Variables") using the syntax "a:b:c" where a,b,c are the names of variables.  Note that you must use "a:b:c" format.  "a : b : c" will not work.

Similar rules for setting or binding variables applies to Variable Vectors.  Setting a variable to a variable vector will set a static vector based on the values of the variables at the time it was created.  Binding a variable to a variable vector will effectively create a vector with dynamic values.

Any variable you use to create a vector needs to resolve to a number.  If you specify a variable with any other Primitive type, you will get a script halting exception.

```
set xComponent to 1
set yComponent to 2
set zComponent to 3

#Static Vector
set myVector to xComponent:yComponent:zComponent

#Dynamic Vector
bind myDynamicVector to xComponent:yComponent:zComponent

set xComponent to 4

print "My Vector: " + myVector
#1:2:3

print "My Dynamic Vector: " + myDynamicVector
#4:2:3
```

## Colors

There are some blocks types that support colors, such as text colors on screen.  You can create colors using special reserved keywords, or by explicitly declaring hex values.

```
#All of these are the equivalent color
set myColor to orange
set myColor to #FFA500
set myColor to 255:165:0 as color

set my display color to myColor
```

### Color Components
To access the color components (RGB) of a given color, you can use ```myColor.r, myColor.g, myColor.b```.  Values returned will be an integer between 0 - 255.

```
set myColor to #00FF00

Print "R: " + myColor.r
Print "G: " + myColor.g
Print "B: " + myColor.b
```

### Constructing Colors from RGB values
The easiest way to construct colors from RGB values is to create a vector representing R:G:B and then convert it to a color using the Cast operation.

```
#Using Static Vectors
set myColor to 255:128:45 as color
print myColor

#Using Dynamic Vectors
set myRed to 255
set myGreen to 128
set myBlue to 45
set myColor to myRed:myGreen:myBlue as color
print myColor
```

Using this, you can do some pretty cool things for changing light colors.  Here's a script for making a rainbow effect:
```
#Change these to adjust the color spectrum and speed
set rgbValues to [255,128,0]
set rgbIncrements to [-5,2,3]

for each i in 0..2
  async updateColor i

goto updateLights

:updateColor rgbIndex
if rgbValues[i] >= 255
  set rgbIncrements[i] to -abs(rgbIncrements[i])
else if rgbValues[i] <= 0
  set rgbIncrements[i] to abs(rgbIncrements[i])
set rgbValues[i] to (rgbValues[i] + rgbIncrements[i])
replay

#Vectors are auto-converted to colors if you set a color property to a vector
:updateLights
set the lights color to rgbValues[0]:rgbValues[1]:rgbValues[2]
replay
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
