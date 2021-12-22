# Drill Block Handler
This Block Handler handles Drills, which can be used to gather ores and other raw materials quickly.

* Block Type Keywords: ```drill```
* Block Type Group Keywords: ```drills```

Default Primitive Properties:
* String - Name
* Bool - Enabled

## "Enabled" Property
* Primitive Type: Bool
* Keywords: ```enable, enabled```
* Inverse Keywords: ```disable, disabled```

Enables or Disables the given block

```
#Enable Drill
enable "My Drill"
set "My Drill" to enabled
turn on "My Drill"

#Disable Block
disable "My Drill"
set "My Drill" to disabled
turn off "My Drill"
```

## "Power" Property
* Primitive Type: Bool
* Keywords: ```power, powered```

Turns on or off power to the block.  Effectively the same as the Enabled property.

```
#Turn on
turn on power to "My Drill"
power on "My Drill"

#Turn off
turn off "My Drill"
power off "My Drill"
```