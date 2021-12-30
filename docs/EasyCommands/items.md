# Items and Blueprints

EasyCommands provides a set of user friendly words for retrieving almost all items in the version of the game.\

These user friendly words can be used for Items and Blueprints.  Items includes keywords for each unique item in the game, as well as keywords for groups of related items (ore, weapons, tools, etc).

For more information on how to use items and blueprints, see the [Inventory](https://spaceengineers.merlinofmines.com/EasyCommands/blockHandlers/inventory "Inventory Handler") and [Assembler](https://spaceengineers.merlinofmines.com/EasyCommands/blockHandlers/assembler "Assembler Block Handler") Block Handlers.

## Items

Items can be used when fetching items from inventories, transferring items between inventories

### Specifying Multiple Items

You can specify multiple items to be retrieved or transferred by comma separating them inside of quotes.

```
transfer "ice,nickel ore,cobalt ore" from "Inventory 1" to "Inventory 2"
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