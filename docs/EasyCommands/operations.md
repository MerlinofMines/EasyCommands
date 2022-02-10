# Operations

Operations allow you to manipulate and combine [Variables](https://spaceengineers.merlinofmines.com/EasyCommands/variables "Variables") together using addition, subtraction, etc.  The result of an Operation always returns another Variable, so you can chain operations together:

```
set a to 1
set b to a + 1
set c to a + b + 1
```

There are a few types of Operations:
* "Uni" (meaning 1 operand),
* "Bi" (meaning takes 2 operands),
* "Ternary" operand that takes 3 operands.

UniOperations include operations like ```not, abs, sin, cos, tan``` etc.  Note that some UniOperands expect the operand to preceed the operations (such as ```keys```, and ```values```)

BiOperations include things like "+", "-", "*", "/", "%", etc.

Below is a description of the various operations supported by EasyCommands, including the behavior for different [Primitive Type](https://spaceengineers.merlinofmines.com/EasyCommands/primitives "Primitive Types") input(s) and output type.

One final note: In general, operations that produce a collection typically start with a collection (with a couple of exceptions).  For more information on how you can work with collections of things in EasyCommands see [Collections](https://spaceengineers.merlinofmines.com/EasyCommands/collections "Collections").

## Order of Operations

Operations happen in "tiers", meaning some operations are performed before others.  For example, multiplication & division are attempted before addition and subtraction.  Note that addition does not always happen before subtraction; they are the same tier of operation.

``` 
set result to 1 + 2 * 3
Print "Result: " + result
#Result: 7
```

Also, all UniOperands are performed before other operands.

```
set result to abs -1 + 2
Print "Result: " + result
#Result: 3
```

All UniOperand Operations are evaluated before BiOperand Operations.

Here are the tiers for BiOperand Operations:
* Tier 0 BiOperand Operations: ```.```
* Tier 1 BiOperand Operations: ```^, round```
* Tier 2 BiOperand Operations: ```*, /, %, split, join```
* Tier 3 BiOperand Operations: ```+, -```
* Tier 4 BiOperand Operations: ```.., cast```

### Changing Order of Operations
To change the order of operations, use parentheses ```()```.  Anything in parentheses is evaluated first.  Order of operations within the parantheses follows the same rules as outside.  You can nest parentheses to further change order of operations.

```
set result to (1+2)*3
Print "Result: " + result
#Result: 9
```

```
set result to abs (-1 + 2)
Print "Result: " + result
#Result: 1
```

```
set result to abs (-2 * (1 + 3))
Print "Result: " + result
#Result: 8
```

## Casting Variables To Other Types
Sometimes you may want to convert one input type to another, for example, a string input which is actually supposed to be a number.  You can do this using the Cast Operation.

Keywords:  ```cast, as, resolve, resolved```

The Cast operation can be used as either a UniOperand or as BiOperand.

### Casting As a UniOperand

When used as a UniOperand, the Cast operand will attempt to parse a given string variable as another variable type, by looking at its structure.  Here are some Examples:

```
set myBool to cast "true"
set myNumber to cast "3.14"
set myVector to cast "1:2:3"
set myColor to cast "red"
set myColor to cast "#FF0000"
set myString to cast "Some random String"
```

Casting strings to lists is not currently supported.  If the input variable is not a string, it is returned without any conversion.  So casting a vector does nothing.

See the Cast under UniOperands for more information.

### Casting as a BiOperand

When used as a BiOperand, the Cast Operation expects the second variable to be a string representing one of the supported cast types.

See Cast under BiOperands below for more information.

## The Minus Sign
```-``` can be used as either a UniOperand to negate a value (See the Not Operation)), or as a BiOperand to subtract a value from another value (see the Subtraction operation).

```
#Negation
set a to 2
set b to -a
print "b: " + b
#b: -2
```

```
#Subtraction
set a to 2
set b to 5 - a
print "b: " + b
#b: 3
```

EasyCommands will do its best to infer whether you meant by "-", but may not be perfect.  If needed, use parantheses to clarify your intent.

Negation resolves before subtraction, so ```-a-a``` resolves to ```(-a) - a```, vs ```-(a-a)```.

## Rounding
Rounding can be used to either quickly round number and vector values to an integer, or to round numbers and vector values to a specific number of digits.

Keywords:  ```round, rnd, rounded```

Note that "digit and "digits" are ignored keywords.

If the digits to round to are not specified, rounding takes precedence over other BiOperands like ```*,+``` etc.  When the number of digits are specified, then Rounding acts as a tier 1 BiOperand.

Here's some examples for rounding numbers:
```
print pi rounded to 2 digits
#3.14

print "Decimal: " + pi rounded 1
#Decimal: 3.1

print "Integer: " + 9.81 rounded
#Integer: 10

set myValue to 5.617
print "My Value: " + round myValue
#My Value: 6
```

And here's some examples for rounding vectors:
```
print "1.11:2.22:3.33" rounded to 1 digit
#1.1:2.2:3.3

print "1.11:2.22:3.33" rounded
#1:2:3

print "My Cockpit" velocity rounded to 2 digits
```

## Uni Operations

### Absolute Value
Behavior varies based on input type.

Keywords: ```abs, absolute```

* **(Number)**: Absolute value of the number
* **(Vector)**: Returns the length of the vector

### Arc Cosine
Performs the arc cosine operation on the given numeric value

Keywords: ```acos, arccos```

### Arc Sine
Performs the arc sine operation on the given numeric value

Keywords: ```asin, arcsin```

### Arc Tangent
Performs the arc tan operation on the given numeric value

Keywords: ```atan, arctan```

### Cast (Before or After)
Casts the given string variable to another variable type by attempting to resolve it to the appropriate type.

Keywords: ```cast, resolve, resolved```

```
#number
set myVariable to cast "123.4"

#Vector
set myVariable to "1:2:3" resolved

#Color
set myVariable to resolve "#FF0000"
```

If the input type is not a string, will return the original value.  If the input string cannot be parsed as a supported Primitive Type, the original string will be returned.

```
#Returns the original string
set myVariable to resolve "Some Random String"
```

### Cosine
Performs the cosine operation on the given numeric value

Keywords: ```cos, cosine```

### Keys (After)
Gets the keys from the given list.  The list is expected to come before the operand.  Items without keys are not returned in the resulting list.

Keywords: ```keys, indexes```

```
set myList to ["one" -> 1, "two" -> 2, 3]
#myKeys will be ["one","two"]
set mykeys to myList keys
```

### Natural Logarithm
Returns the logarithm of the given numeric value to the base of the mathematical constant e

Keywords: ```ln```

### Not
Inverses a given property.  Behavior varies by input type

Keywords:
```not, !, isnt, arent, stop, -```

* **(Boolean)**: inverts the boolean
* **(Number)**: multiplies the number by -1
* **(Vector)**: returns the inverse of the given vector
* **(Color)**: returns the inverse of the given color
* **(List)**: returns a list whose order is reversed

Example:
```
set myValue to true
set myNewValue to not myValue
```

Not can also be used in some property conditions.
```
if the "Gatling Turrets" are not firing
  Print "Why arent you firing??"
```

### Random
Behavior varies based on input types.

Keywords: ```random, rand```

* **(Number)**: Returns a random whole number between 0 and the specified value (exclusive).
* **(List)**: Returns a random item from the given list.

```
#Random number between 0 and 9
print "Random Number: " + rand 10

#Random item from list
Print "Random Item: " + rand [one,two,three,four,five]

#Random number between 5 and 10 using List
print "Random Number: " + random (5..10)
```

### Reverse
Reverses the given list

Keywords: ```reversed```
```
set myList to ["one", "two", "three"]
set myReversedList to reversed myList
```

### Round (before or after)
Behavior varies based on input types.

Keywords: ```round, rnd, rounded```

* **(Number)**: Rounds the given number to the nearest integer (half-up)
* **(Vector)**: Rounds each vector component to the nearest integer (half-up)

### Shuffle
Shuffles the given list.  This does not modify the input list but rather returns a shuffled copy of the input list.

Keywords: ```shuffle, shuffled```

### Sign
Behavior varies based on input type.

Keywords: ```sign, quantize```

* **(Number)**: Return the sign of the number or 0
* **(Vector)**: Returns the vector with the sign of each component or 0

### Sin
Performs the sin operation on the given numeric value

Keywords: ```sin```

### Square Root
Returns the Square Root of the given number

Keywords: ```sqrt```

### Sort
Sorts the given list using the comparison operand against each value (see below). This operation is expensive so be careful when using it.

Keywords: ```sorted```

```
set myList to [3,4,1,2]
#Becomes [1,2,3,4]
set myReversedList to sorted myList
```

### Tangent
Performs the tangent operation on the given numeric value

Keywords: ```tan, tangent```

### Ticks (After)
Converts the given number from seconds to ticks

Keywords: ```tick, ticks```

Example: 
```
#Wait 30 ticks = 0.5 seconds
wait 30 ticks

#Wait 30 seconds
wait 30
```

### Type (After)
Returns the [Primitive Type](https://spaceengineers.merlinofmines.com/EasyCommands/primitives "Primitives") of the given variable as a string.

Keywords: ```type```

Possible output values: ```boolean, string, number, vector, color, list```

```
set myVariable to "True"
print myVariable type
#bool

#Notice the quotes
set myVariable to "123"
print myVariable type
#string

set myVariable to 123
print myVariable type
#number

set myVariable to 1:2:3
print myVariable type
#vector

set myVariable to #FF0000
print myVariable type
#color

set myVariable to [1,2,3]
print myVariable type
#list
```

This is particularly useful when trying to cast arbitrary input as a value.

```
#Pretend the text is "1"
set myInput to "My Display" text

#This will resolve the input string to a primitive type but you don't know which
set myResolvedVariable to resolve myInput

#This will tell you what type you are dealing with
set myVariableType to myResolvedVariable type
print "Type: " + myVariableType
#number

if myVariableType is number
  set myNewValue to myResolvedVariable + 10
  print myNewValue
  #11
else
  set "My Display" text to "Input must be a number! Try again"
```

### Values (After)
Gets the values from the given list (ignoring keys).  The list is expected to come before the operand.

Keywords: ```values```

```
set myList to ["one" -> 1, "two" -> 2, 3]
#myValues will be [1,2,3]
set myValues to myList values
```

## BiOperand Operations

### Addition
Behavior varies based on input types.

Keywords: ```plus, +```

* **(String, Any)**: Concatenates the first string with the second variable after converting the second variable to a string.
* **(Any, String)**: Concatenates the first variable (as a string) with the second string
* **(Number, Number)**: Adds two numbers together, returning a number
* **(Vector, Vector)**: Performs Vector Addition on the two vectors
* **(Vector, Number)**: Returns a Vector in the same direction as "a" whose length has been increased by "b" amount
* **(Color, Color)**: Adds two colors together by adding together RGB values (capped at 255, of course)
* **(List, Any)**: Adds the second item(s) to the first list, at the end
* **(Any, List)**: Adds the first item(s) to the second list, inserted at the beginning

### And
Checks whether both boolean operands are true (a && b)

Keywords:
```and, &, &&, but, yet```

### Cast

This special operation allows you to cast a given value as another value.  This enables you to construct something using a string, and then convert it to a vector or color, for example.

Keywords: ```cast, as```

Cast Types: ```bool, boolean, string, number, vector, color, list```

```
set myVector to 1 + ":" + 2 + ":" + 3
set myVector to myVector as vector
set the "Remote Control" destination to myVector
```

If you attempt to cast a variable to a type that it cannot be converted to, you will get a script halting exception.  So before casting to a specific type make sure you know you can cast it to that type.

If you are unsure of the type, use the "Cast" UniOperand to cast it to whatever type it actually is, and then check its type using the Type operation before converting.

```
#Pretend I don't know what this value is
set myInputValue to "My Display" text

set myResolvedValue to cast myInputValue
if myResolvedValue type is not number
  Print "Input Value must be a number!"
else
  Print "Input Number is: " + myResolvedValue
```

### Supported Casts

#### Boolean
* Number -> Bool, returns true if != 0, false otherwise
* String -> Bool, returns true if the string is a represents a number != 0, or if the lowercase string equals "true".  Throws an exception if the string represents a vector or color.  Returns false otherwise.

#### Number
* Bool -> Number, returns 1 if true, 0 otherwise
* String -> Number, attempts to parse the given string as a number.  Throws a script halting exception if unable to parse.
* Vector -> Number, returns the vector's length

#### String
* Bool -> String, returns ```True``` if true, ```False``` otherwise
* Number -> String, returns the string version of the number
* Vector -> String, returns the string version of the vector, in the form ```x:y:z```
* Color -> String, returns the hex string version of the color, in the form ```#RRGGBB```
* List -> String, prints out the list in the form ```[key1->value1,value2,key3->value3]``` (keys only included if present, order maintained)

#### Vector
* String -> Vector, attempts to parse the given String as a vector by separating values by colon and looking for 3 numeric values.  Can be used to parse vectors as well as GPS coordinates.
* Color -> Vector, returns a vector whose x,y,z components are the r,g,b components of the color, respectively.

#### Color
* String -> Color, attempts to parse the given String as a color, using either known colors (```red```) or Hex syntax (```#FF0000```)
* Vector -> Color, returns a Color whose r,g,b components are the x,y,z components of the vector, respectively.  Useful for creating colors directly using RGB values.

#### List
List has no supported conversion types currently

### Comparison

There are a few different comparisons supported by EasyCommands.  Comparison support and behavior changes slightly based on the input types, see below for description.

#### Equals
Keywords: ```is, are, equal, equals, =, ==```

#### Not Equals
Keywords: ```is not, are not, !=, is not equal, not equal```

#### Less Than
Keywords: ```less than, <, below```

#### Greater Than
Keywords: ```greater than, >, more than, above```

#### Greater Than Or Equal
Keywords: ```>=```

* **(Boolean, Boolean)**: compares booleans.  true is greater than false.  True is equal to true and false is equal to false

* **(String, String)**: compares strings using lexicographical order

* **(Number, Number)**: compares two numbers

* **(Vector, Vector)**: supports equality.  Other comparisons not supported

* **(Color, Color)**: Compares two Colors using their packed integer value

* **(List, List)**: Compares two lists for equality (membership and order).  Other comparisons not supported

* **(Vector, Number)**: Compares the vector's length against the given number

* **(Number, Vector)**: compares the number against the given vector's length

### Contains
Checks whether the first operand contains the given value. (a contains b).  Behavior varies based on input types.

Keywords:

```
contains
```

* **(String,Any)**: Converts the second operand to a String and then checks if the first operand contains the second operand
* **(List, Any)**: Checks whether a contains b (or all the elements of b if b is a collection)

Examples:

```
#True
set containsValue to "My Value" contains "Value"

#False
set containsValue to "My Value" contains "Me"
```

```
set myList to ["one", "two", "three"]
#True
set containsValue to myList contains "one"
set containsValue to myList contains ["one", "three"]

#False
set containsValue to myList contains 1
set containsValue to myList contains ["one", "four"]
```

### Division
Behavior varies based on input types.

Keywords: ```/, divide```

* **(Number, Number)**: Divides the first number by the second
* **(Vector, Vector)**: Divides the first vector by the length of the second vector, returning a Vector
* **(Vector, Number)**: Divides the vector by the Number, returning a Vector in the same direction as a with a magnitude reduced by a factor of b.
* **(Color, Number)**: Divides the color by the Number, returning a Color

### Dot Product
Returns the dot product of the given two vectors.

Keywords: ```.```

#### Accessing Vector and Color Components

The dot product is used in a clever way in order to get component information for vectors and colors.

You can use ```myVector.x, myVector.y, myVector.z``` to get the components of a given vector.

Also, you can use ```myColor.r, myColor.g, myColor.b``` to get the RGB components of a given color.

See the Vector and Color [Primitives](https://spaceengineers.merlinofmines.com/EasyCommands/primitives "Primitives") for more information.

### Exponent (Also XOR)
Behavior varies based on the input types.

Keywords: ```^, pow, xor```

* **(Bool, Bool)**: Performs the XOR operation on the two given bools.
* **(Number, Number)**: Raises the first number to the power of the second number
* **(Vector, Vector)**: Returns the angle (in degrees) between the two given vectors (get it? ```^```)

### Join
Joins a given list by the given string separator by casting each list value to a string and then joining them together using the given separator.  When joining values the keys are stripped.  Only the values get joined.

Keywords: ```join, joined```

```
set myOutput to [1,2,3] joined ", "
#1, 2, 3
```

```
#Line Separated Values
set myOutput to [1,2,3] joined "\n"
print myOutput
#1
#2
#3
```

```
#Keys are stripped
set myOutput to ["one" -> 1, "two" -> 2, "three" -> 3] joined ", "
print myOutput
#1, 2, 3
```

### Modulus
Behavior varies based on input types.

Keywords: ```%, mod```
* **(Number, Number)**: Returns a % b, meaning return the remainder of a / b
* **(String, String)**: Removes all instances of b from the string a
* **(Vector, Vector)**: Performed Vector Rejection of b on the given Vector A, returning the resulting Vector.

### Multiplication
Behavior varies based on input types

Keywords: ```*, multiply```

* **(Number, Number)**: Multiplies a by b
* **(Vector, Vector)**: Returns the Vector Cross Product of a and b (order matters!). 
* **(Vector, Number)**: Returns a Vector in the same direction as a with the magnitude multiplied by b
* **(Number, Vector)**: Returns a Vector in the same direction as b with the magnitude multiplied by a
* **(Color, Number)**: Returns a Color multiplied by the factor of b
* **(Number, Color)**: Returns b Color multiplied by the factor of a

### Or
Checks whether either boolean operand is true (a || b).

Keywords:
```or, |, ||```

### Range
Creates a Collection consisting of numbers between a and b (inclusive).

This operation is often used for iterating through existing collections by index, or fetching a subset of values from a given collection.  See [Collections](https://spaceengineers.merlinofmines.com/EasyCommands/collections "Collections") for more information on how to use the Range operation to perform various operations on a given Collection.

Keywords: ```..```

Example:

```
#[0, 1, 2]
set myList to [0..2]

set myList to ["one", "two", "three"]
for each i in 0..count of myList[] - 1
  Print "myList[" + i + "]: " + myList[i]
```

### Round
Behavior varies based on input types.

Keywords: ```round, rnd, rounded```

* **(Number, Number)**: Rounds the given number a to the given number of digits, b. (half-up)
* **(Vector, Number)**: Rounds each vector component of the given vector a to the given number of digits, b. (half-up)

### Split
Splits the given string by the given string separator.  The result is a List containing the separated values.

Keywords: ```split, separate, separated```

```
set myOutput to "My Values" split " "
#[My,Values]
```

```
#Get Display Output Lines
set myOutput to my display[0] text split "\n"
```

### Subtraction
Behavior varies based on input types.

Keywords: ```-, minus```

* **(Number, Number)**: Returns a - b
* **(String, String)**: Returns a copy of "a" with the first instance of b removed.
* **(String, Number)**: Returns a with the last b characters removed.  if b >= a.length, returns an empty string.
* **(Vector, Number)**: Returns a copy of the vector with it's magnitude reduced by b amount.
* **(Vector, Vector)**: Returns the vector produced by a - b
* **(Color, Color)**: Returns the resulting color from subtract b from a
* **(List, String)**: Removes the item for the given key, if an entry for the given key exists.  Otherwise returns a copy of the original list.
* **(List, Number)**: Removes the item at the given numeric index (0 based) from the provided list.  If the index is outside of the size of the list, throws a script halting exception.
* **(List, List)**: Removes all items either the keys and/or indexes provided in the second list.

## Ternary Operations

There's only one supported ternary operations, currently.  It uses the first operand as a conditional check.  If the check is true, it returns the 2nd operand; otherwise, it returns the 3rd operand.

The format is: ```conditionVariable ? trueVariable : falseVariable```

```
set myCondition to true
set myValue to myCondition ? "True!" : "False!"

Print myValue
#True!

set myCondition to false
set myValue to myCondition ? "True!" : "False!"

Print myValue
#False!
```

