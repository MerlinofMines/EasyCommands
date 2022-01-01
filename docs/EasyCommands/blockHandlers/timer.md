# Timer Block Handler
This block handler lets you control Timer blocks, including triggering them immediately on on a delay.
* Block Type Keywords: ```timer```
* Block Type Group Keywords: ```timers```

Default Primitive Properties:
* Bool - Enabled

## "Enabled" Property
* Primitive Type: Bool
* Keywords: ```enable, enabled```
* Inverse Keywords: ```disable, disabled```

Enables or Disables the given block

```
#Enable Block
enable "My Timer Block"
set "My Timer Block" to enabled
turn on "My Timer Block"

#Disable Block
disable "My Timer Block"
set "My Timer Block" to disabled
turn off "My Timer Block"
```

## "Power" Property
* Primitive Type: Bool
* Keywords: ```power, powered```

Turns on or off power to the block.  Effectively the same as the Enabled property.

```
#Turn on
turn on power to "My Timer Block"
power on "My Timer Block"

#Turn off
turn off "My Timer Block"
power off "My Timer Block"
```

## "Trigger" Property
* Primitive Type: Bool
* Keywords: ```trigger, triggered```

Get/Sets whether the Timer Block is currentl triggered. When getting, returns if the Timer Block is currently counting down.  When set, will trigger the Timer Block immediately.

```
if "My Timer Block" is triggered
  Print "Timer Block is running"

#Trigger Immediately
trigger "My Timer Block"
```

## "Silence" Property
* Primitive Type: Bool
* Keywords: ```silent, silence```

Gets/Sets whether the timer will play an audible beep when it triggers.

```
if "My Timer Block" is silent
  Print "Running Silently"

silence "My Timer Block"
```

## "Delay" Property
* Primitive Type: Numeric
* Keywords: ```delay, delays, limit, limits```

Gets/Sets the trigger delay for the Timer Block, in seconds.  This does *not* return the active countdown time (if there is one), it returns the configured delay for the Timer Block.

```
Print "Timer Delay: " + "My Timer Block" delay

set "My Timer Block" delay to 10 seconds
```

## "Countdown" Property
* Primitive Type: Bool
* Keywords: ```countdown, countdowns```

When retrieving, returns whether the timer is currently counting down to activation.

When setting, will either start or stop the countdown based on the input value.

```
#Check if counting down
if "My Timer Block" countdown is on
  Print "Timer is ticking..."

#start the countdown
start "My Timer Block" countdown

#Stop the countdown
stop "My Timer Block" countdown
```
