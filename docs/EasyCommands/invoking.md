# Invoking Your Script
EasyCommands scripts can be invoked just like any Programmable Block.  EasyCommands will parse any passed in argument as a command to be executed.  This can be used to execute your functions, directly run commands, halt execution, etc.

Below are some options for how you can invoke your script.

# From The Programmable Block Menu
From the Programmable Block menu, you can use the run argument to specify the command you want EasyCommands to execute, which could be a direct command or a function call:
Example Run Arguments:
```
turn on the "Exterior Lights"
```
```
doSomething
```
```
exit
```

# From a Button or Timer Block
From a Button or Timer block, select your target Programmable Block and choose an action "Run".  You can use the argument to specify the command you want EasyCommands to execute, which could be a direct command or a function call:
Example Run Arguments:
```
turn on the "Exterior Lights"
```
```
doSomething
```

# From Another EasyCommands Script
EasyCommands has a Block Handler for Programmable Blocks which makes it easy to invoke a Programmable Block from an EasyCommands Script.  To run the block with the default argument, you can say:
```
run "My Other Program"
```

And to run that program with an argument, you could say:
```
tell "My Other Program" to run "doSomething"
```

# From Another Grid (Cross Grid Communication)
EasyCommands makes Cross Grid Communication pretty easy.  First, you'll need to set up your receiving EasyCommands Program to listen for events on a given channel using the Listen Command

```
listen myChannel
#This part is important. Your script needs to be actively running to get commands from the sender!
wait until false

:doSomething
print "I did something!"
```

Then, from the sending script, you will need to send a message to that channel with the command you wish to execute:

```
send 'doSomething' to myChannel
```
If done correctly, the receiving EasyCommands script will interpret the message as a command to execute, and then execute the command just like it was coming from a button or other source.  This means you can either send direct commands to the block (```turn on the lights```) or ask it invoke functions (```doSomething```).

Check out the [Garage Door Example](https://spaceengineers.merlinofmines.com/EasyCommands/examples "Examples") for a full example using cross grid communication.


