# Gun Block Handler
This Block Handler can be used to control all block weapons, including gatling guns, rocket launchers, and turrets.

There is also a separate [Turret Block Handler](https://spaceengineers.merlinofmines.com/EasyCommands/blockHandlers/turret "Turret Block Handler") which has additional properties for turrets in addition to the below properties.

* Block Type Keywords: ```gun, rocket, missile, launcher```
* Block Type Group Keywords: ```guns, rockets, missiles, launchers```

Default Primitive Properties:
* Boolean - Enabled
* Numeric - Range

Default Directional Properties
* Up - Range

## "Enabled" Property
* Primitive Type: Bool
* Keywords: ```enable, enabled```
* Inverse Keywords: ```disable, disabled```

Enables or Disables the given block

```
#Enable Block
enable "My Gun"
set "My Gun" to enabled
turn on "My Gun"

#Disable Block
disable "My Gun"
set "My Gun" to disabled
turn off "My Gun"
```

## "Power" Property
* Primitive Type: Bool
* Keywords: ```power, powered```

Turns on or off power to the block.  Effectively the same as the Enabled property.

```
#Turn on
turn on power to "My Gun"
power on "My Gun"

#Turn off
turn off "My Gun"
power off "My Gun"
```

## "Shoot" Property
* Primitive Type: Bool
* Keywords: ```shoot, shooting, trigger, triggered```

Gets/Sets whether the gun is shooting.  This property tell the block to continuously fire (vs shooting once).

```
if "My Gun" is shooting
  Print "Drop some lead!"

#Start Shooting
tell "My Gun" to shoot

#Stop Shooting
tell "My Gun" to stop shooting
```