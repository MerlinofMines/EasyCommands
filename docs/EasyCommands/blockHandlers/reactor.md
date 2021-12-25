# Reactor Block Handler

This Block Handler handles Reactors.  It can be used to turn on/off the Reactor and get information about its energy output 

* Block Type Keywords: ```reactor```
* Block Type Group Keywords: ```reactors```

Default Primitive Properties:
* Bool - Enabled

## "Enabled" Property
* Primitive Type: Bool
* Keywords: ```enable, enabled```
* Inverse Keywords: ```disable, disabled```

Enables or Disables the given block

```
#Enable Block
enable "My Reactor"
set "My Reactor" to enabled
turn on "My Reactor"

#Disable Block
disable "My Reactor"
set "My Reactor" to disabled
turn off "My Reactor"
```

## "Power" Property
* Primitive Type: Bool
* Keywords: ```power, powered```

Turns on or off power to the block.  Effectively the same as the Enabled property.

```
#Turn on
turn on power to "My Reactor"
power on "My Reactor"

#Turn off
turn off "My Reactor"
power off "My Reactor"
```

## "Output" Property
* Read-only
* Primitive Type: Numeric
* Keywords: ```output, outputs```

Gets the current energy output, in MW.

```
Print "Energy Output: " + "My Reactor" output
```

## "Limit" Property
* Read-only
* Primitive Type: Numeric
* Keywords: ```limit, limits```

Gets the maximum energy output the reactor is capable of, in MW.

```
Print "Maximum Output: " + "My Reactor" limit
```

## "Ratio" Property
* Read-only
* Primitive Type: Numeric
* Keywords: ```ratio, ratios, percent, percents, percentage, percentages```

Returns a value between 0 - 1 representing the current output over the maximum output. 

```
Print "Output Ratio: " + "My Reactor" ratio
```
