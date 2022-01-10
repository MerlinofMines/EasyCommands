# Battery Block Handler
This block handler is all about batteries.  Keep your ships powered and monitor battery levels so you're never without power.

* Block Type Keywords: ```battery```
* Block Type Group Keywords: ```batteries```

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
enable "My Battery"
set "My Battery" to enabled
turn on "My Battery"

#Disable Block
disable "My Battery"
set "My Battery" to disabled
turn off "My Battery"
```

## "Power" Property
* Primitive Type: Bool
* Keywords: ```power, powered```

Turns on or off power to the block.  Effectively the same as the Enabled property.

```
#Turn on
turn on power to "My Battery"
power on "My Battery"

#Turn off
turn off power to "My Battery"
power off "My Battery"
```

## "Supply" Property
* Primitive Type: Bool
* Keywords: ```supply, supplying, generate, generating, discharge, discharging```
* Inverse Keywords: ```consume, consuming, recharge, recharging```

This property sets whether the battery is set to "Auto" (discharging) or "Recharge" (recharging).

```
#Check if batteries are recharging
if the "Ship Batteries" are recharging
  Print "Don't disconnect or you'll be sorry!"

#Recharge the batteries
tell the "Ship Batteries" to recharge

#Supply
tell the "Ship Batteries" to generate
tell the "Ship Batteries" to supply

#Stop recharging (same as supply)
stop recharging the "Ship Batteries"
tell the "Ship Batteries" to stop recharging
```

If you really want to set a battery to discharge (vs auto), you can do this with the "ChargeMode" dynamic property:

```
#Sets the battery to Discharge
set the "Test Battery" "ChargeMode" property to 2
```



## "Auto" Property
* Primitive Type: Bool
* Keywords: ```auto```

Gets or Sets whether the battery is currently in "Auto" mode.  If set to false, the battery mode is set to recharge.

```
#Check if batteries are on auto
if the "Ship Batteries" are on auto
  Print "Safe to take off!"

#Set the batteries to auto
set the "Ship Batteries" to auto
```

## "Limit" Property
* Read-only
* Primitive Type: Numeric
* Keywords: ```limit, limits```

Gets the maximum amount of power that the given battery is able to store.

```
Print "Max Stored Power: " + the "Ship Batteries" limit, in MW
```

## "Ratio" Property
* Read-only
* Primitive Type: Numeric
* Keywords: ```ratio, ratios, percent, percents, percentage, percentages```

Returns a value between 0 and 1 representing the ratio of stored power / max stored power.  

```
#Since aggregations use sum by default, make sure to use avg.
#The sum of battery ratios doesn't make much sense.
Print "Charge Percentage: " + the avg "Ship Batteries" ratio
```

## "Level" Property
* Read-only
* Primitive Type: Numeric
* Keywords: ```level, levels```

Returns a value representing the current stored power in the battery, in MW.

```
#This property can be sumed since levels are totals
Print "Power Level: " + the "Ship Batteries" level
```

## "Input" Property
* Read-only
* Primitive Type: Numeric
* Keywords: ```input, inputs```

Returns a value representing the current input power to the battery, in MW / s.

```
Print "Current Power Input: " + "Ship Batteries" input
```

## "Output" Property
* Read-only
* Primitive Type: Numeric
* Keywords: ```output, outputs```

Returns a value representing the current output power from the battery, in MW / s.

```
Print "Current Power Output: " + "Ship Batteries" output
```
