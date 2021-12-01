# Parsing Your Script

After loading EasyCommands onto your Programmable Block, EasyCommands will immediately try to parse the Custom Data for that Programmable Block.  If the Custom Data is empty you will get a message prompting you to create a script.

## Dealing with Parsing Errors

If EasyCommands cannot parse the script you provided, it will tell you which line of your script could not be parsed.  You will need to find the failing line and fix the error.  After fixing the error you will need to click "Run" to get EasyCommands to re-parse your program.  You **do not need to re-compile** to fix parsing errors.

## Automatic Re-parsing

Every time EasyCommands executes, it will inspect the CustomData to see if it has changed since the last execution.  If the CustomData has changed, EasyCommands will halt all execution, resets it's state, and re-parse your script.  So if you decide to make a change to your script, you can simply update the Custom Data and EasyCommands will automatically update the script to reflect your changes.  Note that this means EasyCommands will reset back to the default [Function](https://spaceengineers.merlinofmines.com/EasyCommands/functions "Functions").

## Progressive Parsing

EasyCommands progressively parses your script to avoid performance impact and to avoid the dreaded "Script Too Complex" issue during parsing.  As such there are no limits on function length or the number of commands any function can have.  That said, your script might still not be able to execute everything in a single tick even if it's able to parse it.  If you find yourself executing too many commands in a single tick and you get the "Script Too Complex" error during execution, consider breaking up your commands into multiple ticks.  The simplest way to do this is to add a ```wait``` command somewhere in your script.

