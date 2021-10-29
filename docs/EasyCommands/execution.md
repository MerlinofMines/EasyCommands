# How Your Script Executes

Just like all Programmable Block programs, EasyCommands runs on a "game loop".  Once every execution interval, or "tick", your script gets executed.  By default your script will execute 60 times / second.

An EasyCommands script will run until completion, at which point the program terminates unless it is executed again.  It is also easy to create programs that area always running to manage things like Airlocks, systems automation, you name it.

At it's core, EasyCommands is just keeping track of a list of [Commands](https://spaceengineers.merlinofmines.com/EasyCommands/commands "Commands") to execute.  These Commands can also be broken up into [Functions](https://spaceengineers.merlinofmines.com/EasyCommands/functions "Functions")

Each tick, EasyCommands attempts to run as many commands as possible, in order, until it encounters a command that blocks execution (such as waiting for a specific amount of time, or for a specific [Condition](https://spaceengineers.merlinofmines.com/EasyCommands/conditions "Conditions") to be met.

So for example, the following script would execute all commands on the same tick:

```
Print "Starting Program"
turn on the "Outside Lights"
turn off the "Outside Lights"
Print "Done"
```

However, by adding a wait command we would execute the same commands but over the course of 2 ticks

```
Print "Starting Program"
turn on the "Outside Lights"
wait 1 tick
turn off the "Outside Lights"
Print "Done"
```

By waiting for seconds, instead of ticks, we can create programs that execute over perceptible periods of time:

```
turn on the "Outside Lights"
wait 1 second
turn off the "Outside Lights"
wait 1 second
turn on the "Outside Lights"
wait 1 second
turn off the "Outside Lights"
```
As you can imagine, the above script has the effect of making the "Outside Lights" blink on and off

#Breaking Up Your Script Into Pieces
EasyCommands also supports the concept of functions, which allows you to break up your script into easy to understand and re-usable sections.  For example, We could improve the above script by doing the following:

```
blink 2 times

:blink
turn on the "Outside Lights"
wait 1 second
turn off the "Outside Lights"
wait 1 second
```

Check out the section on [Functions](https://spaceengineers.merlinofmines.com/EasyCommands/functions "Functions") to get more information on the syntax, passing parameters, and more.

# Managing Multiple Programs
Another useful feature of EasyCommands is built in support for multi-tasking.  For you programmers out there, think "multi-threading".

EasyCommands has the concept of "Threads" which can keep track of multiple things at once.  Every tick the main thread will execute.  In addition, all asynchronous threads will also be executed.

This allows you to run multiple tasks in the background from a single program.  So from a single EasyCommands script you could
* Turn the Lights On when you enter your base
* Manage the Airlock
* Monitor your stored power levels and turn on the Hydrogen Engine if your power gets too low

```
async manageLights
async manageAirlock
async monitorPowerLevels

:manageLights
#Write your script

:manageAirlock
#Write your script

:monitorPowerLevels
#Write your script
```

Check out the section on [Threads](https://spaceengineers.merlinofmines.com/EasyCommands/threads "Threads") for more information!
