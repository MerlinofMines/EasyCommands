# Warhead Block Handler
This block handler gives you control over warheads, including arming/disarming, set the countdown, and of course, detonating.

* Block Type Keywords: ```warhead```
* Block Type Group Keywords: ```warheads```

## "Armed" Property
* Primitive Type: Bool
* Keywords: ```enable, enabled, arm, armed```
* Inverse Keywords: ```disable, disabled, disarm, disarmed```

Arms/Disarms the Warhead

```
if the "Trap Warhead" is armed
  Print "Tread Lightly"

arm the "Trap Warhead"
```

## "Detonate" Property
* Primitive Type: Bool
* Keywords: ```detonate, trigger, triggered```

This property can be used to immediately detonate the warhead without a countdown.

When retrieving, this property will tell you if the warhead is counting down.  When setting (either true or false), it detonates the bomb.

```
if the "Trap Warhead" is triggered
  Print "Time is ticking..."

#Boom
detonate the "Trap Warhead"
```

## "Delay" Property
* Primitive Type: Numeric
* Keywords: ```delay, delays, limit, limits```

This property will get or set the Detonation Time for the warhead, in seconds.

When retrieving, this property returns the current countdown (whether active or not).

```
Print "Detonation in " + the "Trap Warhead" delay + " seconds"

set the "Trap Warhead" delay to 10 seconds
```

## "Delay" Property
* Primitive Type: Numeric
* Keywords: ```delay, delays, limit, limits```

This property will get or set the Detonation Time for the warhead, in seconds.

When retrieving, this property returns the current countdown (whether active or not).

```
Print "Detonation in " + the "Trap Warhead" delay + " seconds"

set the "Trap Warhead" delay to 10 seconds
```

## "Countdown" Property
* Primitive Type: Bool
* Keywords: ```countdown```

When retrieving, returns where the warhead is currently counting down to detonation.

When setting, will either start or stop the countdown based on the input value.

```
#Check if counting down
if the "Trap Warhead" countdown is on

#start the countdown
start the "Trap Warhead" countdown

#Stop the countdown
stop the "Trap Warhead" countdown
```



