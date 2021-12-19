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
* Keywords: ```done, ready, complete, finished```

Returns true if the airvent is not actively pressurizing or depressuring, false otherwise.

## "Run" property
* Read-only
* Primitive Type: Bool
* Keywords: ```run, running```

The opposite of "complete".  Returns true if the airvent is actively pressuring or depressuring, false otherwise.

```
until the "Air Lock Vent" is complete
  Print "Please wait..."
```

```
while the "Air Lock Vent" is running
  Print "Please wait..."
```

## "Pressurize" property
* Primitive Type: bool
* Keywords: ```pressurize, pressurized```
* Inverse Keywords: ```depressurize, depressurized```

Gets or sets whether the air vent is currently pressurized.  If the Vent is de-pressurized, de-pressurizing, or pressurizing, this will return false.

```
until the "Air Vent" is pressurized
  pressurize the "Air Vent"
```

```
depressurize the "Air Vent"
when the "Air Vent" is complete
  open the "External Door"
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
