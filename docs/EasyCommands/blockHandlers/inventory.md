# Inventory Block Handler
This block handler can be used to get information about inventories for any blocks or block groups.

Note that this block handler does not extend from Terminal Block, so this Block Handler *does not* have properties defined in the Reference: [Terminal Block Handler](https://spaceengineers.merlinofmines.com/EasyCommands/blockHandlers/terminal "Terminal Block Handler").

Note that many blocks have multiple inventories, so make sure you are querying the right inventory when looking for items/capacity.

Reference: [Items](https://spaceengineers.merlinofmines.com/EasyCommands/blockHandlers/items "Items and Blueprints")

* Block Type Keywords: ```cargo, container, inventory, inventories```
* Block Type Group Keywords: ```containers```

Note that "inventories" is a keyword for Block Type, not Block Group Type.  This is because individual blocks often have more than 1 inventory (refineries, assemblers).  If you want to get all inventories from a group of items that aren't "Containers", use ```"My Block Group" group inventories```

Default Primitive Properties:
* String - Name
* Boolean - Show
* Numeric - Ratio

## "Name" Property
* Primitive Type: String
* Keywords: ```name```

Gets/Sets the name of the block for which this container belongs.  Most useful for re-naming Cargo Container objects.

```
Print "My name is: " + "My Cargo" name

set "My Cargo" name to "My New Name"
```

## "Show" Property
* Primitive Type: Bool
* Keywords: ```show, showing```
* Inverse Keywords: ```hide, hiding```

Gets/Sets whether the inventory shows up in the list of Inventories in the Terminal Menu.  Note that this property affects all inventories for the block that owns this inventory.

```
show "My Cargo"
hide "My Secre Treasure" containers
```

## "Limit" Property
* Read-only
* Primitive Type: Numeric
* Keywords: ```limit, limits```

This property returns the maximum cargo limit, in L. Note that this is a measure of Volume, not Mass.

```
Print "Cargo Limit: " + "My Cargo" limit
```

## "Volume" Property
* Read-only
* Primitive Type: Numeric
* Keywords: ```volume, volumes```

This proeptty returns the current cargo amount, in L.  Note that this is a measure of Volume, not Mass.

## "Ratio" Property
* Read-only
* Primitive Type: Numeric
* Keywords: ```ratio, ratios, percent, percents, percentage, percentages```

This property returns a value between 0 and 1 representing the % of cargo space that is currently in use.

```
Print "Cargo Percent used: " + "My Cargo" percent
```

## "Weight" Property
* Read-only
* Primitive Type: Numeric
* Keywords: ```weight, mass, level```

Returns the current mass of the inventory, in Kg.

```
Print "Inventory Weight: " + "My Cargo" weight
```

## "Amount" Property
* Requires a String Attribute indicating the [Item(s)](https://spaceengineers.merlinofmines.com/EasyCommands/items "Items & Blueprints") to get amounts for.
* Read-only
* Primitive Type: Numeric
* Keywords: ```amount, amounts```

This property will retrieve the amount (in units or kg) of items matching the ItemFilter attribute that are present in the given inventory/inventories.

This can be an expensive operation across a large set of inventories; be careful about trying to get inventory amounts of too many inventories in a single command.  Instead, iterate through them and gradually calculate the total, if need be.

```
set goldAmount to the "gold ingot,gold ore" amount in the "Treasure Container"
set silverAmount to the "Treasure Container" "silver ingot, silver ore" amount
set oreAmount to the "Treasure Container" "ore" amount
```

Check out [Items](https://spaceengineers.merlinofmines.com/EasyCommands/items "Items & Blueprints") for more information on how to specify Item Filters.