# Spherical Gravity Generator Block Handler
This Block Handler controls Spherical Gravity Generators, which enable you to get/set artificial gravity.

Note that there is a separate block handler for [Gravity Generators](https://spaceengineers.merlinofmines.com/EasyCommands/blockHandlers/gravityGenerator "Gravity Generators").  This block handler cannot be used for Gravity Generators.

* Block Type Keywords: ```gravitySphere```
* Block Type Group Keywords: ```gravitySpheres```

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
enable "My Gravity Generator" gravitySphere
set "My Gravity Generator" gravitySphere to enabled
turn on "My Gravity Generator" gravitySphere

#Disable Block
disable "My Gravity Generator" gravitySphere
set "My Gravity Generator" gravitySphere to disabled
turn off "My Gravity Generator" gravitySphere
```

## "Power" Property
* Primitive Type: Bool
* Keywords: ```power, powered```

Turns on or off power to the block.  Effectively the same as the Enabled property.

```
#Turn on
turn on power to "My Gravity Generator" gravitySphere
power on "My Gravity Generator" gravitySphere

#Turn off
turn off "My Gravity Generator"
power off "My Gravity Generator"
```

## "Strength" Property
* Primitive Type: Numeric
* Keywords: ```strength, strengths, gravity, gravities, force, forces```

Gets/Sets the Gravitation Strength of the Gravity Generator, in m/s^2

```
Print "Gravity: " + "My Gravity Generator" gravitySphere strength

set "My Gravity Generator" gravitySphere strength to 5
```

## "Range" Property
* Primitive Type: Numeric
* Keywords: ```range, ranges, limit, limits, distance, distances, radius, radii```

This property Gets/Sets the Range of the Gravity Generator, in meters.

```
Print "Gravitation Field Limit: " + "Test Gravity Generator" gravitySphere limit

set "Test Gravity Generator" gravitySphere limit to 3
```

## "Size" Property
* Primitive Type: Numeric
* Keywords: ```size, sizes, height, heights, length, lengths```

Identical to the "Range" property.  This property Gets/Sets the Field Size of the Gravity Generator, in meters.

```
Print "Gravitation Field Size: " + "Test Gravity Generator" gravitySphere size

set "Test Gravity Generator" gravitySphere size to 3
```
