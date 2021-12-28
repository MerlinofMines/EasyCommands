
## Automatic Brake and Reverse Lights & Beeper
This script will add some fun to your rover by adding brakes, reverse lights, and reverse beeper.  

The lights will continue running until you turn them off (either manually or by calling "turnOff")

![Lights on the Boom Rover](https://i.imgur.com/ZaHA9xc.gif)

```
:main
set myCockpit to "Boom Cockpit"
set rearLights to "Rear Lights"
set reverseSirens to "Reverse Sirens"

goto runLights

:runLights
controlRearLights
controlReverseSirens
replay

:controlRearLights
if $myCockpit is locked
  turn on the $rearLights
  set the $rearLights color to red
else if $myCockpit backwards input > 0
  if $myCockpit forward velocity > 1
    turn on the $rearLights
    set the $rearLights color to red
  else
    turn on the $rearLights
    set the $rearLights color to yellow
else if $myCockpit forwards input > 0 and $myCockpit backwards velocity > 1
    turn on the $rearLights
    set the $rearLights color to red
else if $myCockpit backwards velocity > 1
    turn on the $rearLights
    set the $rearLights color to yellow
else
  turn off the $rearLights

:controlReverseSirens
if $myCockpit backwards velocity > 1
  if $reverseSirens are off
    turn on the $reverseSirens
else
  turn off the $reverseSirens
```
