# Programmable Block Handler
This Block Handler handles Programmable Blocks.  It allows you to get the running state of a given program and also ask it to execute a script.

* Block Type Keywords: ```program```
* Block Type Group Keywords: ```programs```

Default Primitive Properties:
* String - Run

## "Enabled" Property
* Primitive Type: Bool
* Keywords: ```enable, enabled```
* Inverse Keywords: ```disable, disabled```

Enables or Disables the given block

```
#Enable Block
enable "My Program"
set "My Program" to enabled
turn on "My Program"

#Disable Block
disable "My Program"
set "My Program" to disabled
turn off "My Program"
```

## "Power" Property
* Primitive Type: Bool
* Keywords: ```power, powered```

Turns on or off power to the block.  Effectively the same as the Enabled property.

```
#Turn on
turn on power to "My Program"
power on "My Program"

#Turn off
turn off "My Program"
power off "My Program"
```

## "Run" Property
* Primitive Type: Bool / String
* Keywords: ```run, running, execute, executing, script```

Gets/Sets whether the Program is running.  When retrieving, only Bool is supported.

When setting, if a boolean is set then the program will try to run with no argument ("") when true and disable the block when false.

If a string is set, then the program will try to run with the given argument.

```
if "My Program" is running
  Print "Server is humming"

#Run with "" argument
tell "My Program to run

#Run "doSomething"
tell "My Program" to run "doSomething"

#Stop running
tell "My Program" to stop running
```

## "Complete" Property
* Read-only
* Primitive Type: Bool
* Keywords: ```done, ready, finished, complete```

Gets whether the program is currently running or not.

```
when "My Program" is complete
  Print "Finally, that took forever!"
```

## "Text" Property
* Read-only
* Primitive Type: String
* Keywords: ```text```

Gets the default terminal argument.

```
Print "Default Argument: " + "My Program" text
```
