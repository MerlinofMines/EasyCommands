# Air Vent Handler

This Block Handler handles Air Vents. Pretty Straightforward

* Block Type Keywords: ```vent, airvent```
* Block Type Group Keywords: ```vents, airvents```

Default Primitive Properties:
* Boolean - Pressurized (replaces "enabled" from Terminal Block Handler)
* Numeric - Ratio

Default Directional Properties
* Up - Ratio

## "Complete" property
* Read-only
* Primitive Type: Bool
* Keywords: ```done, ready, complete, finished, pressurized, depressurized```

Returns true if the airvent is not actively pressurizing or depressuring, false otherwise.  Note the "pressurized" and "depressurized" both return true if the airvent is not in progress.
To determine the full state of the air vent, use this property in combination with the "Pressurize" property.

Due to a quark of Space Engineers, sometimes completely depressurized is not registered as "Complete" by Space Engineers.  As a work around, this property considers a pressurized ratio < 0.0001 as Complete.

```
#Pressurize Property to depressurize Air Lock Vent
depressurize the "Air Lock Vent"

#Complete Property to wait until we are finished depresssurizing
until the "Air Lock Vent" is depressurized
  Print "Please wait..."
```

## "Run" property
* Read-only
* Primitive Type: Bool
* Keywords: ```run, running```

The opposite of "complete".  Returns true if the airvent is actively pressuring or depressuring, false otherwise.

```
while the "Air Lock Vent" is running
  Print "Please wait..."
```

## "Pressurize" property
* Primitive Type: bool
* Keywords: ```pressurize, pressurized```
* Inverse Keywords: ```depressurize, depressurized```

Gets or sets whether the air vent pressurizing mode (pressurizing or de-pressuring).  This property does not indicate the actual pressurization state, but rather the mode.  To get the
pressurization state, use this property in combination with either the "Complete" or "Run" property. 

```
until the "Air Vent" is pressurized
  pressurize the "Air Vent"
```

```
depressurize the "Air Vent"
when the "Air Vent" is complete
  open the "External Door"
```
## Examples

Here's a full example to get all possible Air Vent states.  Substitute with your own air vent to test yourself.

```
set myAirVent to "Airlock Vent"

if $myAirVent is pressurizing and $myAirVent is pressurized
  set my display to myAirVent + " is completely pressurized"
else if $myAirVent is pressurizing and $myAirVent is running
  set my display myAirVent + " is pressurizing"
else if $myAirVent is depressurizing and $myAirVent is depressurized
  set my display myAirVent + " is completely depressurized"
else if $myAirVent is depressurizing and $myAirVent is not complete
  set my display myAirVent + " is depressurizing"

increase my display text by "\nLevel: " + $myAirVent level
replay
```

## "Ratio" property
* Read-only
* Primitive Type: Numeric
* Keywords: ```ratio, ratios, percentage, percentages, percent, percents```

Returns a value between 0 - 1 representing the pressure ratio (oxygen level) of the Air Vent.

```
depressurize the "Air Vent"
wait until the "Air Vent" ratio < 0.1
open the "Exterior Doors"
```

## "Level" property
* Keywords: ```level, levels```

Identical to the "ratio" property

```
depressurize the "Air Vent"
wait until the "Air Vent" level < 0.1
open the "Exterior Doors"
```
