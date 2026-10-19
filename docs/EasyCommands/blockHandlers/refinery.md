# Refinery Block Handler
This Block Handler handles Refineries, which are used to process ores quickly and efficiently.  This BlockHandler works for both Basic and regular Refineries.

* Block Type Keywords: ```refinery```
* Block Type Group Keywords: ```refineries```

Default Primitive Properties:
* Bool - Enabled

## "Running" Property
* Primitive Type: Bool (read-only)
* Keywords: ```run, running```

Returns true only when the refinery is working and actively producing. Disabled
or unpowered refineries report false even if the game retains its previous
production flag. This matches the [assembler](assembler) `running` property.
Assembler queue commands are not available on refineries.

```
print "My Refinery" refinery is running
```

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
