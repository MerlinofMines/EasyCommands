# Connector Block Handler
This Block Handler handles Connectors, which allow you to automatically connect and disconnect grids together.

* Block Type Keywords: ```connector```
* Block Type Group Keywords: ```connectors```

Default Primitive Properties:
* Numeric - Strength

## "Enabled" Property
* Primitive Type: Bool
* Keywords: ```enable, enabled```
* Inverse Keywords: ```disable, disabled```

Enables or Disables the given block

```
#Enable Block
enable "My Connector"
set "My Connector" to enabled
turn on "My Connector"

#Disable Block
disable "My Connector"
set "My Connector" to disabled
turn off "My Connector"
```

## "Power" Property
* Primitive Type: Bool
* Keywords: ```power, powered```

Turns on or off power to the block.  Effectively the same as the Enabled property.

```
#Turn on
turn on power to "My Connector"
power on "My Connector"

#Turn off
turn off "My Connector"
power off "My Connector"
```

## "Connect" Property
* Primitive Type: Bool
* Keywords: ```connect, connected, attach, attached, dock, docked```
* Inverse Keywords: ```disconnect, disconnectd, detach, detached, undock, undocked```

Gets/Sets whether the connector is connected.  A connector is connected if it's Connector status is "Connected".

When setting the property, the connector will attempt to connect or disconnect, depending on if the supplied value is true/false, respectively.  

The connector only attempts to connect/disconnect once.  It does not continue to attempt, or wait for a connection/disconnection.  However, you can use a conditional check to continually attempt to connect/disconnect.

```
if "My Connector" is connected
  Print "I am connected!"

#Try to connect until connected
until "My Connector" is connected
  connect "My Connector"

#Disconnect
disconnect "My Connector"
```

## "Locked" Property
* Primitive Type: Bool
* Keywords: ```lock, locked```
* Inverse Keywords: ```unlock, unlock```

Identical to the Connect Property

## "Strength" Property
* Primitive Type: Numeric
* Keywords: ```strength, strengths, force, forces```

Gets/Sets the strength of the Connector, as a percentage between 0 and 1 (representing 0% and 100%).  The Connector strength determines how hard the connector pulls on other connectors in proximity in order to connect.

## "Drain" Property
* Primitive Type: Bool
* Keywords: ```drain, draining```

Gets/Sets whether the connector is throwing out items (acting like an ejector).

```
if "My Connector" is draining
  Print "Throwing out the garbage"

tell "My Connector" to drain
drain "My Connector"
```

## "Collect" Property
* Primitive Type: Bool
* Keywords: ```collect, collecting, consume, consuming, gather, gathering```

Gets/Sets whether the connector is automatically pulling items from connected inventories (acting like a Sorter, but without a filter).

```
if "My Connector" is collecting
  Print "Grabbing all the items."

tell "My Connector" to gather
tell "My Connector" to stop collecting
```
