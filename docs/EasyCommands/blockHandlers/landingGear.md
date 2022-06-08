﻿# Landing Gear Block Handler
This Block Handler handles Landing Gear and Magnets.  It can be used to connect/disconnect them, and set/unset them to auto lock.

* Block Type Keywords: ```gear, magnet```
* Block Type Group Keywords: ```gear, gears, magnets```
* Note: ```gear``` is ambiguous and can work for either a block or a group

Default Primitive Properties:
* Bool - Enabled

## "Enabled" Property
* Primitive Type: Bool
* Keywords: ```enable, enabled```
* Inverse Keywords: ```disable, disabled```

Enables or Disables the given block

```
#Enable Block
enable "My Landing Gear"
set "My Landing Gear" to enabled
turn on "My Landing Gear"

#Disable Block
disable "My Landing Gear"
set "My Landing Gear" to disabled
turn off "My Landing Gear"
```

## "Power" Property
* Primitive Type: Bool
* Keywords: ```power, powered```

Turns on or off power to the block.  Effectively the same as the Enabled property.

```
#Turn on
turn on power to "My Landing Gear"
power on "My Landing Gear"

#Turn off
turn off "My Landing Gear"
power off "My Landing Gear"
```

## "Lock" Property
* Primitive Type: Bool
* Keywords: ```lock, locked```
* Inverse Keywords: ```unlock, unlocked```

Gets/Sets whether the Landing Gear / Magnet is locked. When setting, only attempts to lock once.  If set to a false value, will disconnect.

```
#Keep trying to lock until locked
until "My Landing Gear" is locked
  lock "My Landing Gear"

#Disconnect
unlock "My Landing Gear"
```

## "Connect" Property
* Primitive Type: Bool
* Keywords: ```connect, connected, attach, attached, dock, docked```
* Inverse Keywords: ```disconnect, disconnected, detach, detached, undock, undocked```

Same as Locked. Gets/Sets whether the Landing Gear / Magnet is locked. When setting, only attempts to lock once.  If set to a false value, will disconnect.

```
#Keep trying to lock until connected
until "My Landing Gear" is connected
  connect "My Landing Gear"

#Disconnect
disconnect "My Landing Gear"
```

## "Ready" Property
* Primitive Type: Bool
* Read-only
* Keywords: ```ready, able```
* Inverse Keywords: ```unable```

Returns true if the landing gear is ready to connect, false if already connected or not ready to connect.

```
if "My Landing Gear" is ready
  Print "Ready to Connect!"
```

## "Ready Connect" Property
* Primitive Type: Bool
* Read-only
* Keywords: ```ready to connect, able to connect, can connect```
* Inverse Keywords: ```unable to connect, cannot connect```

Same as Ready. Returns true if the landing gear is ready to connect, false if already connected or not ready to connect.

```
if "My Landing Gear" is ready to connect
  Print "Ready to Connect!"
```

## "Ready Lock" Property
* Primitive Type: Bool
* Read-only
* Keywords: ```ready to lock, able to lock, can lock```
* Inverse Keywords: ```unable to lock, cannot lock```

Same as Ready. Returns true if the landing gear is ready to connect, false if already connected or not ready to connect.

```
if "My Landing Gear" is ready to lock
  Print "Ready to Connect!"
```

## "Auto" Property
* Primitive Type: Bool
* Keywords: ```auto```

Gets/Sets whether the Landing Gear/Magnet will auto lock.

```
if "My Landing Gear" is on auto
  Print "Landing Gear will auto connect"

set "My Landing Gear" to auto

#Turn off Auto lock
turn "My Landing Gear" auto off
```