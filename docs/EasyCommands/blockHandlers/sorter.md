# Sorter Block Handler
This Block Handler gives you some basic control over Sorter Blocks.

* Block Type Keywords: ```sorter```
* Block Type Group Keywords: ```sorters```

Default Primitive Properties:
* Bool - Enabled

## "Enabled" Property
* Primitive Type: Bool
* Keywords: ```enable, enabled```
* Inverse Keywords: ```disable, disabled```

Enables or Disables the given block

```
#Enable Block
enable "My Sorter"
set "My Sorter" to enabled
turn on "My Sorter"

#Disable Block
disable "My Sorter"
set "My Sorter" to disabled
turn off "My Sorter"
```

## "Power" Property
* Primitive Type: Bool
* Keywords: ```power, powered```

Turns on or off power to the block.  Effectively the same as the Enabled property.

```
#Turn on
turn on power to "My Sorter"
power on "My Sorter"

#Turn off
turn off "My Sorter"
power off "My Sorter"
```

## "Drain" Property
* Primitive Type: Bool
* Keywords: ```drain, draining, auto```

Get/Sets whether the sorter is auto-draining.

```
if "My Sorter" is draining
  print "Pulling in items, sir"

tell "My Sorter" to drain

#Stop draining
tell "My Sorter" to stop draining
```
