# Commands

Each line of your script is considered a "Command".  EasyCommands supports a variety of different commands to accomplish tasks.  Some Commands manage the program, such as creating [Variables](https://spaceengineers.merlinofmines.com/EasyCommands/variables "Variables"), creating [Threads](https://spaceengineers.merlinofmines.com/EasyCommands/threads "Threads"), or starting/stopping/pausing/restarting the program.  Other commands can interact with [Blocks](https://spaceengineers.merlinofmines.com/EasyCommands/blockHandlers "Block Handlers"), listen for or send commands on other grids, transfer items between inventories, check conditions, perform aggregations, and more.  Below is a list of all command types supported by EasyCommands.

## Comments
Any line beginning with "#" will be treated as a comment and ignored by the script.  Feel free to add as many as you like!

```
#This is a comment!
print "I'm not a comment!"
```

## Nested Commands
Nested commands are a way of executing multiple commands, sequentially, as a unit.  This allows you to execute a series of commands when a condition is met (or not met).

```
if the lights are on
  Print "The Lights are on!"
  Print "You're using power!"
```

Conversely, in the following example "You're using power!" would always be printed but "The Lights are on!" would only be printed if the lights are on:

```
if the lights are on
  Print "The Lights are on!"
Print "You're using power!"
```

There's no requirement that you use 1 vs 2 vs 4 spaces for nesting commands together, but you'll need to use consistent spacing for a set of nested commands to be interpreted correctly.

### Nesting Spaces

You can further nest spaces if you want to use nested commands inside of another nested command.  Just make sure that all the items intended to be part of one nested command have the same spacing

```
if "outer door" is open
  turn on the "outer lights"
  if the "inner door is open"
    turn on the "inner lights"
    turn on the generator
  else
    close the "outer door"
    wait 1
  wait 3
```

## Print Command
The simplest thing you can do with EasyCommands is output text to the Programmable Block.  This text will be visible when viewing the Programmable Block from the terminal menu.

```
Print "Hello World!"
```

You can print the result any [Variable](https://spaceengineers.merlinofmines.com/EasyCommands/variables "Variables") as well:

```
#The following would print "My Variable is: Hello World!"
set myVariable to "Hello World!"
Print "My Variable is: " + myVariable
```

The output from each Print command will appear on a new line in EasyCommands.  The below script would yield 2 lines of output:

```
Print "Hello World!"
Print "How are you?"
```

Print statements are **erased every tick**.  If you want to continually see a message on the Programmable Block screen you will need to make sure you are printing the message **every tick**.

The following would not end up showing anything useful since "Hello World!" would only show up for 1 tick and then be gone.
```
print "Hello World!"
wait until false
```

The below command would persist "Hello World!" on the screen forever:
```
until false
  print "Hello World!"
```

Print statements are organized by [Thread](https://spaceengineers.merlinofmines.com/EasyCommands/threads "Threads").  By default, EasyCommands prints a few pieces of useful information in addition to things you specifically ask to print to the console.  To change EasyCommands to only print what you want printed to the screen, edit the EasyCommands program (not your script, the actual program using "Edit") and change logLevel to LogLevel.SCRIPT_ONLY:

```
public LogLevel logLevel = LogLevel.SCRIPT_ONLY;
```

See [Debugging](https://spaceengineers.merlinofmines.com/EasyCommands/debugging "Debugging") for more information on other available logging levels.

## Wait Command

Sometimes you just have to wait.  The Wait command takes an optional variable (in seconds) for how long to take.  You can also specify "ticks" instead of seconds if you want to wait an amount of ticks instead.

```
#Waits 1 tick
wait
```

```
#Waits 10 seconds
wait 10
```

```
#Waits 10 seconds
wait 10 seconds
```

```
#Waits 10 ticks, where 1 tick = 1/60 seconds (regardless of Update Frequency)
wait 10 ticks
```

```
#Waits until myCondition (could be a variable or an expression) is true
wait until myCondition is true
```

The Wait command waits for the requested amount of time regardless of the Update frequency.  That said, the amount of time waited will become less accurate the lower the update frequency you set, so keep this in mind.

Also note that the Wait Command's wait duration is not perfectly accurate.  Don't use it for trying to keep track of accurate amounts of time over long durations.

## Interacting With Blocks

Probably the most important command type, and most often used, in EasyCommands is the Block Command.  This command enables you to act on a given block, group of blocks, or all blocks of a given type.  There are lots of variations of BlockCommand but the general form is to set or change the property of a block to a new value.

```turn on the "Exterior Lights"```

```recharge the batteries```

```set the "Elevator Piston" velocity to 2```

```open the "Outer Hangar" doors```

BlockCommands typically consist of an action to take, a [Selector](https://spaceengineers.merlinofmines.com/EasyCommands/selectors "Selectors") for which blocks to act on, the property for those blocks (see [BlockHandlers](https://spaceengineers.merlinofmines.com/EasyCommands/blockHandlers "Block Handlers")), and a value to set the property to.  Specifically:

```<set/increase/decrease> the <selector> <property> to <value>```.  Most BlockHandlers have default properties for value types.  Default values (usually "true") are also applied if you specify a property but not a value.  So ```turn on the "Exterior Lights"```
represents ```<set> the "Exterior Lights" <property?> on```

The default boolean property for the "Light" block type is POWER, which for a Light enables the lights.  Thus, 
```turn on the "Exterior Lights"``` will set the "Exterior Lights" blocks to enabled.  The following commands would also accomplish the same:

```
#All of these commands are equivalent
turn on the "Exterior Lights"
turn the "Exterior Lights" on
turn on the power to the "Exterior Lights"
"Exterior Lights" on
"Exterior Lights" power on
"Exterior Lights" true
power the "Exterior Lights"
power on the "Exterior Lights"
```

Again, check out BlockHandlers for a list of all available properties for each supported block type, what each property does, the default property/values, and the keywords for specifying selectors for those block types.

### Incrementing Values
You can increase or decrease a block's property value using ```increase/decrease```, such as ```increase the "Exterior Lights" range by 10```.  The "by 10" is optional; EasyCommands will attempt to increase/decrease the property by a reasonable delta (usually 10-20% of the properties range).

Increasing a String property value appends the given string to the existing value.  See [Operations](https://spaceengineers.merlinofmines.com/EasyCommands/operations "Operations") for a better description of what will happen whenever you add B to an A value type, as this is what will happen when increasing a block properties value.  

### Moving/Reversing Properties
You can also move some properties, which is handy for things like Rotors & Pistons:
```
#Rotors
turn the "test rotor" clockwise
turn the "test rotor" counterclockwise
reverse the "test rotor"

#Pistons
reverse the "test piston"
extend the "test piston"
retract the "test piston"
```

## Creating a Variable
There are plenty of use cases where you need to store the result of something so that you can use it later. EasyCommands allows you to store [Variables](https://spaceengineers.merlinofmines.com/EasyCommands/variables "Variables") which can be looked up later using the following syntax:

```
#Either of these will work
set myVariable to "Some Value"
assign myVariable to "Some Value"
allocate myVarible to "Some Value"
designate myVariable to "Some Value"
```

Once a variable is created you can reference it later using it's name:
```
set myValue to 2
print "My Value is: " + myValue
set myValue to myValue + 1
print "My Value is: " + myValue

# Results of above would be:
# My Value is: 2
# My Value is: 3
```

A variable can be set to any [Primitive](https://spaceengineers.merlinofmines.com/EasyCommands/primitives "Primitives") or the result of any expression, such as:

```
set myValue to 2
set myValue to otherValue + 3
set myValue to the count of my batteries
set myValue to (count of my batteries > 3)
```

### The "global" Keyword

By default, any variable you create will be tied to the current [Thread](https://spaceengineers.merlinofmines.com/EasyCommands/threads "Threads").  This means that any other threads that are running won't have access to this variable (except in some situations discussed in Threads).  In order to share a variable across all threads you need to specify the global keyword when setting the variable.

```
set global myValue to 2
set global myValue to otherValue + 3
set global myValue to the count of my batteries
set global myValue to (count of my batteries > 3)
```

## Binding a Variable
Typically when you set a variable, the value of that variable stays constant until you change it.  So by default, the following script would generate the expected result below:
```
#set a to 1
set a to 1

#set b to a + 1 = (1 + 1) = 2
set b to a + 1

#set a to 2
set a to 2

print "a is: " + a
print "b is: " + b

#The Result would be:
#a is: 2
#b is: 2
```

However, you might find that in some instances you want to store a calculation, rather than a value at that particular time.  You can do this using the "bind" keyword:
```
#set a to 1
set a to 1

#bind b to a + 1 (reference)
bind b to a + 1

#set a to 2
set a to 2

print "a is: " + a
print "b is: " + b

#The Result would be:
#a is: 2
#b is: 3
```

## Incrementing a Variable
Once you have variables, you sometimes want to increment or decrement them (say, you're iterating through a list of things and want to do an action on each).  To increase a variable, use the following syntax

```
set i to 5
set myVariable to 0
#Increases the "myVariable" variable by 10
increase myVariable by 10
myVariable+=10

#Decrease the "myVariable" by the "i" variable amount
decrease myVariable by i
myVariable-=i

#Increase "myVariable" by 1
increase myVariable
myVariable++
myVariable+=1

#Decrease "myVariable" by 1
decrease myVariable
myVariable--
myVariable-=1

#Add "World" to myVariable
myVariable+="World"
```

## Conditional Commands
Sometimes you might only want to run a command if or when a condition is met.  For example, maybe I want to wait for the airlock to be pressurized before I open the interior doors.  To do this, I could do the following:
```
if the "Airlock Vent" is pressurized
 open the "Airlock Interior Door"
```

In general, the form of a Conditional Command is ```<if/until/when/while/unless> <condition> <command> <else/otherwise> <command>```

A <condition> needs to be a variable or expression that resolves to true/false.  Things like:
```
if (outofAmmo)
if (a > b)
if (all of the lights are on)
if (the average "Elevator Piston" height > 5)
```

See the sections on [Variables](https://spaceengineers.merlinofmines.com/EasyCommands/variables "Variables") and [Operations](https://spaceengineers.merlinofmines.com/EasyCommands/operations "Operations") for a complete list of  the things you can use to create a condition.

You can also specify <command> in front of the <if> etc, though I only recommend this for simple commands like "wait":

```
wait until the "Exterior Doors" are closed
Print "Not High Enough" until the average "Elevator Piston" height > 5
```

Here's are all the types of conditional commands and how you might use them:

### If Command
As the name suggests, this command checks for a condition and if the condition is true it begins running the requested command(s).  If the condition is not met, the script proceeds to the next command outside the if statement

```
if the "Airlock Vent" is pressurized
  open the "Interior Door"
  set the "Base Screens" text to "Welcome Home!"
  wait 10 seconds
print "This command would run immediately if the condition isn't met."
```

### If/Else
If you want something else to happen when the command is not met, you can use the "else" or "otherwise" keywords.

```
if the "Airlock Vent" is pressurized
  open the "Interior Door"
  set the "Base Screens" text to "Welcome Home!"
otherwise
  pressurize the "Airlock Vent"
  set the "Airlock Screens" to "Please wait for airlock to pressurize"
```

You can chain conditional commands together in case there are more than two conditions to check:
```
set ammoRemaining to the "Gatling Turrets" inventory "ammo" amount
if ammoRemaining > 2
  print "Locked and loaded!"
else if ammoRemaining > 0
  print "Getting low on Ammo!"
else
  print "No More Ammo!"
```

You can also perform conditional checks inside of conditional checks.  Just make sure to keep your spaces consistent.  The script will line up commands and if/else statements based on the spaces
```
if a > 5
  if a > b
    print "A is greater than b and more than 5"
  else
    print "A is not greater than b but more than 5"
else
  print "A is not greater than 5
```

### Unless
The "unless" keyword is similar to if but the command only executes if the condition is false.
```
#Turn off the lights when no one is in the base
unless the "Base Occupied" sensor is triggered
  print "Turning off the lights to save power"
  turn off the "Base Lights"
```

### While
The "while" keyword is similar to "if", however the given command(s) will continue to be executed until the condition is false.  This allows you to effectively do something until a condition is met and then move on to do something else. For example:
```
while the "Airlock Vent" is not pressurized
  turn off the "Warning Lights"
  wait 1 second
  turn on the "Warning Lights"
  wait 1 second
```
Note that in the above command we are waiting for seconds as part of the requested command.  If a condition is met for any conditional command, the *entire* command will finish executing before the condition is checked again.  So the above script would always finish turning the "Warning Lights" off and on again before checking the condition again.  In addition, the condition *is not being checked* while the command is running.  So if the "Airlock Vent" happened to get pressurized, and then de-pressurized again, while the lights were blinking the while loop *would not* stop executing.

### Until
The "until" keyword is the "while" equivalent for unless.  It will execute the requested command if the given condition is false, and will continue exeucting the command until the condition is true.

```
#Keep trying to pressurize the Airlock Vent until it is pressurized!
until the "Airlock Vent" is pressurized
  pressurize the "Airlock Vent"
Print "We did it!  Airlock Vent is pressurized"
```

### When
The "when" keyword is a cool conditional check.  This conditional command will check for the given condition and wait until the condition is met.  Once the condition is met, it will execute the given command *once*.

```
pressurize the "Airlock Vent"
when the "Airlock Vent" is pressurized
  open the "Interior Airlock Door"
  Print "Welcome Home!"
```

You can use the else/otherwise command with when to perform a given command until the condition is met.  So putting this all together, you could write the Airlock Script we've been playing with previously as follows:

```
when the "Come Inside" sensor is triggered
  close the "Exterior Doors"
  wait until the "Exterior Doors" are closed
  pressurize the "Airlock Vent"
  when the "Airlock Vent" is pressurized
    open the "Interior Doors"
    Print "Welcome Home!"
  otherwise
    turn off the "Warning Lights"
    wait 1 second
    turn on the "Warning Lights"
    wait 1 second
otherwise
  Print "Waiting for a request to come inside..."
```
### Breaking While/Until/When Command
You can break out of a While/Until/When Conditional Command using the ```break``` keywords.  This will immediately halt execution of the While/Until/When and proceed to the next command after the conditional command.

```
set myList to []
set i to 0
until i > 5
  i++
  if i % 4 is 0
    break
  increase myList by i
print "My List: " + myList
#My List: [1,2,3]
```

Breaking from a While/Until/When command only breaks out of the immediate While/Until/When command.  If you perform a While/Until/When inside a While/Until/When, and break from the inner while/until/when, it will continue iterating through the outer While/Until/When.

```
set j to 0
until i > 3
  set i to 0
  until j > 3
    if i = j
      break
  print "Continuing..."
```

### Continuing a While/Until/When Command
Sometimes you might want to keep running a conditional command but restart the current command execution.  You can do this with the ```continue``` keywords.

```
set myList to []
set i to 0
until i > 5
  i++
  if i % 4 is 0
    continue
  increase myList by i
print "My List: " + myList
#My List: [1,2,3,5]
```

Continue acts similarly to break in that it only affects the immediate While/Until/When command.

## For Each Command

When dealing with [Collections](https://spaceengineers.merlinofmines.com/EasyCommands/collections "Collections"), you may find that you need to iterate over them and perform a command on each one.

You can do this a few ways in EasyCommands.  The simplest is to use the For Each Command.

```
set myList to ["one", "two", "three"]
for each item in myList
  set my display text to "My item is: " + item
  wait 1
# The results would be:
# "My Item is: 1"
# "My Item is: 2"
# "My Item is: 3"
```

This command will iterate over the given list of items.  For each item, it will assign a variable to that item, and then execute the given sub command. The name of the variable is given by as part of the command.  In the above command it is "item".  Be careful of using reserved keywords.

Note that when iterating, only 1 iteration is done per tick.  So if you do a "Print" statement, don't expect to get the whole list printed.  Remember, Print statements are reset every tick.  If you want all the items in a list, you'll need to store a variable that adds them all together, or just print the whole list in one go: ```print myList```

### Using Indexes for iteration

Sometimes you might want to use the index of an item, rather than the item itself, as the thing to iterate over.  Maybe you want to use the number for some additional purpose.  You can still use a for each command to do this, though perhaps in a little round about way.

```
set myList to ["one", "two", "three"]
for each i in 0..count of myList[]
  set my display text to "My item is: " + myList[i]
  wait 1
```

There's a few things going on here.  First, we are using "count of myList[] - 1" to get the length of the collection.  The "- 1" is because list indexes are "0" based (0,1,2) vs "1" based (1,2,3).  Second, we are using the Range [Operation](https://spaceengineers.merlinofmines.com/EasyCommands/operations "Operations") to generate a list of numbers from 0 to count of myList[] - 1, inclusive.  And lastly, we are using the iteration variable "i" to do a lookup of the list by numeric index, which returns the value of the list at that index.

### Iterating a list in reverse
Iterating a list by item directly will always iterate from front to back.  To iterate from back to front you will need to use index based list lookup.  Thankfully, you can do this pretty easily.

```
set myList to ["one", "two", "three"]
for each i in count of myList[] - 1 .. 0
  set my display text to "My item is: " + myList[i]
  wait 1
```

Above we are also creating a collection of indexes to iterate over, but in reverse.

### Alternative using until and incrementing variables
If, for some reason, you want to iterate a list in non-consecutive order, or there's some other thing you want to do before iterating further, or you want to remove items from the list as you process them, then you'll want to use an until or while conditional command instead of iteration.  The following program removes all even values from the given list and then prints the remaining values on the screen.
```
set myList to [1, 2, 3, 4, 5]
set i to 0
while i < count of myList[]
  if myList[i] % 2 is 0
    #This command removes the value from myList at the ith index
    set myList to myList - i
  else i++

#Now print the remaining items
for each i in 0..count of myList[] - 1
  set my display text to "myItem[" + i + "] = " + myList[i]
  wait 1
```

### Breaking a For Each Command
You can break out of the a For Each command using the ```break``` keywords.  This will immediately halt execution of the for each and proceed to the next command after the for each command.

```
set myList to []
for each i in 1..5
  if i % 4 is 0
    break
  increase myList by i
print "My List: " + myList
#My List: [1,2,3]
```

Breaking from a for each command only breaks out of the immediate For Each command.  If you perform a for each inside a for each, and break from the inner for each, it will continue iterating through the outer for each.

```
for each i in 0..3
  for each j in 0..3
    if i = j
      break
  print "Continuing..."
```

### Continuing a For Each Command
Sometimes you might want to keep iterating over a for each, but "skip" the current item.  You can do this with the ```continue``` keywords.

```
set myList to []
for each i in 1..5
  if i % 4 is 0
    continue
  increase myList by i
print "My List: " + myList
#My List: [1,2,3,5]
```

Continue acts similarly to break in that it only affects the immediate For Each command.

## Function Command
[Functions](https://spaceengineers.merlinofmines.com/EasyCommands/functions "Functions") allow you to create re-usable and invokable pieces of script which can be called from within your script, and from external sources (see [Invoking Your Script](https://spaceengineers.merlinofmines.com/EasyCommands/invoking "Invoking Your Script")).  

There are two ways to invoke a function in your script.  The first is to simply call it using its name.
```
#Call myFunction
myFunction

:myFunction
print "You called me!"
```

Function names are *case sensitive* so make sure you match the case of your requested function.  Optionally you can specify "call" to better indicate that you are calling a function: ```call myFunction```

When you call a function, the script will wait until the function is completely finished before executing the next statement.

### Function Parameters

You can also pass parameters to your functions.  Parameters get assigned to local thread variables before the function executes, so be careful about your parameter names as these *will* override any other variables of the same name.  See [Functions](https://spaceengineers.merlinofmines.com/EasyCommands/functions "Functions") for more information.

```
printSomething "Hello World!"

:printSomething something
print "Message: " + something
```

### Goto Function
By default, a function call is a "sub-routine" which continues executing the calling function after completing the subroutine.  The "goto" keywords enable you to immediately switch program execution without returning to the calling function.  This effectively enables switching behavior, as well as short circuiting:

```
set outOfAmmo to true
attack

:attack
if outOfAmmo
  goto flee
#This command will not be executed
print "Fire at will!"

:flee
print "Run away!"
```

If you load the above script into a new EasyCommands instance, you'll notice "[Main] flee" in the list of running commands.  This is because EasyCommands has completely switched execution to the flee program.  

## Queue & Async Commands
EasyCommands supports a concept for [Threads](https://spaceengineers.merlinofmines.com/EasyCommands/threads "Threads"), which allows you to execute multiple programs in parallel, or to queue up commands to run after the initial command is finished executing.

The general structure of this command is ```<queue/async> <command>```

### Queuing Commands

You can "queue" commands to run after we've finished running the current script.  You might use this if you create a factory that builds different types of vehicles, and you want to queue up 2 of one type and 1 of another.

```
:myFactory
queue makeRover
queue makeShip
queue makeRover
queue idle

:makeRover
set my display to "Building a Rover"
wait 2

:makeShip
set my display to "Building a Ship"
wait 2

:idle
set my display text to "Idle..."
```

### Async Commands
An asynchronous command is a command that you spin off to run on its own [Thread](https://spaceengineers.merlinofmines.com/EasyCommands/threads "Threads").  Asynchronous commands run in the same tick as the main script, so effectively they run in parallel.  Async commands allow a single program to run multiple scripts simultaneously so that 1 EasyCommands script can manage multiple things.  This is useful and powerful, but be careful about trying to run too much in 1 script!  Async commands will automatically end once they complete their task.  

See [Threads](https://spaceengineers.merlinofmines.com/EasyCommands/threads "Threads") for more information on how parameters are passed from the caller to the asynchronous threads and other information.

```
:setup
#Calling functions asynchronously
async runTask "Task1"
async runTask "Task2"

#Async command without needing a function
async
  set my display text to "Im running a background task!"
  wait 5
  set my display text to "Now Im done!"
  exit
goto main

:main
Print "Main function..."
repeat

:runTask taskName
Print "Running task: " + taskName
repeat
```

## Control Commands
As in the above example, you might find that sometimes you want to pause, stop, or restart your script

Stopping or Restarting a script will clear all running & asynchronous threads, and clear all local and global variables.

```
#Restart Script from the beginning
restart

#Stop Script immediately
exit

#Pause your script and all asynchronous threads.  When you run it again it will begin execution right where it left off (including asynchronous threads)
pause
```

### The Repeat Command
The "repeat" command is a special command often used for asynchronous threads to keep executing the given function.  The Repeat command effectively restarts the current "thread", rather than the whole program.



```
set my display to "Tick"
wait 1
set my display to "Tock"
wait 1
repeat
```

If you specify a "goto <function>" somewhere before calling "repeat", the script will repeat from the last "goto function".  Note that any previous parameters passed to that function are not passed when calling repeat.

## Send/Listen/Forget Commands

Sometimes you might want to send or listen for events from other grids.  You can do this from EasyCommands using the ```send``` and ```listen``` commands.  If later you want to stop listening for those events, you can do so with the ```forget``` command.

### Send Command

To send a message to another grid, you broadcast it to a channel.  The outline of the command is
```send <message> to <channel>```

You can also use this to send full commands to other EasyCommands scripts which are listening for commands.

```
send "Finished!" to myChannel
send 'open the "Outer Doors"' to myBaseChannel
```

### Listen Command
To listen for commmands from your script, you can use the listen command.  The outline of this command is
```listen <channel>```.

Keywords: ```listen, channel, register, subscribe```

Note that your script needs to be running in order to be triggered by an event from a channel.  So it's important to put the script in an "always listening" mode, which can be accomplished in several ways but the easiest would be as follows:

```
listen "myChannel"
wait until false
```

### Forget Command
To stop listening to a channel and ignore future messages from that channel, you can use the ignore command.  The outline of this command is
```forget <channel>```.

Keywords: ```forget, dismiss, ignore, deregister, unsubscribe```

```
#start listening
listen myChannel

#Do some stuff

#stop listening
forget myChannel
```

### Using Send/Listen commands together
You can quickly do some fun cross-grid things with these two commands.  Here's a simple example that enables keyless entry to your garage from your rover:

Base Script:
```
listen "myBaseChannel"
wait until false

:openDoors
open the "Garage Doors"
```

Rover Script:
```
:main
Print "doing whatever..."

#Create an item in your hotbar to run "openDoors" on your EasyCommands Programmable Block
:openDoors
send "openDoors " to "myBaseChannel"
```

For more information on Cross Grid communication and handling input in your EasyCommands script, see [Invoking Your Script](https://spaceengineers.merlinofmines.com/EasyCommands/invoking "Invoking Your Script")

### Transferring Items Across Inventories

You can transfer items between inventories using the Transfer Item Command.  The general outline is:
```
<transfer/give> <items> from <source> to <destination>
<transfer/give> <variable> <items> from <source> to <destination>
```

The transfer command will attempt to transfer as many items as possible from the sources to the destionations.  Be careful when using this command as it's easy to hit the Script Too Complex limit.  The transfer command is best effort and "fire & forget".  If you ask it to transfer 50 "gold ingot" and you only have 30, the command will transfer 30 ingots and complete successfully.

Here's a full example of transferring items in small batches, just for fun

```
set sourceInventories to "My Rover Inventories"
set destinationInventories to "My Base Inventories"
#Bind this as a calculation, so it auto decrements as we pull ore
bind amountToTransfer to $sourceInventories "ore" amount
until amountToTransfer <= 0
  transfer 100 "ore" from $sourceInventories to $destinationInventories
```

#### Transferring Custom Items

Custom Items can be transferred across items similarly to other supported item types.  Just make sure to use the correct TypeId and/or SubTypeIds when transferring items.

```
#HackingChip is the SubTypeId.  TypeId is not considered
transfer 100 "HackingChip" from $mySource cargo to $myDestination cargo

#MyObjectBuilder_Component is the TypeId (effectively the group of items).
transfer 100 "MyObjectBuilder_Component." from $mySource cargo to $myDestination cargo

#MyObjectBuilder_Component is the TypeId and HackingChip is the SubTypeId.
transfer 100 "MyObjectBuilder_Component.HackingChip" from $mySource cargo to $myDestination cargo
```

See [Items](https://spaceengineers.merlinofmines.com/EasyCommands/items "Items") for more information on dealing with custom item types.