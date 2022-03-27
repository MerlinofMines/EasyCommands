# Collector Block Handler
This Block Handler handles Collectors which can automatically grab floating objects and place them into inventories.  Good for trash compactors, etc.

* Block Type Keywords: ```collector```
* Block Type Group Keywords: ```collectors```

Default Primitive Properties:
* Bool - Enabled

## "Enabled" Property
* Primitive Type: Bool
* Keywords: ```enable, enabled```
* Inverse Keywords: ```disable, disabled```

Enables or Disables the given block

```
#Enable Collector
enable "My Collector"
set "My Collector" to enabled
turn on "My Collector"

#Disable Block
disable "My Collector"
set "My Collector" to disabled
turn off "My Collector"
```

## "Power" Property
* Primitive Type: Bool
* Keywords: ```power, powered```

Turns on or off power to the block.  Effectively the same as the Enabled property.

```
#Turn on
turn on power to "My Collector"
power on "My Collector"

#Turn off
turn off "My Collector"
power off "My Collector"
```