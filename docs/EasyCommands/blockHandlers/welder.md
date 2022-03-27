# Welder Block Handler
This Block Handler handles Grinders, which can be used to create ships, bases, and other creations quickly.

* Block Type Keywords: ```welder```
* Block Type Group Keywords: ```welders```

Default Primitive Properties:
* Bool - Enabled

## "Enabled" Property
* Primitive Type: Bool
* Keywords: ```enable, enabled```
* Inverse Keywords: ```disable, disabled```

Enables or Disables the given block

```
#Enable Welder
enable "My Welder"
set "My Welder" to enabled
turn on "My Welder"

#Disable Block
disable "My Welder"
set "My Welder" to disabled
turn off "My Welder"
```

## "Power" Property
* Primitive Type: Bool
* Keywords: ```power, powered```

Turns on or off power to the block.  Effectively the same as the Enabled property.

```
#Turn on
turn on power to "My Welder"
power on "My Welder"

#Turn off
turn off "My Welder"
power off "My Welder"
```