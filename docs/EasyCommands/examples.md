## Automatic Brake and Reverse Lights & Beeper
This script will add some fun to your rover by adding brakes, reverse lights, and reverse beeper.  

```
:turnOn
turn on the "rover signal lights"
until "rover light program" is off
  async call "rearLights"
  async call "reverseSirens"

:rearLights
if "rover cockpit" is locked
  async turn on the "rear lights"
  set the "rear lights" color to "red"
else if "rover cockpit" backwards input > 0
  if "rover cockpit" forward velocity > 1
    async turn on the "rear lights"
    set the "rear lights" color to "red"
  else
    async turn on the "rear lights"
    set the "rear lights" color to "yellow"
else if "rover cockpit" forwards input > 0 and "rover cockpit" backwards velocity > 1
  async turn on the "rear lights"
  set the "rear lights" color to "red"  
else
  turn off the "rear lights"

:reverseSirens
if "rover cockpit" backwards velocity > 1
  if "reverse sirens" are off
    turn on the "reverse sirens"
else
  turn off the "reverse sirens"

:turnOff
turn off the "rover signal lights"
```


## Simple Airlock Manager using Sensor at External Door:
The following script controls a simpler airlock with 2 doors and a single sensor which detects wanting to move through the external door (to exit or enter):

```
:main
async call "runAirlock"
wait 3
restart

:runAirlock
if "Airlock Sensor" is triggered
  call "goOut"
else
  call "comeIn"

:comeIn
if "Airlock Exterior Door" is open
  close the "Airlock Exterior Door"
else if "Airlock Vent" is not pressurized
  pressurize the "Airlock Vent"
else if "Airlock Interior Door" is closed
  open the "Airlock Interior Door"

:goOut
if "Airlock Interior Door" is open
  close the "Airlock Interior Door"
else if "Airlock Vent" is pressurized
  depressurize the "Airlock Vent"
else if "Airlock Exterior Door" is closed
  open the "Airlock Exterior Door"
```

## Garage Door Opener
The following script can control 2 (or more if you want) doors for your garage or ship hanger by listening on the "garageDoors" channel.  From your vehicle, use the following command to open your door remotely!

Vehicle Opener Script:

```
:wait
wait 1 tick

:toggleDoor
send "goto toggleDoor" to "garageDoors"

:openDoor
send "goto opendoor" to "garageDoors"

:closeDoor
send "goto closedoor" to "garageDoors"
```

Garage Door Controller Script:

```
:setup
listen "garageDoors"
goto "listen"

:listen
wait 1
restart

:toggleDoor1
if "door1" doors are closed
  goto "openDoor1"
else
  goto "closeDoor1"
goto "listen"

:toggleDoor2
if "door2" doors are closed
  goto "openDoor2"
else
  goto "closeDoor2"
goto "listen"

:openDoor1
open "door1" doors
goto "listen"

:closeDoor1
close "door1" doors
goto "listen"

:openDoor2
open "door2" doors
goto "listen"

:closeDoor2
close "door2" doors
goto "listen"
```
