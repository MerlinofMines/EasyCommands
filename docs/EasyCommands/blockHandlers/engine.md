# Hydrogen Engine Block Handler

This Block Handler handles Hydrogen Engines.  It can be used to turn on/off the Hydrogen Engine and get information about its energy output 

* Block Type Keywords: ```engine```
* Block Type Group Keywords: ```engines```

Default Primitive Properties:
* Bool - Enabled

## "Enabled" Property
* Primitive Type: Bool
* Keywords: ```enable, enabled```
* Inverse Keywords: ```disable, disabled```

Enables or Disables the given block

```
#Enable Block
enable "My Hydrogen Engine"
set "My Hydrogen Engine" to enabled
turn on "My Hydrogen Engine"

#Disable Block
disable "My Hydrogen Engine"
set "My Hydrogen Engine" to disabled
turn off "My Hydrogen Engine"
```

## "Power" Property
* Primitive Type: Bool
* Keywords: ```power, powered```

Turns on or off power to the block.  Effectively the same as the Enabled property.

```
#Turn on
turn on power to "My Hydrogen Engine"
power on "My Hydrogen Engine"

#Turn off
turn off "My Hydrogen Engine"
power off "My Hydrogen Engine"
```

## "Output" Property
* Read-only
* Primitive Type: Numeric
* Keywords: ```output, outputs```

Gets the current energy output, in MW.

```
Print "Energy Output: " + "My Hydrogen Engine" output
```

## "Limit" Property
* Read-only
* Primitive Type: Numeric
* Keywords: ```limit, limits```

Gets the maximum energy output the hydrogen engine is capable of, in MW.

```
Print "Maximum Output: " + "My Hydrogen Engine" limit
```

## "Ratio" Property
* Read-only
* Primitive Type: Numeric
* Keywords: ```ratio, ratios, percent, percents, percentage, percentages```

Returns a value between 0 - 1 representing the current output over the maximum output.  For Hydrogen Engines, will typically be 0 if off or unfueled and 1 if on and fueled.

```
Print "Output Ratio: " + "My Hydrogen Engine" ratio
```
