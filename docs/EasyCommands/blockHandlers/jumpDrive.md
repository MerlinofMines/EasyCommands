# Jump Drive Block Handler
This Block Handler handles Jump Drives.  You aren't able to automatically jump (not supported by SE) but you can use it to get some information about possible jumps.

Note: These properties need to be refreshed so that "limit" references the jump limit, not power limits.  Stay tuned for updated properties on this block handler.

* Block Type Keywords: ```jump, jumpdrive```
* Block Type Group Keywords: ```jumpdrives```

Default Primitive Properties:
* Bool - Enabled

## "Enabled" Property
* Primitive Type: Bool
* Keywords: ```enable, enabled```
* Inverse Keywords: ```disable, disabled```

Enables or Disables the given block

```
#Enable Block
enable "My Jump Drive"
set "My Jump Drive" to enabled
turn on "My Jump Drive"

#Disable Block
disable "My Jump Drive"
set "My Jump Drive" to disabled
turn off "My Jump Drive"
```

## "Power" Property
* Read-Only
* Primitive Type: Numeric
* Keywords: ```power, powered```

Returns the currently stored power, in MW, of the Jump Drive.

```
Print "Stored Power: " + "My Jump Drive" power
```

## "Ratio" Property
* Read-only
* Primitive Type: Numeric 
* Keywords: ```ratio, ratios, percentage, percentages, percent, percents```

Gets the current ratio of stored power to max power.

```
Print "Power Ratio: " + "My Jump Drive" ratio
```

## "Limit" Property
* Primitive Type: Numeric
* Supported Directions: Up, Down
* Keywords: ```limit, limits, distance, distances, range, ranges```

Gets or Sets the current Jump limit for the jump drive.  If no direction is passed, returns the currently set jump distance.

When set, the jump distance is expected in meters.  If the requested distance is less than the minimum jump distance, the minimum is set.  If the requested distance is more than the max possibly jump distance, the max possible jump distance is set.

Directions are only supported for retrieval.  If Up is included, returns the maximum Limit, in meters, the jump drive is capable of jumping based on the current charge level. If Down is included, returns the minimum distance, in meters, the jump drive is capable of jumping.

```
Print "Current Jump Distance: " + "My Jump Drive" limit

Print "Max Jump Distance: " + "My Jump Distance" upper limit

Print "Min Jump Distance: " + "My Jump Distance" lower limit

#10km
set "My Jump Drive" distance to 10000
```

## "Length" Property
* Primitive Type: Numeric
* Supported Directions: Up, Down
* Keywords: ```length, lengths, height, heights, level, levels```

Same as Limit Property.  Returns the current Jump limit for the jump drive.

```
Print "Current Jump Distance: " + "My Jump Drive" length

Print "Max Jump Distance: " + "My Jump Distance" upper length

Print "Min Jump Distance: " + "My Jump Distance" lower length

#10km
set "My Jump Drive" length to 10000
```

## "Complete" Property
* Read-only
* Primitive Type: Bool
* Keywords: ```complete, ready, done, finished```

Returns whether the Jump Drive is ready to jump.  Specifically, whether the Jump Drive's status is READY.

```
if "My Jump Drive" is ready
  Print "Ready to Jump!"
```

## "Recharge" Property
* Primitive Type: Bool
* Keywords: ```recharge, recharging, consume, consuming```
* Inverse Keywords: ```supply, supplying, generate, generating, discharge, discharging```

This property gets or sets whether the jump drive is set to "Recharge" 

```
if "My Jump Drive" is recharging
  Print "Charging Warp Drive"

#Recharge the jump drive
tell "My Jump Drive" to recharge

#Stop Recharging the jump drive
stop recharging "My Jump Drive"
```
