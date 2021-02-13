# Multi Commands
 
## Overview

Multi-commands are a way of executing multiple commands, sequentially, as a unit.  This allows you to execute a series of commands when a condition is met (or not met). I highly recommend using
tabbing to identify multi-commands and ensure proper command parsing, though it is possible simply using "and"

See Functions for even more things you can do using Multi-commands.

## Using Tabs

The simplest way to lump together a series of commands is to use tabbing:

```
if "Alarm sensor" is triggered
  turn on the "warning lights"
  close the "blast doors"
  turn on the "sentry guns"
  
```

### Nesting Tabs

You can nest tabs pretty easily.  Just make sure that all the items intended to be part of one multi-command have the same tabbing

```
if "outer door" is open
  turn on the "outer lights"
  if "inner door is open"
    turn on the "inner lights"
    turn on the "generator"
  else
    close the "outer door"
    wait 1
  wait 3
```

## Using "and" and parenthesis

You can also execute multiple commands in a single line using "and", but its parsing isn't going to be near as accurate.  I'd keep this to pretty simple cases if I was you.

```
if the "outer door" is open then turn on the "lights" and wait 1 second and turn off the "lights" and wait 1 second
```

is equivalent to

```
if the "outer door" is open
  turn on the "lights"
  wait 1 second
  turn off the "lights"
  wait 1 second
```



