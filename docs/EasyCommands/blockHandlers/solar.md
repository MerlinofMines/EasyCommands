# Solar Panel Block Handler

This Block Handler handles Solar Panels.  It can be used to turn on/off the Solar Panel and get information about its energy output 

* Block Type Keywords: ```solar```

While there is no Block Type Group word for Soloar Panels, the keyword "panels" implies you are referring to a group.  So the following will turn on all Solar Panels in the "My Solar Panels" group.

```
turn on "My Solar Panels" 
```

Default Primitive Properties:
* Bool - Enabled

## "Enabled" Property
* Primitive Type: Bool
* Keywords: ```enable, enabled```
* Inverse Keywords: ```disable, disabled```

Enables or Disables the given block

```
#Enable Block
enable "My Solar Panel"
set "My Solar Panel" to enabled
turn on "My Solar Panel"

#Disable Block
disable "My Solar Panel"
set "My Solar Panel" to disabled
turn off "My Solar Panel"
```

## "Power" Property
* Primitive Type: Bool
* Keywords: ```power, powered```

Turns on or off power to the block.  Effectively the same as the Enabled property.

```
#Turn on
turn on power to "My Solar Panel"
power on "My Solar Panel"

#Turn off
turn off "My Solar Panel"
power off "My Solar Panel"
```

## "Output" Property
* Read-only
* Primitive Type: Numeric
* Keywords: ```output, outputs```

Gets the current energy output, in MW.

```
Print "Energy Output: " + "My Solar Panel" output
```

## "Limit" Property
* Read-only
* Primitive Type: Numeric
* Keywords: ```limit, limits```

Gets the maximum energy output the solar is capable of, in MW.

```
Print "Maximum Output: " + "My Solar Panel" limit
```

## "Ratio" Property
* Read-only
* Primitive Type: Numeric
* Keywords: ```ratio, ratios, percent, percents, percentage, percentages```

Returns a value between 0 - 1 representing the current output over the maximum output. 

```
Print "Output Ratio: " + "My Solar Panel" ratio
```
