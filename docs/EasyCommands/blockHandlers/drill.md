# Decoy Block Handler
This Block Handler handles Decoys, which will automatically attract enemy fire when enabled.

* Block Type Keywords: ```decoy```
* Block Type Group Keywords: ```decoys```

Default Primitive Properties:
* String - Name
* Bool - Enabled

## "Enabled" Property
* Primitive Type: Bool
* Keywords: ```enable, enabled```
* Inverse Keywords: ```disable, disabled```

Enables or Disables the given block

```
#Enable Decoy
enable "My Decoy"
set "My Decoy" to enabled
turn on "My Decoy"

#Disable Block
disable "My Decoy"
set "My Decoy" to disabled
turn off "My Decoy"
```

## "Power" Property
* Primitive Type: Bool
* Keywords: ```power, powered```

Turns on or off power to the block.  Effectively the same as the Enabled property.

```
#Turn on
turn on power to "My Decoy"
power on "My Decoy"

#Turn off
turn off "My Decoy"
power off "My Decoy"
```