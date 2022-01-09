# Debugging Your Script

Rarely will you get your script perfect the first time you write it.  Some errors are obvious, given a hint at where the problem is at.

Others take a bit more investigation to understand what's going wrong.

## Dealing with Parsing Issues

The first error you'll likely encounter is a parsing issue, meaning EasyCommands can't parse your script.

Your script may not parse for a variety of issues.  When your script can't be parsed, EasyCommands will halt the script and tell you what line number cannot be parsed.

Lines start at 1, so just count down from the top the number of lines until you find the error.  Also feel free to copy/paste into a text editor to quickly find the line number, for longer scripts.

### Isolating the broken lines
If your script is having trouble parsing a particular line or lines, try commenting out everything else (or save a copy and then delete it).  This will help you get to the root cause more quickly.

### Setting Debug Log Level
If isolating alone doesn't help, try setting the Log Level of EasyCommands to DEBUG.  You can do this by editing the script and changing the line

```public LogLevel logLevel = LogLevel.INFO;```

to

```public LogLevel logLevel = LogLevel.DEBUG;```

EasyCommands will debug how each line was parsed into a command, which should give you a clue about whether EasyCommands parsed your script as you intended.

### Wrapping with Parentheses
Most things in EasyCommands can be wrapped with parentheses.  If the way your commands are parsed might be ambiguous, try wrapping specific pieces with parentheses to help EasyCommands parse correctly.

### Common Mistakes
* Trying to use a reserved keyword (like a block type or operation name) as a variable name
* Naming a variable the same as a function
* Forgetting to specify a block type for selectors that don't include the correct block type in their name(s).

## Dealing With Execution Issues
Sometimes your script will parse just fine but then will error out during execution

### Script Too Complex

If you receive the "script too complex" issue, a couple things might be happening.

Most likely, your script is trying to do too many things in a single tick.  To fix this, break up your script into pieces.

The easiest way is to add a ```wait``` command.  This will pause the script for 1 tick, and continue executing on the next tick.

Another possibility is that you are trying to call a function from within itself, or you have a chain of functions calling each other in a cycle.

### Common Mistakes
* My block(s) aren't responding to the command
  * Most likely, your block names and/or block types are incorrect.  It could also be that your blocks are in a group and you aren't specifying to request a group, or vise versa.  See [Selectors](https://spaceengineers.merlinofmines.com/EasyCommands/blockHandlers/selectors "Selectors") for more information.
* Operation cannot be performed on types
  * Likely the variable(s) you are using are not the type you are expecting.  Try printing out the variable using ```Print myVariable```  to see it's value.

### Common debugging steps
* Print out the result of your operation(s) variables and make sure they are what you expect
* Wrap calculations in parantheses to clarify order of operations.
* Split up pieces into functions and call them directly to make sure they do what you expect.
* Make sure your selectors are working by printing out their names:
  * ``` Print $mySelector names ```, or
  * ```Print mySelector blockType names```

### Pausing/Slowing Down Script Execution
Sometimes your program may run too fast to see what's happening effectively.

There are a couple things you can do to slow down execution and see what's happening.

First, you can use the "Pause" command anywhere to pause the script.  This will pause the script in its exact state, and will pick up where you left off when you run the program again (without an argument) hitting "run".  In this way, you can pause execution at a given moment, maybe right after you print some output to the screen.

You can also set the Runtime Frequency of the script manually.  "Edit" the program and change execution to your desired frequency:

```
public UpdateFrequency updateFrequency = UpdateFrequency.Update10;
```

Supported Values:
* ```UpdateFrequency.Update1``` - Script runs once every tick (the default)
* ```UpdateFrequency.Update10``` - Script runs once every 10 ticks
* ```UpdateFrequency.Update100``` - Script runs once every 100 ticks
* ```UpdateFrequency.None``` - Script runs 1 tick per time you click "Run".  Effectively pasues after each execution.
* ```UpdateFrequency.Once``` - Script runs 1 tick per time you click "Run".  Effectively pasues after each execution.