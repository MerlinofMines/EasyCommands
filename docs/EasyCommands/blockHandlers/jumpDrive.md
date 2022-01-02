# Jump Drive Block Handler
This Block Handler handles Jump Drives.  You aren't able to automatically jump (not supported by SE) but you can use it to get some information about possible jumps.

Note: These properties need to be refreshed so that "limit" references the jump limit, not power limits.  Stay tuned for updated properties on this block handler.

* Block Type Keywords: ```jumpdrive```
* Block Type Group Keywords: ```jumpdrives```

Default Primitive Properties:
* Bool - Power

## "Enabled" Property
* Primitive Type: Bool
* Keywords: ```enable, enabled```
* Inverse Keywords: ```disable, disabled```

Enables or Disables the given block

```
#Enable Block
enable "My JumpDrive"
set "My JumpDrive" to enabled
turn on "My JumpDrive"

#Disable Block
disable "My JumpDrive"
set "My JumpDrive" to disabled
turn off "My JumpDrive"
```

## "Power" Property
* Primitive Type: Bool
* Keywords: ```power, powered```

Turns on or off power to the block.  Effectively the same as the Enabled property.

Note: This property will be updated in a future release to reference the current stored power instead.

```
#Turn on
turn on power to "My JumpDrive"
power on "My JumpDrive"

#Turn off
turn off "My JumpDrive"
power off "My JumpDrive"
```


## "Limit" Property
* Read-only
* Primitive Type: Numeric
* Keywords: ```limit, limits```

Gets/Sets the current max stored power for this Jump Drive.

Note: This will be changed in a future update to reference the jump limits instead.

```
Print "Max Stored Power: " + "My JumpDrive" limit
```

## "Level" Property
* Read-only
* Primitive Type: Numeric
* Keywords: ```level, levels```

Gets the current stored power on the Jump Drive.

```
Print "Stored Power: " + "My Jumpdrive" level
```

## "Ratio" Property
* Read-only
* Primitive Type: Numeric 
* Keywords: ```ratio, ratios, percentage, percentages, percent, percents```

Gets the current ratio of stored power to max power.

```
Print "Power Ratio: " + "My JumpDrive" ratio
```

## "Complete" Property
* Read-only
* Primitive Type: Bool
* Keywords: ```complete, ready, done, finished```

Returns whether the Jump Drive is ready to jump.  Specifically, whether the Jump Drive's status is READY.

```
if "My JumpDrive" is ready
  Print "Ready to Jump!"
```