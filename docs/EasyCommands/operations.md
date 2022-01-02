# Operations

Operations allow you to manipulate and combine [Variables](https://spaceengineers.merlinofmines.com/EasyCommands/variables "Variables") together using addition, subtraction, etc.  The result of an Operation always returns another Variable, so you can chain operations together:

```
set a to 1
set b to a + 1
set c to a + b + 1
```

There are a few types of Operations: "Uni" (meaning 1 operand), "Bi" (meaning takes 2 operands), and even a "Ternary" operand that takes 3.  UniOperations include operations like "not", "abs", "sin", "cos", "tan", etc.  BiOperations include things like "+", "-", "*", "/", "%", etc.  Note that some UniOperands expect the operand to preceed the operations (such as "keys", and "values")

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

All UniOperand Operations are on tier 1.  Here are the tiers for BiOperand Operations
Tier 1 BiOperand Operations: ```*, /, %, ., cast, ^```
Tier 2 BiOperand Operations: ```+, -```
Tier 3 BiOperand Operations: ```..```

### Changing Order of Operations
To change the order of operations, use parentheses ```()```.  Anything  in parentheses is evaluated first.  Order of operations within the parantheses follows the same rules as outside.  You can nest parentheses to further change order of operations.

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

## Uni Operations

### Absolute Value
Behavior varies based on input type.

Keywords: ```abs, absolute```

**(Number)**: Absolute value of the number
**(Vector)**: Returns the length of the vector

### Arc Cosine
Performs the arc cosine operation on the given numeric value

Keywords: ```acos, arccos```

### Arc Sine
Performs the arc sine operation on the given numeric value

Keywords: ```asin, arcsin```

### Arc Tangent
Performs the arc tan operation on the given numeric value

Keywords: ```atan, arctan```

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

### Not
Inverses a given property.  Behavior varies by input type

Keywords:
```not, !, isnt, arent, stop```

**(Boolean)**: inverts the boolean
**(Number)**: multiplies the number by -1
**(Vector)**: returns the inverse of the given vector
**(Color)**: returns the inverse of the given color

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

### Reverse
Reverses the given list

Keywords: ```reversed```
```
set myList to ["one", "two", "three"]
set myReversedList to reversed myList
```

### Round
Rounds the given number to the nearest integer (half-up)

Keywords: ```round, rnd```

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
###Values (After)
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

**(String, Any)**: Concatenates the first string with the second variable after converting the second variable to a string.
**(Any, String)**: Concatenates the first variable (as a string) with the second string
**(Number, Number)**: Adds two numbers together, returning a number
**(Vector, Vector)**: Performs Vector Addition on the two vectors
**(Vector, Number)**: Returns a Vector in the same direction as "a" whose length has been increased by "b" amount
**(Color, Color)**: Adds two colors together by adding together RGB values (capped at 255, of course)
**(List, Any)**: Adds the second item(s) to the first list, at the end
**(Any, List)**: Adds the first item(s) to the second list, inserted at the beginning

### And
Checks whether both boolean operands are true (a && b)

Keywords:
```
and
&
&&
but
yet
```

### Cast

This special operation allows you to cast a given value as another value.  This enables you to construct something using a string, and then convert it to a vector or color, for example.

Keywords: ```cast, as```

Cast Types: ```bool, string, number, vector, color, list```

```
set myVector to 1 + ":" + 2 + ":" + 3
set myVector to myVector as vector
set the "Remote Control" destination to myVector
```

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

**(String,Any)**: Converts the second operand to a String and then checks if the first operand contains the second operand
**(Collection, Any)**: Checks whether a contains b (or all the elements of b if b is a collection)

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

## Division
Behavior varies based on input types.

Keywords: ```/, divide```

**(Number, Number)**: Divides the first number by the second
**(Vector, Vector)**: Divides the first vector by the length of the second vector, returning a Vector
**(Vector, Number)**: Divides the vector by the Number, returning a Vector in the same direction as a with a magnitude reduced by a factor of b.
**(Color, Number)**: Divides the color by the Number, returning a Color

## Dot Product
Returns the dot product of the given two vectors.

Keywords: ```.```

## Exponent (Also XOR)
Behavior varies based on the input types.

Keywords: ```^, pow, exp, xor```

**(Bool, Bool**): Performs the XOR operation on the two given bools.
**(Number, Number)**: Raises the first number to t`e power of the second number
**(Vector, Vector)**: Returns the angle (in degrees) between the two given vectors (get it? ```^```)

## Modulus
Behavior varies based on input types.

Keywords: ```%, mod```
**(Number, Number)**: Returns a % b, meaning return the remainder of a / b
**(String, String)**: Removes all instances of b from the string a
**(Vector, Vector)**: Performed Vector Rejection of b on the given Vector A, returning the resulting Vector.

## Multiplication
Behavior varies based on input types

Keywords: ```*, multiply```

**(Number, Number)**: Multiplies a by b
**(Vector, Vector)**: Returns the Vector Cross Product of a and b (order matters!). 
**(Vector, Number)**: Returns a Vector in the same direction as a with the magnitude multiplied by b
**(Number, Vector)**: Returns a Vector in the same direction as b with the magnitude multiplied by a
**(Color, Number)**: Returns a Color multiplied by the factor of b
**(Number, Color)**: Returns b Color multiplied by the factor of a

## Or
Checks whether either boolean operand is true (a || b).

Keywords:
```
or
|
||
```

## Range
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

## Subtraction
Behavior varies based on input types.

Keywords: ```-, minus```

**(Number, Number)**: Returns a - b
**(String, String)**: Returns a copy of "a" with the first instance of b removed.
**(String, Number)**: Returns a with the last b characters removed.  if b >= a.length, returns an empty string.
**(Vector, Number)**: Returns a copy of the vector with it's magnitude reduced by b amount.
**(Vector, Vector)**: Returns the vector produced by a - b
**(Color, Color)**: Returns the resulting color from subtract b from a
