The following script makes it easy to control your functional blocks using human readable commands.

To use, simply install this script onto your Programmable Block (or blocks!) of choice.

Then, in the custom data of that programmable block, put in a line separated list of commands that you'd like to execute.

Supported Commands (this is a very small subset of possible command phrases, it just has 1 per actual command capability):
Wait [for] (number) {seconds/ticks}
[Turn the] (selector) (blockType) {group} (on/off)
[Turn] (on/off) [the] (selector) (blockType) {group}
(Extend/Retract) [the] (selector) piston{s} [to] (number) {meters}
(Extend/Retract) [the] (selector) piston{s} {by} (number) {meters}
[Rotate the] (selector) rotor{s} {clockwise/counter clockwise} [to] (number) {degrees}
[Set the] (selector) (piston/rotor){s} (speed/velocity) [to] (number) {meters/rpm}

() = Required
[] = optional, ignored
{} = optional, not ignored

Example Commands:
Wait for 3 seconds
Turn the ship projector on
Turn off the factory merge block
Turn on the "Self Destruct" Timer Block
Extend the vertical pistons
Move the "Lower Pistons" pistons to 180 degrees
Rotate the "Lower Pistons" piston group by 30 degrees clockwise

Or, the same commands as above, but shorthand:
Wait 3
ship projector on
factory merge off
"Lower Pistons" pistons 180
"Lower Pistons" pistons add 30

Supported Block Types:
Piston
Rotor
Timer [Block]
Merge [Block]
Projector

Group Support:
You can say "group" or use the plural of the above ("timers", "timer blocks") to specify that the program should look for a "group" instead of individual names

Selector Support:
Selector uses the Custom Name of individual blocks, or the Group Name, to figure out which thing to talk to.  If you have multiple blocks with the same custom name (and block type) then the command
well execute on all of them.  

If a selector is provided and a BLock Type is not, the program will attempt to parse the Block Type from the selector.  This means if you named your piston group "Pistons" or "Horizontal Pistons" then it will work.

Direction:
Raise/Expand/Extend/Forward/Up/Increase/Clock/Clockwise = Increase, or move Clockwise
Lower/Reduce/Retract/Backwards/Down/Decrease/Counter/Counterclock = Decrease, or move Counter Clockwise.  Dealing with "Counter clockwise" is not supported..yet.  

"Relative" Command Support
Relative means "do this relative to existing".  This allows you to increase/decrease the velocity/extension/rotation (vs setting it directly).
By = Relative
Add = Relative Increase
Subtract = Relative Decrease

Measurement Unit Type Support:
Distance: meters (defaults to meters)
Time: seconds/ticks (defaults to seconds)
Angles: degrees (defaults to degrees).  Radian not supported yet.
Rotational Rate: rpm (defaults to rpm).  Degrees/Radians per minute not supported yet.  

Velocity Support:
When changing velocity, direction is ignored.  So if the velocity is currently -2.0 and you say "increase velocity by 2", the resulting velocity will be
-4.  Not 0.  Why?  Trust me, this is easier to reason about when writing a program.  I think it's even more clear if you use words like "speed" instead of "velocity"