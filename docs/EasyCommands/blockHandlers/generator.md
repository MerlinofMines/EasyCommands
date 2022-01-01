# O2/H2 Generator Block Handler
This Block Handler handles O2/H2 Generators.  It enables you to turn them on or off, and set them to auto-refill bottles or not.

* Block Type Keywords: ```generator```
* Block Type Group Keywords: ```generators```

Default Primitive Properties:
* Bool - Auto
* Numeric - Ratio

Default Directional Properties
* Up - Ratio

## "Enabled" Property
* Primitive Type: Bool
* Keywords: ```enable, enabled```
* Inverse Keywords: ```disable, disabled```

Enables or Disables the given block

```
#Enable Block
enable "My Generator"
set "My Generator" to enabled
turn on "My Generator"

#Disable Block
disable "My Generator"
set "My Generator" to disabled
turn off "My Generator"
```

## "Power" Property
* Primitive Type: Bool
* Keywords: ```power, powered```

Turns on or off power to the block.  Effectively the same as the Enabled property.

```
#Turn on
turn on power to "My Generator"
power on "My Generator"

#Turn off
turn off "My Generator"
power off "My Generator"
```

## "Auto" Property
* Primitive Type: Bool
* Keywords: ```auto, refill```

Gets/Sets whether the Generator is set to auto-refill bottles.
