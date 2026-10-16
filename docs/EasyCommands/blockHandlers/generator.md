# O2/H2 Generator Block Handler
This Block Handler handles O2/H2 Generators.  It enables you to turn them on or off, and set them to auto-refill bottles or not.

* Block Type Keywords: ```generator, oxygen generator, hydrogen generator```
* Block Type Group Keywords: ```generators, oxygen generators, hydrogen generators```

## Selecting blocks and groups

Spaced keywords work both in double-quoted names and as explicit types after a name. Add `s` to the last word for a block group.

```
turn on "Hydrogen generator"
turn on "Storage" oxygen generator
turn on "Backup" hydrogen generators
```

`oxygen generator` and `hydrogen generator` are aliases for the same `generator` handler. They do not filter by gas type. Use the block name or a dedicated group to select the intended blocks.

Default Primitive Properties:
* Bool - Enabled

## "Enabled" Property
* Primitive Type: Bool
* Keywords: ```enable, enabled```
* Inverse Keywords: ```disable, disabled```

Enables or Disables the given block

```
#Enable Block
enable "My Generator"
set "My Generator" to enabled
turn on "My Generator"

#Disable Block
disable "My Generator"
set "My Generator" to disabled
turn off "My Generator"
```

## "Power" Property
* Primitive Type: Bool
* Keywords: ```power, powered```

Turns on or off power to the block.  Effectively the same as the Enabled property.

```
#Turn on
turn on power to "My Generator"
power on "My Generator"

#Turn off
turn off "My Generator"
power off "My Generator"
```

## "Auto" Property
* Primitive Type: Bool
* Keywords: ```auto, refill```

Gets/Sets whether the Generator is set to auto-refill bottles.

## Production status

`running` is a read-only Boolean reading whether the generator is producing. [Irrigation System](https://spaceengineers.merlinofmines.com/EasyCommands/blockHandlers/irrigation) extends this handler and remains selected by `generator`. Its inherited bottle-refill option is not automatic watering.
