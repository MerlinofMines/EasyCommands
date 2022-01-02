# Merge Block Handler
This Block handler will let you connect and disconnect Merge Blocks.

* Block Type Keywords: ```merge```

There are no Block Type Group Keywords for merge blocks, but you can use "blocks" to indicate a group:
```
#Group called "My Merge Blocks"
turn on "My Merge Blocks"
```

Default Primitive Properties:
* Bool - Enabled

## "Enabled" Property
* Primitive Type: Bool
* Keywords: ```enable, enabled```
* Inverse Keywords: ```disable, disabled```

Enables or Disables the given block

```
#Enable Block
enable "My Merge Block"
set "My Merge Block" to enabled
turn on "My Merge Block"

#Disable Block
disable "My Merge Block"
set "My Merge Block" to disabled
turn off "My Merge Block"
```

## "Power" Property
* Primitive Type: Bool
* Keywords: ```power, powered```

Turns on or off power to the block.  Effectively the same as the Enabled property.

```
#Turn on
turn on power to "My Merge Block"
power on "My Merge Block"

#Turn off
turn off "My Merge Block"
power off "My Merge Block"
```

## "Locked" Property
* Primitive Type: Bool
* Keywords: ```lock, locked```
* Inverse Keywords: ```unlock, unlocked```

Returns whether the merge block is currently connected to another merge block. This property is read-only.  When set, this property enables or disables the merge block (the only way to connect/disconnect).

```
if "My Merge Block" is locked
  Print "Merge block is locked"

#Enables the merge block
lock "My Merge Block"

#Disables the merge block
unlock "My Merge Block"
```

## "Connected" Property
* Primitive Type: Bool
* Keywords: ```connected, attached, docked, joined```
* Inverse Keywords: ```disconnected, detached, undocked, separated```

Same as Locked.  Returns whether the merge block is currently connected to another merge block.  This property is read-only.  When set, this property enables or disables the merge block (the only way to connect/disconnect).

```
if "My Merge Block" is connected
  Print "Merge block is connected"

#Enables the merge block
connect "My Merge Block"

#Disables the merge block
disconnect "My Merge Block"
```
