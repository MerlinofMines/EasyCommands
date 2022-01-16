# Cryo Chamber Block Handler
This BlockHandler supports Cryo Chambers, which are technically also a type of Cockpit.  This page will list the typically used properties for Cryo Chamber, but technically those from the [Cockpit Handler](https://spaceengineers.merlinofmines.com/EasyCommands/blockHandlers/cockpit "Cockpit Handler") can also be used (though not all may work as expected).

* Block Type Keywords: ```cryo```
* Block Type Group Keywords: ```cryos```

Note that "chambers" is a block group keyword, so you can also use "cryo chambers" to indicate a group of cryo chambers.

## "Occupied" Property
* Read-only
* Primitive Type: Bool
* Keywords: ```use, used, occupy, occupied, control, controlled```
* Inverse Keywords: ```unused, unoccupied, available```

Returns whether the cryo chamber is currently occupied.

```
Print "Occupied: " + "My Cryo Chamber" is occupied
Print "In Use: " + "My Cryo Chamber" is in use
Print "Controlled: " + "My Cryo Chamber" is being controlled
```

## "Ratio" Property
* Read-only
* Primitive Type: Bool
* Keywords: ```ratio, ratios, percentage, percentages, percent, percents```

Gets the current Oxygen Ratio of the cryo chamber, as a value from 0 - 1 where 1 = 100%.

```
Print "Oxygen Ratio: " + "My Cryo Chamber" ratio
```

## "Capacity" Property
* Read-only
* Primitive Type: Numeric
* Keywords: ```limit, limits, capacity, capacities```

Gets the Maximum Oxygen Capacity of the cryo chamber, in L.  

```
Print "Oxygen Capacity: " + "My Cryo Chamber" capacity
```