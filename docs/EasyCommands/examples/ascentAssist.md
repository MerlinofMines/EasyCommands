# Automated Ascent Assist

This script uses a PID loop to control your thrusters output, based on a desired speed up to the max of 100.0.

This script ensures you do not go over 100m/s, while getting you as close to it as possible using the least amount of thrust.

It automatically adjusts for changes in gravitational strength as you get farther from the planet.  No more thrust overrides!

Let Automated Ascent Assist do the work for you.

```
:main
set desiredSpeed to 80
set Kp to 0.1
set Kd to 0.02
set Ki to 0.008

#Test

set i to 0
set d to 0
set previousError to desiredSpeed
goto ascendAssist

:ascendAssist
set currentSpeed to the "Fighter Cockpit" speed
set error to desiredSpeed - currentSpeed
set dt to 1/60
set i to i + error * dt
if Ki*i > 1
  set i to 1/Ki
set d to (error - previousError) / dt
set previousError to error
set newValue to Kp*error + Ki*i + Kd*d
Print "Desired Speed : " + desiredSpeed
Print "Current Speed: " + currentSpeed
Print "Error: " + error
Print "New Thrust Value: " + newValue
Print "P: " + (Kp*error)
Print "D: " + (Kd*d)
Print "I: " + (Ki*i)

if error < 1 and abs error < 5 and desiredSpeed < 99.5 and currentSpeed - error < 100
  set desiredSpeed to desiredSpeed + 0.01

set "Up Hydrogen Thrusters" ratio to newValue
set thrusterLimit to the avg "Up Hydrogen Thrusters" limit
Print "Current Limit: " + thrusterLimit

replay

```