# Ejector Block Handler
This Block Handler handles Ejectors, which you can use to automatically eject items out of connected inventories.

* Block Type Keywords: ```ejector```
* Block Type Group Keywords: ```ejectors```

## "Enabled" Property
* Primitive Type: Bool
* Keywords: ```enable, enabled```
* Inverse Keywords: ```disable, disabled```

Enables or Disables the given block

```
#Enable Block
enable "My Ejector"
set "My Ejector" to enabled
turn on "My Ejector"

#Disable Block
disable "My Ejector"
set "My Ejector" to disabled
turn off "My Ejector"
```

## "Power" Property
* Primitive Type: Bool
* Keywords: ```power, powered```

Turns on or off power to the block.  Effectively the same as the Enabled property.

```
#Turn on
turn on power to "My Ejector"
power on "My Ejector"

#Turn off
turn off "My Ejector"
power off "My Ejector"
```

## "Drain" Property
* Primitive Type: Bool
* Keywords: ```drain, draining```

Gets/Sets whether the ejector is throwing out items.

```
if "My Ejector" is draining
  Print "Throwing out the garbage"

tell "My Ejector" to drain
drain "My Ejector"
```

## "Collect" Property
* Primitive Type: Bool
* Keywords: ```collect, collecting, consume, consuming, gather, gathering```

Gets/Sets whether the ejector is automatically pulling items from connected inventories (acting like a Sorter, but without a filter).

```
if "My Ejector" is collecting
  Print "Grabbing all the items."

tell "My Ejector" to gather
tell "My Ejector" to stop collecting
```
