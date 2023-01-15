# Variables

Variables allow you to retrieve, bind and store values in order to get or set block state(s), store and send information, and in general acts as the glue between the various [Commands](https://spaceengineers.merlinofmines.com/EasyCommands/commands "Commands"). The following describes all the possible types of Variables avaiable to your script, and some examples of how to use them to bring your creations to life.

## Static Variables

Static Variables are the easiest type of variables to create and use.  These include strings, numbers, vectors, colors, and static lists.

```
Print "Numbers:"
Print -3
Print 3.04
Print ""

#Vector (x:y:z)
Print "Vectors:"
Print 1:2:3
Print ""

#Colors
Print "Colors:"
Print red
Print #FF0000
Print ""

#List
Print "Lists:"
Print []
Print [1,2,3]
Print["one" -> 1, "two" -> 2]
```

### Explicit Strings
Sometimes you might want to create strings that include spaces, double quotes, or single quotes, directly in the string.  You can do this by wrapping the string appropriately.

By default, double quoted strings are considered as possible [Selectors](https://spaceengineers.merlinofmines.com/EasyCommands/selectors "Selectors").  The parser should figure out whether you intended it to be a string or a selector, but in some cases you may need to wrap your string.

To wrap double quotes so that they appear in your static string, wrap the entire string with single quotes (').
To wrap single quotes so that they appear in your static stirng, wrap the entire string with tildes (\`).


```
#String
Print "This is a string"
Print 'This is an explicit string, containing "Another String"'
Print `This string isn't worried about having single quotes or having"double quotes"`
```

### Available Static Variables
EasyCommands provides a few variables out of the box, so that you don't need to create them yourself.  These are well known constants which might come in handy for some types of scripts involving logarithmic or circular calculations.

Nothing stops you from overriding these variables, but if you do then don't expect them work as intended.  My recommendation would simply be to avoid overriding these variables with your own.

The x,y,z,r,g,b variables are in places so that you can get the components of vectors & colors easily. See [Primitives](https://spaceengineers.merlinofmines.com/EasyCommands/primitives "Primitives") for more information.

Supported Variables:
* ```x``` - 1:0:0 vector, used for X component of Vectors
* ```y``` - 0:1:0 vector, used for Y component of Vectors
* ```z``` - 0:0:1 vector, used for Z component of Vectors
* ```r``` - 1:0:0 vector, used for R component of Colors
* ```g``` - 0:1:0 vector, used for G component of Colors
* ```b``` - 0:0:1 vector, used for B component of Colors
* ```pi``` - Pi
* ```e``` - Euler's number, useful for e^(myValue)
* ```empty``` - An empty list, useful for comparison (if myList is empty)
* ```NUMBER_FORMAT``` - The number format used when outputting numerical values. See [Numbers](https://spaceengineers.merlinofmines.com/EasyCommands/primitives#numbers "Numbers") for more information.

```
#x = 1:0:0
Print "x: " + x

#y = 0:1:0
Print "y: " + y

#z = 0:0:1
Print "z: " + z

#r = 1:0:0
Print "r: " + r

#g = 0:1:0
Print "g: " + g

#b = 0:0:1
Print "b: " + b

#pi = 3.141593
Print "Pi: " + pi

#e = 2.718282
Print "e: " + e

#empty
Print "empty: " + empty
```

## In Memory Variables

In Memory Variables allow you "remember" values so that you can use them later. You might do this if you need to use a value in many places, or if the value is difficult to calculate and you'd rather not have to re-calculate it.

You can use any alphanumeric name you'd like for your Variables (no spaces allowed and can't look like just a number), but I recommend avoiding names that conflict with reserved keywords.  You can technically get around this by surrounding your variable with double quotes, but that also means that every time you want to use your variable you also need to supply double quotes.

```
#Basic Variable
set myText to "My Text!"
Print "Text: " + myText

#text is a reserved keyword
set "text" to "My Text!"
Print "Text: " + "text"
```

When creating Variables, you can set them to a static value, or an expression using [Operations](https://spaceengineers.merlinofmines.com/EasyCommands/operations "Operations")
```
#Static Variables
set myString to "someValue"
set myNumber to 3
set myVector to 1:2:3
set myColor to red
set myList to ["one", "two", "three"]

#Expressions
set myString to "someValue: " + value
set myNumber to 3 * (2 + 1)
set myVector to 1:2:3 * 4:5:6
set myColor to red + green
set myList to ["one", "two"] + ["three"]

#Other Variable References
set myString to the avg "Elevator Piston" height
set myVector to my position
```

Once you've created variables, you can use them in other commands, or to create other variables.

```
set myValue to "MyValue"
set myPosition to my position
set outputText to "My Value is: " + myValue
increase outputText by "\n" + "My Position is: " + myPosition
Print outputText
```

### Binding Variables

By default, variables are statically assigned based on the value of the variable or expression at the time the variable is assigned.  So in the below example, even though a is re-assigned from 1 to 2, the value of b is not changed:

```
set a to 1 
set b to a + 1
set a to 2

Print "a: " + a
Print "b: " + b
#Result is:
#a: 2
#b: 2
```

Sometimes it's useful to create a variable that references a calculation, rather than being evaluated statically.  To do this, you can use the ```bind``` keywords.  In the below example, the variable b is bound to the expression "a + 1".  So any time a is updated, the value of b will also be updated.

```
set a to 1 
bind b to a + 1

Print "a: " + a
Print "b: " + b
#Result is:
#a: 2
#b: 2

set a to 2

Print "a: " + a
Print "b: " + b
#Result is:
#a: 2
#b: 3
``` 

### Global Variables

By default, In Memory Variables associated with the [Thread](https://spaceengineers.merlinofmines.com/EasyCommands/threads "Threads") in which they were created.  This means that you can run multiple threads using the same variable names without conflict (comes in handy for running multiple instances of the same execution, like managing multiple airlocks).  

To share a variable across threads, you need to create a global variable.  You can do this with the ```global``` keyword.  Global Variables exist until the program is restarted or a new program is loaded.  

```
set global myGlobalVariable to "3"

```

## Comparison Variables

Sometimes you want to compare variables for equality, ordering, or simply to check which values are greater/less than others.

You can do this using the Comparison [Operation](https://spaceengineers.merlinofmines.com/EasyCommands/operations "Operations").  Compared Variables will always result in a boolean type (true/false).  Also note that not all variable types can be compared with other variable types.  See the Comparison Operations for supported comparison.

```
set value1 to 3
set value2 to 2
set myComparison to value2 > value1
Print "Comparison: " + myComparison
```

## Ternary Condition Variable
A common use case you may come across in your script is "if someCondition set a to x, otherwise set a to y".  The long way to do this would be to write something like the following:

```
set someCondition to true
if someCondition
  set a to "someValue"
else
  set a to "otherValue"
```

This is fine & good, but a little lengthy.  Thankfully you can use a ternary condition to make shorten this:

```
set someCondition to true
set a to someCondition ? "someValue" : "otherValue"
Print "a: " + a
```

Fun fact, you can actually use the sentence used to describe this use case to do the same:
```
set someCondition to true
if someCondition set a to x otherwise set a to y
Print "a: " + a
```

## Aggregate Block Variables
Aggregate Variables allow you to get the aggregate property value for a given [Selector](https://spaceengineers.merlinofmines.com/EasyCommands/selectors "Selectors")

Aggregate Variables typically take the form of ```<Aggregator> <Selector> <Property>``` and return a numeric value representing the aggregate value for the given selector + property.

### Implicit Property Selection
If a property is not explicitly provided, the aggregation will attempt to use the default numeric property for the selector's [Block Type](https://spaceengineers.merlinofmines.com/EasyCommands/blockHandlers "Block Handlers").

### Implicit Aggregation
Whenever you request a selector's property, you are implicitly asking for an Aggregation.  When an explicit aggregation is not specified, a special default aggregator is used, with the returned value as follows:
* If there are no blocks in the given selector, returns an empty list (```[]```).
* If there is only 1 block in the given selector, returns the selector's property directly.
* If there are more than 1 blocks in the given selector and all property values are numeric, returns the total sum of all block property values.
* If there are more than 1 blocks in the given selector and not all property values are numeric, returns a list containing the property values of all the given blocks.

Therefore, if you only have 1 block in your selector and ask for a property value, that value will be returned directly.  If your selector has no blocks you will get back an empty list.  Otherwise, you will get either a sum of the selector block's property values (if numeric) or a list of the selector block's property values otherwise.

```
#Implicit Aggregation since "my" will be a selector for the current Programmable Block
set myName to my name
Print "My name is: " + myName

#Implicit Aggregation for a selector with no blocks in it.  Will return an empty list.
set myValue to "Invalid BlockName" terminal names

#Implicit Aggregation for a list of blocks (assuming you have multiple turrets).  Since names is a string property, will return a list of names for your turruts.
set myNames to the turret names

#Implicit Aggregation assuming there is only 1 block named "My Piston".  Since height is a numeric property, will return a SUM of both if there are more than 1 blocks named "My Piston"
set pistonHeight to "My Piston" height
```

### Supported Aggregators

#### Count
Returns a count of the items from the selector.  The property requested does not matter, and can be omitted.

Keywords: ```count, number```

```
set myCount to the count of programs
Print "Program Block Count: " + myCount

if the number of "Exterior Spotlights" < 5
  Print "They're going after at the lights! Pull em back"
```

#### List
Returns a list representing the values for each block in the given selector.

Keywords: ```list, collection```

```
set myRanges to the list of "My Beacon" ranges
Print "My Beacon Ranges are: " + myRanges

for each rangeValue in myRanges
  Print "Beacon Range: " + rangeValue
```

#### Sum
Returns the sum of the provided property values across all the blocks in the given selector.  Most useful for numeric properties.  Returns 0 if there are no blocks in the given selector.

Keywords: ```sum, total```

```
set totalPower to the sum of the battery levels 
Print "Total Power: " + totalPower + " MWh"

set totalTreasure to the total "gold ingot" amount in the "Treasure Chest" inventories
Print "Total Treasure: " + totalTreasure
```

#### Average
Returns the average value of the provided property across all of the blocks in the given selector.  Most useful for numeric properties.  Returns 0 if there are no blocks in the given selector.

Keywords: ```average, avg```

```
set avgOutput to the average battery output
Print "Average Output: " + avgOutput

set averageHeight to the average "Elevator Piston" height
Print "Average Height: " + averageHeight
```

#### Max
Returns the maximum value of the provided property across all of the blocks in the given selector.  Most useful for numeric properties.  Returns 0 if there are no blocks in the given selector.

Keywords: ```maximum, max```

```
set maximumOutput to the maximum "Base Battery" output
Print "Maximum Output: " + maximumOutput

set maximumHeight to the max "Elevator Piston" height
Print "Maximum Piston Height: " + maximumHeight
```

#### Min
Returns the minimum value of the provided property across all of the blocks in the given selector.  Most useful for numeric properties.  Returns 0 if there are no blocks in the given selector.

Keywords: ```minimum, min```

```
set minimumOutput to the minimum "Base Battery" output
Print "Minimum Output: " + minimumOutput

set minimumHeight to the min "Elevator Piston" height
Print "Minimum Piston Height: " + minimumHeight
```

## Aggregate List Variables

Similarly to aggregating values over a [Selector](https://spaceengineers.merlinofmines.com/EasyCommands/selectors "Selectors"), you can aggregate values for a [Collection](https://spaceengineers.merlinofmines.com/EasyCommands/collections "Collection").  The general format is

```
<aggregate> <variable>[]
```

Note that the "[]" is required so that the aggregation knows you are trying to aggregate the given variable as if it is a list.

```
set myList to [1,2,3]
#Count: 3
Print "Count: " + count of myList[]

#Sum: 6
Print "Sum: " + sum of myList[]

#Average: 2
Print "Average: " + avg of myList[]

#Max: 3
Print "Max: " + max of myList[]

#Min: 1
Print "Min: " + min of myList[]
```

You can also get the aggregate of a subset of the items:

```
#List of numbers 0 - 10
set myList to 0..10

#Average: 5
Print "Average: " + avg of myList[]

#Average: 8
Print "Average: " + avg of myList[6..10]
```

## Aggregate Block Conditions
Sometimes you might want to know if all, any or none of a given selector has a specific property.  For example, maybe you want to know if any of your batteries have a power level < 50%, because if so you want to turn on the generators.

There's a few ways you could do this.

```
if the min "Base Batteries" level < 0.5
  turn on the "Base Generators"
```

Another way you can do this is to use Aggregate Conditions.  The general form is

```<aggregateCondition> <selector> <property> <comparison> <value>```, where aggregateCondition can be ```any, all, none```

So another way to write the above would be

```
if any "Base Batteries" level < 0.5
  turn on the "Base Generators"
```

Note that an "all/none" block condition will always return true for a selector with no matching blocks.  Similarly, an "any" block condition will always return false for a selector with no matching blocks.

## Aggregate List Conditions

Similarly to Aggregate Block Conditions, you can check an Aggregate List Condition.  The general format is

```<aggregateCondition> <list>[] <comparison> <value>```.  Again, supported aggregate conditions are ```any, all, none```

It is important to specify "[]" so that the script knows you intend to perform an aggregate condition over the variable as if it is a list.

```
set myList to [1,2,3]

if any of myList[] > 2
  Print "List contains an item > 2"
```

Similarly to Aggregate Block conditions, the "all/none" checks will return true for an empty list and "any" will return false for an empty list.