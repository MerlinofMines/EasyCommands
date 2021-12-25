# Wind Turbine Block Handler

This Block Handler handles Wind Turbines.  It can be used to turn on/off the Wind Turbine and get information about its energy output 

* Block Type Keywords: ```turbine```
* Block Type Group Keywords: ```turbines```

Default Primitive Properties:
* Bool - Enabled

## "Enabled" Property
* Primitive Type: Bool
* Keywords: ```enable, enabled```
* Inverse Keywords: ```disable, disabled```

Enables or Disables the given block

```
#Enable Block
enable "My Wind Turbine"
set "My Wind Turbine" to enabled
turn on "My Wind Turbine"

#Disable Block
disable "My Wind Turbine"
set "My Wind Turbine" to disabled
turn off "My Wind Turbine"
```

## "Power" Property
* Primitive Type: Bool
* Keywords: ```power, powered```

Turns on or off power to the block.  Effectively the same as the Enabled property.

```
#Turn on
turn on power to "My Wind Turbine"
power on "My Wind Turbine"

#Turn off
turn off "My Wind Turbine"
power off "My Wind Turbine"
```

## "Output" Property
* Read-only
* Primitive Type: Numeric
* Keywords: ```output, outputs```

Gets the current energy output, in MW.

```
Print "Energy Output: " + "My Wind Turbine" output
```

## "Limit" Property
* Read-only
* Primitive Type: Numeric
* Keywords: ```limit, limits```

Gets the maximum energy output the wind turbine is capable of, in MW.

```
Print "Maximum Output: " + "My Wind Turbine" limit
```

## "Ratio" Property
* Read-only
* Primitive Type: Numeric
* Keywords: ```ratio, ratios, percent, percents, percentage, percentages```

Returns a value between 0 - 1 representing the current output over the maximum output. 

```
Print "Output Ratio: " + "My Wind Turbine" ratio
```
