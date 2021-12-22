# Refinery Block Handler
This Block Handler handles Refineries, which are used to process ores quickly and efficiently.  This BlockHandler works for both Basic and regular Refineries.

* Block Type Keywords: ```refinery```
* Block Type Group Keywords: ```refineries```

Default Primitive Properties:
* String - Name
* Bool - Enabled

## "Enabled" Property
* Primitive Type: Bool
* Keywords: ```enable, enabled```
* Inverse Keywords: ```disable, disabled```

Enables or Disables the given block

```
#Enable Refinery
enable "My Refinery"
set "My Refinery" to enabled
turn on "My Refinery"

#Disable Block
disable "My Refinery"
set "My Refinery" to disabled
turn off "My Refinery"
```

## "Power" Property
* Primitive Type: Bool
* Keywords: ```power, powered```

Turns on or off power to the block.  Effectively the same as the Enabled property.

```
#Turn on
turn on power to "My Refinery"
power on "My Refinery"

#Turn off
turn off "My Refinery"
power off "My Refinery"
```