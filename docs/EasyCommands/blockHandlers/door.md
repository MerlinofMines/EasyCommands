# Door Block Handler
This block handler can be used to open and close doors, including regular doors and Airtight Hangar Doors.  The properties are the same for both.

* Block Type Keywords: ```door, bay, hangar, gate```
* Block Type Group Keywords: ```doors, bays, hangars, gates```

Default Primitive Properties:
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
enable "My Door"
set "My Door" to enabled
turn on "My Door"

#Disable Block
disable "My Door"
set "My Door" to disabled
turn off "My Door"
```

## "Power" Property
* Primitive Type: Bool
* Keywords: ```power, powered```

Turns on or off power to the block.  Effectively the same as the Enabled property.

```
#Turn on
turn on power to "My Door"
power on "My Door"

#Turn off
turn off "My Door"
power off "My Door"
```

## "Open" Property
* Primitive Type: Boolean
* Keywords: ```open, opened, ```
* Inverse Keywords: ```close, closed, shut```

Gets/Sets whether the door is open.  If the door Ratio > 0 the door is considered Open.  So while a door is closing it is still considered open.

```
when the "Outer Door" is opened
  Print "Welcome Home!"

close the "Outer Door"
close the "Bombay Doors"
open the "Storage Bay"
```

## "Ratio" Property
* Read-only
* Primitive Type: Numeric
* Keywords: ```ratio, ratios, percent, percents, percentage, percentages```

Returns a value from 0 - 1 representing the Open Ratio for the given door(s).  0 means fully closed, 1 means fully open.

```
Print "Open Ratio: " + "My Door" ratio
```