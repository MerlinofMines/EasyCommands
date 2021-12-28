# Ship Follow Script

With these scripts you can always keep your support ship close at hand!  

Just load the Sender script onto your ship and load the Listener Script onto the ship you want to follow you.

![Ship Following](https://i.imgur.com/ZzSsCxN.gif)

Don't forget to put antenna's on both ships!  You can have more than 1 ship follow you by putting the Listener script on multiple ships.

## Listener Script
```
:setup
listen "follow"
async call "listenForFollow"

:listenForFollow
wait
replay

:setLocation targetLocation followDistance
set myLocation to "Main Cockpit" location
set followVector to (targetLocation - myLocation) - followDistance
set followLocation to myLocation + followVector

if followLocation - myLocation > 10
  set the "Remote Control" destination to followLocation
  turn on the "Remote Control"
else
  turn off the "Remote Control"

:stopFollowing
turn off the "Remote Control"
```

## Sender Script
```
bind myLocation to "Fighter Cockpit" location
bind command to 'setLocation ' + myLocation + " " + 100
goto sendCommand

:sendCommand
Print "Command: " + command
set "Test Program" display [0] to command
send command to "follow"
wait 3
restart
```
