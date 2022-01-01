# Projector Block Handler
This Block Handler gives you basic information on Projectors such as the build % and if it is complete.

Support for getting/setting the offsets and rotation will be added in a future release.

* Block Type Keywords: ```projector```
* Block Type Group Keywords: ```projectors```

## "Enabled" Property
* Primitive Type: Bool
* Keywords: ```enable, enabled```
* Inverse Keywords: ```disable, disabled```

Enables or Disables the given block

```
#Enable Block
enable "My Projector"
set "My Projector" to enabled
turn on "My Projector"

#Disable Block
disable "My Projector"
set "My Projector" to disabled
turn off "My Projector"
```

## "Power" Property
* Primitive Type: Bool
* Keywords: ```power, powered```

Turns on or off power to the block.  Effectively the same as the Enabled property.

```
#Turn on
turn on power to "My Projector"
power on "My Projector"

#Turn off
turn off "My Projector"
power off "My Projector"
```

## "Ratio" Property
* Read-only
* Primitive Type: Numeric
* Keywords: ```ratio, ratios, percentage, percentages, percent, percents```

Gets a value between 0 - 1 representing how close the Projector's blueprint is to completion.

```
Print "Percentage Done: " + "My Projector" percentage
```

## "Complete" Property
* Read-only
* Primitive Type: Bool
* Keywords: ```done, ready, finished, complete, built```

Gets whether the projector's blueprint is complete or not.

```
when "My Projector" is complete
  Print "My Creation is ready!"
```
