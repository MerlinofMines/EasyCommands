## Airlocks

Automating Airlocks is a great use case for EasyCommands.  There's lots of ways to build an airlock, so here are a couple of examples.

![Automated Airlock](https://i.imgur.com/UMk0wuZ.gif)

## Simple Button Based Airlock
Here's a simple script to manage a simple button-based airlock:

```
:main
wait

:comeInside
close the "Airlock Exterior Door"
wait until the "Airlock Exterior Door" are closed
pressurize the "Airlock Vent"
until the "Airlock Vent" is pressurized
  Print the "Airlock Vent" ratio
open the "Airlock Interior Door"

:goOutside
close the "Airlock Interior Door"
wait until the "Airlock Interior Door" is closed
depressurize the "Airlock Vent"
until the "Airlock Vent" is depressurized
  Print the "Airlock Vent" ratio
open the "Airlock Exterior Door"
```

## Sensor Based Airlock Manager
The following script can control multiple airlocks simultaneously, with each airlock consistening of 2 doors, an airlock vent, and a sensor which detects someone at the exterior door (presumably wanting to exit or enter).

If the sensor is triggered it de-pressurizes the airlock and opens the exterior door.  Otherwise it pressurizes the airlock and keeps the interior door open.

The below script is running 3 different airlocks.

```
:main
async call "runAirlock" '[SAS] Airlock Sensor' '[SAS] Airlock Exterior Door' '[SAS] Airlock Interior Door' '[SAS] Airlock Vent'
async call "runAirlock" '[SAS] Dock 1 Sensor' '[SAS] Dock 1 Outer Door' '[SAS] Dock 1 Inner Door' '[SAS] Dock 1 Air Vent'
async call "runAirlock" '[SAS] Dock 2 Sensor' '[SAS] Dock 2 Outer Door' '[SAS] Dock 2 Inner Door' '[SAS] Dock 2 Air Vent'

:runAirlock sensorName exteriorDoorName innerDoorName ventName
if $sensorName sensor is triggered
  call "goOutAirlock" exteriorDoorName innerDoorName ventName
else
  call "comeInAirlock" exteriorDoorName innerDoorName ventName
replay

:goOutAirlock exteriorDoorName innerDoorName ventName
if $innerDoorName door is open
  close the $innerDoorName door
else if $ventName vent ratio > 0.1
  depressurize the $ventName vent
else
  open the $exteriorDoorName door

:comeInAirlock exteriorDoorName innerDoorName ventName
if exteriorDoorName door is open
  close the $exteriorDoorName door
else if $ventName vent ratio < 0.99
  pressurize the $ventName vent
else if $innerDoorName door is closed
  open the $innerDoorName door
```
