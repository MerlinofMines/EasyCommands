# Functions

## Overview 
Functions allow you to bundle together a set of commands into a single command which you can invoke from other commands, or other functions.

Functions also break up your code into nice re-usable and readable sections.

Lastly, functions help overcome some of the limitations with complexity with scripting.  If you find that your script is not able to compile and you are getting "script too complex" errors, try breaking up your changes using functions.  The command parser for EasyCommands is written to only parse 1 function per tick, so it will iteratively parse through your program until the full program is loaded up.  That way your complexity can run wild! There is no limit on the number of functions you can define in a program.  Beware of using reserved keywords for your function names as they might not always work.  

## Syntax 
It's pretty easy to create functions.  begin a line with a colon (:) followed immediately by your function name (non case sensitive, no spaces).  

```
:functionName
set the "pistons" height to 10
until the "pistons" height > 9.9
  turn on the "lights"
  wait 1 second
  turn off the "lights"
  wait 1 second
```

Functions can also take in a list of parameters.  Simply add the names of the parameters (with or without quotes) after the function like below.  Just be sure to watch out for reserved keywords, as these will need to be wrapped in quotes.

```
:sendMessage message channel 
Print 'Sending Message: "' + message + '" to channel: ' + channel
send message to channel
Print "Message Sent"
```

Function parameters are always passed by value, not reference.  So the following script would print "myValue = 0".

```
set myValue to 0
call updateValue myValue
print "myValue = " + myValue

:updateValue valueInput
assign valueInput to valueInput + 1
```

## Calling a Function

### "call"
You can invoke a function a couple different ways.  The most common is to simply make a blocking call to the function, wait for it to finish executing, and then proceed to the next command.  You can do this using the "call" or "gosub" commands, followed by the function name usually in parenthesis.

```
call "myFunction"
gosub "myFunction"
```

### "goto"
You can also ask the program to entirely switch execution, completely stopping any current function execution and switching to a different function. You can do this by specifying "goto" followed by the function name.

```
goto "myFunction2"
```

This is effectively changing what function is currently considered "active" by the program.  The current function, no matter its state of execution, is abandoned in favor of the new active program.  Furthermore, any commands to "restart", "loop", "start" will now restart the new function.  This behavior effectively enables "modes" for your program to run in where the modes are intended to be exclusive. 

### Calling Functions Dynamically
It is possible to invoke a named function using a variable specifying the name of the function to invoke by explicitly using ```call``` or ```goto```.  This provides an easy way to create modes for your program.  Make sure to pass the same number of parameters to the function you intend to invoke.

```
set global runningFunction to "attack"

#Spin off a separate thread that's actually running the program
async runProgram

#Tie this to a button
:switchMode mode
set global runningFunction to mode

#Dynamically resolve the function to call using the runningFunction variable
:runProgram
call runningFunction
replay

#Mode 1
:attack
Print "Attacking the enemy!"

#Mode 2
:flee
Print "Fleeing!"
```

Note that since the function to call isn't known until your script is executing, you won't be told about potentially invalid function calls during parsing.  

If you try to invoke a function that is not defined, or try to invoke a defined function with the wrong number of parameters, you will get a script halting exception.

## Returning from a function
Sometimes you might want to return from a function early (meaning if some condition is met, short circuit).  You can return from a function using the ```return``` keywords.

```
#Doesn't do anything
doSomething 2

#Prints "Input is: 3"
doSomething 3

:doSomething myInput
if myInput is 2
  return

Print "Input is: " + 3
```

A more interesting example using this would be to end an asynchronous thread based on a condition checked elsewhere:

```
set global stopListening to false
async doSomething
wait 3 seconds
set global stopListening to true

:doListen
if stopListening
  return
Print "Doing something!"
replay
```

You cannot return a value as part of the return statement (like you can do in some languages).  Return is only for short-circuiting evaluation.

If you want to "return a value" from a function, just set a variable with the name you desire and you can reference it after the function completes:

```
calculateValue 4
Print "Return Value: " + returnValue
#Return Value: 8

:calculateValue myValue
set returnValue to myValue * 2
```

## Multi-Threading
As with any command you can call functions asynchronously.  This effectively enables "threads" on your program so you can do multiple things with one program.  This is useful for doing things like controlling multiple airlocks from one program, or whatever else you'd like to have your program multi-task.  

```
async call "myfunction"
async call "myfunction2"
async call "myfunction3"
```

## The "main" function

If you don't start your program with a function name, all the commands up until the first ":functionName" are lumped into a default function named "main".  You are welcome to label your program with a different default function name.  In either case, the first function defined in your program will be executed when the program is executed if no function name or specific command is specified on the input.  

## Conditional Calls to Functions

All commands inside of a conditional command will be executed before checking the condition again.  So the following command will always execute all of the "blink" logic before re-checking the "pistons" height.

```
until "pistons" height > 10
  call "blink"
  
:blink
turn on the "lights"
wait 1
turn off the "lights"
wait 1
```

The only time when a related list of commands, once invoked, would not complete their execution is if one of the following occurs:
* the program is stopped or restarted 
* a "return" command is executed inside of a function
* a "break" command is executed inside of a condition or for each command
* a "goto functionName" command is executed, meaning it breaks immediately and begins executing the new function