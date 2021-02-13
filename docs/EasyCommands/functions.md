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

Just like a multi-command command, a function call made by a time-sensitive condition will execute the entire function before checking the condition again.  So the following command will always execute all of the "blink" logic before re-checking the "pistons" height.

```
until "pistons" height > 10
  call "blink"
  
:blink
turn on the "lights"
wait 1
turn off the "lights"
wait 1
```

This is true even if you make an async call to "blink". The only time when a function, once invoked, would not complete its execution is if either (1) the program is stopped or restarted 
(2) the program is given another command to run directly either through the console, another program block invocing it, or a command is given through a channel being listened to, which causes the program to stop executing any function.
(3) a goto <function> is invoked inside function itself, meaning it breaks immediately and begins executing the new function
(4) the function is called async, and one of the following commands invokes goto <function>, meaning it breaks immediately and begins executing the new function.
  
So let's say, for whatever reason, I *really* need to stop the execution of my function as soon as my condition is no longer met.  How can I do that?

You can accomplish this by making the function call async and then checking the condition again using a when clause:

```
:main
until "pistons" height > 9.9
  async call "function"
  when "pistons" height > 9.9 goto "stopFunction"

:function
turn on the "lights"
wait 1
turn off the "lights"
wait 1

:stopFunction
wait 1
```
