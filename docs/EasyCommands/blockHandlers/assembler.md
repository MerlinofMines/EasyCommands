# Assembler Block Handler
This block handler controls Assemblers.  With this Block Handler you to control whether a given assembler is creating, destroying, in cooperative mode.
Most importantly, this Block Handler allows you create and destroy items in your script.

For more information on what items you can create or destroy, see [Items & Blueprints](https://spaceengineers.merlinofmines.com/EasyCommands/items "Items & Blueprints")

* Block Type Keywords: ```assembler```
* Block Type Group Keywords: ```assemblers```

## "Enabled" Property
* Primitive Type: Bool
* Keywords: ```enable, enabled```
* Inverse Keywords: ```disable, disabled```

Enables or Disables the given block

```
#Enable Block
enable "My Assembler"
set "My Assembler" to enabled
turn on "My Assembler"

#Disable Block
disable "My Assembler"
set "My Assembler" to disabled
turn off "My Assembler"
```

## "Power" Property
* Primitive Type: Bool
* Keywords: ```power, powered```

Turns on or off power to the block.  Effectively the same as the Enabled property.

```
#Turn on
turn on power to "My Assembler"
power on "My Assembler"

#Turn off
turn off power to "My Assembler"
power off "My Assembler"
```

## "Supply" Property
* Primitive Type: Bool
* Keywords: ```supply, supplying, generate, generating, assemble, assembling```
* Inverse Keywords: ```consume, consuming, disassemble, disassembling```

Not to be confused with Create/Destroy properties, this property sets whether the assembler is in Assembling or Dissassembling mode.
To actually create or destroy items, use the Create & Destroy properties.

```
set the "Base Assembler" to assembling
tell the "Base Assembler" to assemble
```

## "Complete" Property
* Primitive Type: Bool
* Keywords: ```done, ready, complete, finish, finished, built```

The Complete property can get, or clear, the current item queue from the assembler.  Returns false if there are any items in the queue.  Clears the queue if updating with a false boolean value.
Even though there are no inverse complete keywords, you can clear the queue easily using "stop", "false" since the default boolean property is Complete.

```
#Check if the assembler is finished
if the "Base Assembler" is finished
  Print "Finished building stuff!"

#All of the below will clear the assembler queue
stop the "Base Assembler"
tell the "Base Assembler" not to finish
tell the "Base Assembler" to stop
"Base Assembler" stop

#Does nothing
tell the "Base Assembler" to finish
```

## "Auto" Property
* Primitive Type: Bool
* Keywords: ```auto, cooperate, cooperating, drain, draining```

This property gets or sets whether the assembler is in cooperate mode or not.  Cooperating assemblers will take items from a non-cooperating assembler and build them automatically.

```
tell "My Assembler" to cooperate

if "My Assembler" is cooperating
  Print "They tell me what to build, and I build it."
```

Here's a script to set the first assembler as the main assembler and the rest as cooperative.

```
set myAssemblers to "My Assemblers"
tell $myAssemblers[0] not to cooperate
if count of $myAssemblers > 1
  tell $myAssemblers[1..count of $myAssemblers - 1] to cooperate
```

## "Create" Property
* Requires a String Attribute indicating the [Item(s)](https://spaceengineers.merlinofmines.com/EasyCommands/items "Items & Blueprints") to produce.
* Primitive Type: Bool (read) / Numeric (update)
* Keywords: ```create, creating, produce, producing, build, building, make, making```

This property enables you to automatically queue up items to be built on the given assembler(s).  This property requires a String attribute representing the items(s) to create.
If the items to create represent more than 1 resolved blueprint, the assembler will produce the indicated amount of each item.

If the given selector has more than 1 assembler, the requested items are sent to *each* assembler, not distributed across them. 

When retrieving the property value, the result is a boolean representing whether the assembler is creating the given item(s).  If any of the requested items are being built, it will return true.

So if you are building steel plate and interior plate and ask for "components" it'll tell you the total amount of steel plate + interior plate, not separated out individually.

```
#Check if the assembler is creating steel plate
if "My Assembler" is creating "steel plate"
  Print "Still creating steel plate"
```

When you use this property to create an item, it automatically switches the Assembler mode to "Assembly".  Also, when creating items, you can specify the amount that you would like to create.  

```
#Create 1 steel plate
tell "My Assembler" to create "steel plate"

#Create 10 steel plate
tell "My Assembler" to create 10 "steel plate"

#Create 1 steel plate and 1 interior plate
tell "My Assembler" to create "steel plate, interior plate"

#Create 10 steel plate and 10 interior plate
tell "My Assembler" to create 10 "steel plate, interior plate"
```

## "Destroy" Property
* Requires a String Attribute indicating the [Item(s)](https://spaceengineers.merlinofmines.com/EasyCommands/items "Items & Blueprints") to destroy.
* Primitive Type: Bool (read) / Numeric (update)
* Keywords: ```destroy, destroying, consume, consuming, recycle, recycling```

This property enables you to automatically queue up items to be destroyed on the given assembler(s).  This property requires a String attribute representing the items(s) to destroy.
If the items to destroy represent more than 1 resolved blueprint, the assembler will destroy the indicated amount of each item.

If the given selector has more than 1 assembler, the requested items are sent to *each* assembler, not distributed across them. 

When retrieving the property value, the result is a boolean representing whether the assembler is destroying the given item(s).  If any of the requested items are being destroyed, it will return true.

So if you are destroying steel plate and interior plate and ask for "components" it'll tell you the total amount of steel plate + interior plate being destroyed, not separated out individually.

```
#Check if the assembler is destroying steel plate
if "My Assembler" is destroying "steel plate"
  Print "Still destroying steel plate"
```

When you use this property to destroy an item, it automatically switches the Assembler mode to "Disassembly".  Also, when destroying items, you can specify the amount that you would like to destroy.  

```
#Destroy 1 steel plate
tell "My Assembler" to destroy "steel plate"

#Destroy 10 steel plate
tell "My Assembler" to destroy 10 "steel plate"

#Destroy 1 steel plate and 1 interior plate
tell "My Assembler" to destroy "steel plate, interior plate"

#Destroy 10 steel plate and 10 interior plate
tell "My Assembler" to destroy 10 "steel plate, interior plate"
```

## "Amount" Property
* Requires a String Attribute indicating the [Item(s)](https://spaceengineers.merlinofmines.com/EasyCommands/items "Items & Blueprints") to get amounts for.
* Primitive Type: Numeric
* Keywords: ```amount, amounts```

This property is similar to create/destroy, but the read value is a numeric.  This allows you to get the amount of item(s) being produced.

Note that this property *does not* look for the amount of items in the Assembler's Inventory.  It is the amount of items in the Assembler's Production Queue.  To get the amount of item's in the Assembler's inventory, you will need to use the [Inventory Handler](https://spaceengineers.merlinofmines.com/EasyCommands/inventory "Inventory Item Handler")

Also, this property does not automatically switch the Assembler mode.  

```
#Check if the assembler is creating/destroying at least 10 steel plate
if "My Assembler" "steel plate" amount > 10
  Print "Still creating steel plate"

#Create/Destroy 1 steel plate
set "My Assembler" "steel plate" amount to 1

#Create/Destroy 10 steel plate
set "My Assembler" "steel plate" amount to 10

#Create/Destroy 1 steel plate and 1 interior plate
set "My Assembler" "steel plate, interior plate" amount to 10

#Create/Destroy 10 steel plate and 10 interior plate
set "My Assembler" "steel plate, interior plate" amount to 10
```

## Examples

Here's a script to automatically recycle basic tools.  Combine this with a sorter for weapons/tools set to drain and you've automated recycling!

```
#Setup
set recycleAssembler to "Recycling Assembler"

async doRecycle "steel plate"

:doRecycle itemName
#Dynamic variable for the amount left to recycle
bind recycleAmount to the $recycleAssembler inventory itemName amount

#Recycle the amount iteratively until we're done
until recycleAmount is 0
  tell $recycleAssembler to recycle recycleAmount itemName
  until recycleAmount is 0 or $recycleAssembler itemName amount is 0
    set my display to "Amount to Recycle: " + recycleAmount

#Once we're done, update the display and stop the Assembler
set my display to "Nothing to Recycle"
stop $recycleAssembler
replay
```