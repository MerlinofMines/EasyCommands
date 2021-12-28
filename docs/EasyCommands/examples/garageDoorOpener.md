## Garage Door Opener
The following scripts enable you to have keyless entry into your base from your rovers or ships using 2 EasyCommands scripts.  The first is loaded onto your vehicle and sends commands to open & close the doors.  The second is loaded on your base and operates the doors.  The two scripts communicate together to enable keyless entry.

![Garage Door Opener](https://i.imgur.com/tgkxiRN.gif)

Don't forget to put antennas on both your vehicle and your base!

### Vehicle Opener Script
This script is loaded onto your vehicle.  It just contains a few functions to call the base to open/close/toggle the doors.

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

## Garage Door Controller Script
This script should be loaded onto a Program on the base, and will actually control the doors.

```
:setup
listen "garageDoors"
goto "listen"

:listen
wait 1
replay

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
