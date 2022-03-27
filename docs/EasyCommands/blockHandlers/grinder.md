# Grinder Block Handler
This Block Handler handles Grinders, which can be used to grind down ships and other blocks quickly.

* Block Type Keywords: ```grinder```
* Block Type Group Keywords: ```grinders```

Default Primitive Properties:
* Bool - Enabled

## "Enabled" Property
* Primitive Type: Bool
* Keywords: ```enable, enabled```
* Inverse Keywords: ```disable, disabled```

Enables or Disables the given block

```
#Enable Grinder
enable "My Grinder"
set "My Grinder" to enabled
turn on "My Grinder"

#Disable Block
disable "My Grinder"
set "My Grinder" to disabled
turn off "My Grinder"
```

## "Power" Property
* Primitive Type: Bool
* Keywords: ```power, powered```

Turns on or off power to the block.  Effectively the same as the Enabled property.

```
#Turn on
turn on power to "My Grinder"
power on "My Grinder"

#Turn off
turn off "My Grinder"
power off "My Grinder"
```