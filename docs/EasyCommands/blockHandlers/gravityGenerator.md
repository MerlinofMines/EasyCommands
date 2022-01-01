# Gravity Generator Block Handler
This Block Handler controls Gravity Generators, which enable you to get/set artificial gravity.

Note that there is a separate block handler for [Spherical Gravity Generators](https://spaceengineers.merlinofmines.com/EasyCommands/blockHandlers/gravitySphere "Spherical Gravity Generators").  This block handler cannot be used for Spherical Gravity Generators.

* Block Type Keywords: ```gravityGenerator```
* Block Type Group Keywords: ```gravityGenerators```

Default Primitive Properties:
* Bool Enabled
* Numeric - Strength
* Vector - Range

## "Enabled" Property
* Primitive Type: Bool
* Keywords: ```enable, enabled```
* Inverse Keywords: ```disable, disabled```

Enables or Disables the given block

```
#Enable Block
enable "My GravityGenerator"
set "My GravityGenerator" to enabled
turn on "My GravityGenerator"

#Disable Block
disable "My GravityGenerator"
set "My GravityGenerator" to disabled
turn off "My GravityGenerator"
```

## "Power" Property
* Primitive Type: Bool
* Keywords: ```power, powered```

Turns on or off power to the block.  Effectively the same as the Enabled property.

```
#Turn on
turn on power to "My GravityGenerator"
power on "My GravityGenerator"

#Turn off
turn off "My GravityGenerator"
power off "My GravityGenerator"
```

## "Strength" Property
* Primitive Type: Numeric
* Keywords: ```strength, strengths, gravity, gravities, force, forces```

Gets/Sets the Gravitation Strength of the Gravity Generator, in m/s^2

```
Print "Gravity: " + "My GravityGenerator" strength

set "My GravityGenerator" strength to 5
```

## "Range" Property
* Primitive Type: Vector / Numeric
* Supports Directions (Left, Right, Up, Down, Forward, Backward)
* Keywords: ```range, ranges, limit, limits, distance, distances```

This property Gets/Sets the Field Size of the Gravity Generator, in meters.

If a direction is included, this property expects a numeric value for the field size in that direction.
* Left/Right = FieldSize.X
* Up/Down - FieldSize.Y
* Forward/Backwards - FieldSize.Z

If a direction is not included, this property will return and expect a Vector of the form X:Y:Z where X,Y,Z relate to the Field Size's X,Y,Z

If you set this property to a numeric value without supplying a direction, all 3 FieldSize values are set to given value.

Note that the field size in any given opposite directions is always the same, so if you set the fowards distance to 10, the backwards distance will also be set to 10, and vice versa.

```
set myGravityField to "Test Gravity Generator" gravityGenerator limits

Print "Gravitation Field: " + myGravityField
Print "x: " + myGravityField.x
Print "y: " + myGravityField.y
Print "z: " + myGravityField.z

Print "Left Gravitational Limit: " + "Test Gravity Generator" gravityGenerator left limit

#Set Field Limits to x=1, y=2, z=3
set "Test Gravity Generator" gravityGenerator limits to 1:2:3

#Set Left/Right Limit to 4
set "Test Gravity Generator" gravityGenerator left limit to 4
```
