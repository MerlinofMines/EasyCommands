# Items and Blueprints

EasyCommands provides a set of user friendly words for retrieving almost all items in the version of the game.

These user friendly words can be used for Items and Blueprints.  Items includes keywords for each unique item in the game, as well as keywords for groups of related items (ore, weapons, tools, etc).

For more information on how to use items and blueprints, see the [Inventory](https://spaceengineers.merlinofmines.com/EasyCommands/blockHandlers/inventory "Inventory Handler") and [Assembler](https://spaceengineers.merlinofmines.com/EasyCommands/blockHandlers/assembler "Assembler Block Handler") Block Handlers.

## Items

Items can be used when fetching items from inventories, transferring items between inventories

### Specifying Multiple Items

You can specify multiple items to be retrieved or transferred by comma separating them inside of quotes.

```
transfer "ice,nickel ore,cobalt ore" from "Inventory 1" to "Inventory 2"
```

### Dynamic Item Names
EasyCommands has keyword mappings for all of the base game items.  To work with other items from installed mods, you will need to specify the TypeId and/or SubTypeId of the item directly.  If EasyCommands does not have a built in mapping for the items specified, it assumes the passed in item is a custom item instead and will look it up directly by TypeId/SubTypeId.

The general format is ```TypeId.SubTypeId```, which is case sensitive.

```
print "Hacking Chip Amount: " + the "my cargo" "MyObjectBuilder_Component.HackingChip" amount
```

You don't have to pass the TypeId, in which case any item regardless of "typeId" will be matched by SubTypeId

```
print "Hacking Chip Amount: " + the "my cargo" "HackingChip" amount
```

To pass only a TypeId, specify the TypeId followed by a "."

```
print "Component Type Amount: " + the "my cargo" "MyObjectBuilder_Component." amount
```

#### Getting Dynamic Item TypeIds and SubTypeIds
You can use the "Types" property of any inventory to get a list of item types in a given inventory, in the form ```TypeId.SubTypeId```.

The returned list will be a list of the TypeId.SubTypes for all items in the inventory, with duplicates removed.

```
set myInventoryTypes to "My Inventory" types

print "Inventory Types:\n" + myInventoryTypes joined "\n"
```

### Supported Item Names

#### Ores

All Ores (includes Ice): ```ore```

* Cobalt - ```cobalt ore```
* Nickel - ```nickel ore```
* Gold - ```gold ore```
* Ice - ```ice```
* Iron - ```iron ore```
* Magnesium - ```magnesium ore```
* Platinum - ```platinum ore```
* Silicon - ```silicon ore```
* Silver - ```silver ore```
* Stone - ```stone```
* Scrap Metal - ```scrap metal```
* Uranium - ```uranium ore```

#### Ingots

All Ingots: ```ingots```

* Cobalt - ```cobalt ingot```
* Nickel - ```nickel ingot```
* Gold - ```gold ingot```
* Gravel - ```gravel```
* Iron - ```iron ingot```
* Magnesium - ```magnesium ingot, magnesium powder```
* Platinum - ```platinum ingot```
* Silicon - ```silicon ingot, silicon wafer```
* Silver - ```silver ingot```
* Old Scrap Metal - ```old scrap metal```
* Uranium - ```uranium ingot```

#### Components
All Components: ```components```

* Bulletproof Glass - ```bulletproof glass```
* Canvas - ```canvas```
* Computer - ```computer```
* Construction - ```construction component```
* Detector - ```detector component```
* Display - ```display```
* Explosives - ```explosives```
* Girder - ```girder```
* GravityGenerator - ```gravity component```
* InteriorPlate - ```interior plate```
* LargeTube - ```large steel tube```
* Medical - ```medical component```
* MetalGrid - ```metal grid```
* Motor - ```motor```
* PowerCell - ```power cell```
* RadioCommunication - ```radio component```
* Reactor - ```reactor component```
* SmallTube - ```small steel tube```
* SolarCell - ```solar cell```
* SteelPlate - ```steel plate```
* Superconductor - ```superconductor```
* Thrust thruster - ```component```
* ZoneChip - ```zone chip```


#### Tools
All Tools: ```tools```

Grinders:
* Basic Grinder - ```grinder```
* Enhanced Grinder - ```enhanced grinder```
* Proficient Grinder - ```proficient grinder```
* Elite Grinder - ```elite grinder```

Welders:
* Basic Welder - ```welder```
* Enhanced Welder - ```enhanced welder```
* Proficient Welder - ```proficient welder```
* Elite Welder - ```elite welder```

Drills:
* Basic Drill - ```drill```
* Enhanced Drill - ```enhanced drill```
* Proficient Drill - ```proficient drill```
* Elite Drill - ```elite drill```

#### Bottles
All bottles: ```bottles```
* Hydrogen Bottle: ```hydrogen bottle```
* Oxygen Bottle: ```oxygen bottle```

#### Weapons
All Weapons: ```weapons```

Rifles:
* Basic Rifle - ```rifle```
* Rapid-Fire Rifle - ```rapid rifle```
* Precision Rifle - ```precision rifle```
* Elite Rifle - ```elite rifle```

Pistols:
* Basic Pistol - ```pistol```
* Rapid-Fire Pistol - ```rapid pistol```
* Elite Pistol - ```elite pistol```

Rocket Launchers:
* Basic Rocket Launcher - ```rocket launcher```
* Precision Rocket Launcher - ```precision rocket launcher```

#### Ammo

All Ammo: ```ammo, ammunition```

Turrets:
* Missile 200mm - ```missile ammo```
* Nato 25x184mm - ```gatling ammo```

Rifles:
* Basic Rifle Ammo: ```rifle ammo```
* Rapid-Fire Rifle Ammo: ```rapid rifle ammo```
* Precision Rifle Ammo: ```precision rifle ammo```
* Elite Rifle Ammo: ```elite rifle ammo```

Pistols:
* Basic Pistol Ammo: ```pistol ammo```
* Rapid-Fire Pistol Ammo: ```rapid pistol ammo```
* Elite Pistol Ammo: ```elite pistol ammo```

#### Consumables
All Consumables: ```consumables```
* Clang Cola - ```clang cola```
* Cosmic Coffee - ```cosmic coffee```
* Medkit - ```medkit```
* Powerkit - ```powerkit```

#### Misc
* Package - ```package```
* Datapad - ```datapad```
* Space Credit - ```space credit```

## Blueprints

Blueprints can be used to get or create items in Assembler Block Queues.  The words used, for supported blueprints, are the same as for the items.

You can specify multiple blueprints to be retrieved or transferred by comma separating them inside of quotes:

```
tell "My Assembler" to produce "elite grinder,elite welder,elite rifle"
```


### Dynamic Blueprints
EasyCommands has keyword mappings for all of the base game blueprints.  To work with other items from installed mods, you will need to specify the BlueprintId of the item directly.  If EasyCommands does not have a built in mapping for the blueprint specified, it assumes the passed in blueprint is a custom blueprint instead and will look it up directly by BlueprintId.

Note that BlueprintIds are case sensitive.

```
print "Hacking Chip Production Amount: " + "My Assembler" "HackingChip" amount
```

You can also use this to create custom items, or get the amount of a custom item currently being produced:

```
tell "My Assembler" to create 10 "HackingChipComponent"

print "Production Amount: " + "My Assembler" "HackingChipComponent" amount
```

If you attempt to create an unknown custom blueprint you will get a script halting exception, so..don't do that.

#### Getting Dynamic BlueprintIds
You can use the "Types" property of any assembler to get a list of BlueprintIds currently being produced by the given assembler.

The returned list will be a list of the BlueprintIds for all items in the assembler production queue, with duplicates removed.

The resulting blueprintIds are the Ids you should specify when retrieving or creating custom blueprints from an assembler.

```
set myProducingTypes to to "My Assembler" types

set outputItems to []

for each customItem in myProducingTypes
  outputItems += [customItem->"My Assembler" customItem amount]

print "Producing Types:\n" + myProducingTypes joined "\n" + "\n"

print outputItems
```

### Supported Blueprints
Below are the list of supported blueprints.

Note that item groups are not supported for Blueprints (unlike for items).

#### Components
* Bulletproof Glass - ```bulletproof glass```
* Canvas - ```canvas```
* Computer - ```computer```
* Construction - ```construction component```
* Detector - ```detector component```
* Display - ```display```
* Explosives - ```explosives```
* Girder - ```girder```
* GravityGenerator - ```gravity component```
* InteriorPlate - ```interior plate```
* LargeTube - ```large steel tube```
* Medical - ```medical component```
* MetalGrid - ```metal grid```
* Motor - ```motor```
* PowerCell - ```power cell```
* RadioCommunication - ```radio component```
* Reactor - ```reactor component```
* SmallTube - ```small steel tube```
* SolarCell - ```solar cell```
* SteelPlate - ```steel plate```
* Superconductor - ```superconductor```
* Thrust thruster - ```component```
* ZoneChip - ```zone chip```

#### Tools
Grinders:
* Basic Grinder - ```grinder```
* Enhanced Grinder - ```enhanced grinder```
* Proficient Grinder - ```proficient grinder```
* Elite Grinder - ```elite grinder```

Welders:
* Basic Welder - ```welder```
* Enhanced Welder - ```enhanced welder```
* Proficient Welder - ```proficient welder```
* Elite Welder - ```elite welder```

Drills:
* Basic Drill - ```drill```
* Enhanced Drill - ```enhanced drill```
* Proficient Drill - ```proficient drill```
* Elite Drill - ```elite drill```

#### Bottles
* Hydrogen Bottle: ```hydrogen bottle```
* Oxygen Bottle: ```oxygen bottle```

#### Weapons
Rifles:
* Basic Rifle - ```rifle```
* Rapid-Fire Rifle - ```rapid rifle```
* Precision Rifle - ```precision rifle```
* Elite Rifle - ```elite rifle```

Pistols:
* Basic Pistol - ```pistol```
* Rapid-Fire Pistol - ```rapid pistol```
* Elite Pistol - ```elite pistol```

Rocket Launchers:
* Basic Rocket Launcher - ```rocket launcher```
* Precision Rocket Launcher - ```precision rocket launcher```

#### Ammo
Turrets:
* Missile 200mm - ```missile ammo```
* Nato 25x184mm - ```gatling ammo```

Rifles:
* Basic Rifle Ammo: ```rifle ammo```
* Rapid-Fire Rifle Ammo: ```rapid rifle ammo```
* Precision Rifle Ammo: ```precision rifle ammo```
* Elite Rifle Ammo: ```elite rifle ammo```

Pistols:
* Basic Pistol Ammo: ```pistol ammo```
* Rapid-Fire Pistol Ammo: ```rapid pistol ammo```
* Elite Pistol Ammo: ```elite pistol ammo```