## Auto Refill Inventories
This script will monitor your chosen inventories to make sure you have enough components of each type you specify.

If there aren't enough components, it queues them to be produced from your assemblers, and automatically moves produced items into your inventories.

Lastly, it updates the Program screen to indicate how many of each component is missing.

![Auto Refill](https://i.imgur.com/zz4K0vv.gif)

```
set global containerGroupName to "[PB] Underground Containers"
set global assemblerGroupName to "[PB] Assemblers"
set global mainAssembler to "[PB] Assembler - 1 - main"
tell the $mainAssembler assembler to supply

setupItems

async maintainCargo
async transferFromAssemblers
async flushAssemblers

:setupItems
set itemRequirements to []
set itemRequirements["construction component"] to 10000
set itemRequirements["metal grid"] to 2000
set itemRequirements["interior plate"] to 5000
set itemRequirements["steel plate"] to 10000
set itemRequirements["girder"] to 2000
set itemRequirements["small steel tube"] to 5000
set itemRequirements["large steel tube"] to 2000
set itemRequirements["motor"] to 3000
set itemRequirements['display'] to 1000
set itemRequirements["bulletproof glass"] to 5000
set itemRequirements["computer"] to 5000
set itemRequirements["radio component"] to 250
set itemRequirements["power cell"] to 1000
set itemRequirements['medical component'] to 50
#set itemRequirements['reactor component'] to 50
set itemRequirements['detector component'] to 100
set itemRequirements["solar cell"] to 200
set itemRequirements["gatling ammo"] to 200

:maintainCargo
set i to 0

set outputText to "Producing Components:\n"
set componentsNeeded to false
set itemKeys to itemRequirements[] keys
set itemCount to count of itemKeys[]
while i <= itemCount - 1
  ensurePresent itemKeys[i] itemRequirements[itemKeys[i]]
  set i to i + 1
if componentsNeeded is false
  set outputText to "Components Satisfied"
  set my display[0] color to green
else
  set my display[0] color to orange
set my display[0] to outputText
replay

:ensurePresent itemName itemAmount
set amountPresent to total itemName amount in the $containerGroupName containers
set amountProducing to total itemName amount in the $assemblerGroupName assemblers

set amountNeeded to round (itemAmount - amountPresent)
set amountToProduce to round (amountNeeded - amountProducing)

if amountNeeded > 0
  set outputText to outputText + itemName + ": " + amountNeeded + "\n"
  set componentsNeeded to true

if amountToProduce > 0
  tell the $mainAssembler assembler to create amountToProduce itemName

:transferFromAssemblers
set componentAmount to the total $assemblerGroupName group inventories "components,ammo" amount
Print "Component Amount: " + componentAmount

if componentAmount > 0
  transfer "components,ammo" from $assemblerGroupName group inventories to $containerGroupName containers
else
  Print "Transfer Complete"
replay

:flushAssemblers
set ingotAmount to the total "ingots" amount in $assemblerGroupName group inventories
Print "Ingot Amount: " + ingotAmount
if ingotAmount > 0
  transfer "ingots" from $assemblerGroupName group inventories to $containerGroupName
else
  Wait 120
replay
```