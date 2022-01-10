# Thruster Block Handler
This Block Handler governs all thruster types (Atmospheric, Ion, Hydrogen).

* Block Type Keywords: ```thruster```
* Block Type Group Keywords: ```thrusters```

Default Primitive Properties:
* Bool - Enabled
* Numeric - Level

Default Directional Properties
* Up - Limit

## "Enabled" Property
* Primitive Type: Bool
* Keywords: ```enable, enabled```
* Inverse Keywords: ```disable, disabled```

Enables or Disables the given block

```
#Enable Block
enable "My Thruster"
set "My Thruster" to enabled
turn on "My Thruster"

#Disable Block
disable "My Thruster"
set "My Thruster" to disabled
turn off "My Thruster"
```

## "Power" Property
* Primitive Type: Bool
* Keywords: ```power, powered```

Turns on or off power to the block.  Effectively the same as the Enabled property.

```
#Turn on
turn on power to "My Thruster"
power on "My Thruster"

#Turn off
turn off "My Thruster"
power off "My Thruster"
```

## "Limit" Property
* Primitive Type: Numeric
* Keywords: ```limit, limits, range, ranges```

Gets/Sets the Maximum Thrust Limit for the thruster, in Newtons.  When retrieving, returns the maximum possible thrust.

When setting, this sets a thrust override to the supplied value.

```
Print "Max Thrust: " + "My Thruster" limit

#10000 Newtons
set "My Thruster" limit to 10000
```

## "Level" Property
* Primitive Type: Numeric
* Keywords: ```level, levels```

Gets/Sets the current thrust from the thruster, in Newtons.

When setting, if the value is set to 0 then thrust overrides are turned off.

```
Print "Current Thrust: " + "My Thruster" level

#Override thrust to 10000 Newtons
set "My Thruster" level to 10000
```

## "Output" Property
* Primitive Type: Numeric
* Keywords: ```output, outputs```

Same as Level property.  Gets/Sets the current thrust from the thruster, in Newtons.

When setting, if the value is set to 0 then thrust overrides are turned off.

```
Print "Current Output: " + "My Thruster" output

#Override thrust to 10000 Newtons
set "My Thruster" output to 10000
```

## "Override" Property
* Primitive Type: Numeric
* Keywords: ```override, overrides```

Gets/Sets the ThrustOverride for the thruster, in Newtons.  Returns 0 if no thrust override is set.  If set to 0, turns off Thrust Overrides.

```
Print "Thrust Override: " + "My Thruster" override

#Override thrust to 10000 Newtons
set "My Thruster" override to 10000
```

## "Ratio" Property
* Primitive Type: Numeric
* Keywords: ```ratio, ratios, percentage, percentages, percent, percents```

Gets/Sets the current thrust override, as a percentage.  Values are expected to be between 0 - 1, with 1 = 100% Thrust Override.

Setting to 0 turns off thrust overrides.

```
Print "Thrust Percentage: " + "My Thrust" percentage

#50% power
set "My Thruster" percent to 0.5
```
