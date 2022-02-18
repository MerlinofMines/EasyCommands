# Terminal Block Handler

The most basic block handler is the Terminal Block Handler, which supports any block that is a TerminalBlock.  Almost all other BlockHandlers inherit from this and so also have these properties.  The only exceptions to this are Displays and Inventories.

* Block Type: ```terminal```
* Group Block Types: ```terminals```

Default Primitive Properties:
* Bool - enabled
* Vector - position
* String - name

### "Names" property
* Read-only
* Primitive Type: String
* Keywords: ```names```

Returns a list of names for all of the blocks in the given selector.  Useful for getting a list of all block's names in a given group so that you can print out or iterate through each one to do something.  Good use cases for this might be for re-naming a group of blocks according to some pattern.  A cool script might prefix all of the blocks on your grid with some value so that later you know that those blocks belong to that ship. Example for doing this:

```
set prefix to "[Base] "
set myTurrets to the turret names

#For each turret, if it isn't prefixed with my prefix, then add it
for each turretName in the turret names
  if turretName does not contain prefix
    set $turretName turret name to prefix + turretName
```

Careful using this, as if you have too many blocks you can easily get the Script Too Complex issue.  If this happens, try getting block names for individual block types instead.

### "Name" property
* Primitive Type: String
* Keywords: ```name```

Gets or sets the CustomName of the given Terminal Block (the name you see in the Terminal menu).  

### "Show" property
* Primitive Type: Bool
* Keywords: ```show, showing```
* Inverse Keywords: ```hide, hiding```

Gets or sets whether the given block is shown by default in the terminal menu.  Useful for hiding individual blocks after you have created groups for them.

```
if any of "My Doors" are showing
  hide "My Doors"
```
### "Enabled" property
* Primitive Type: Bool
* Keywords ```enable, enabled```
* Inverse Keywords: ```disable, disabled```

Sets whether the block is turned on or off.  This property is present for any block that you can enable or disable from the terminal block menu.

```
if "My Remote Control" is disabled
  enable "My Remote Control"
```

### "Power" property
* Primitive Type: Bool
* Keywords: ```power, powered```

Identical to "enabled" unless overridden by the specific block handler.

```
power on the "Hydrogen Thrusters"
turn on the "Exterior Lights"
```

### "CustomData" property
* Primitive Type: String
* Keywords: ```data, customdata```

Gets or Sets the blocks `CustomData` field. 

```
#Output your current program
print my data

#Delete your current program (do not actually do this).
set my data to ""
```


### "DetailedInfo" property
* Read-only
* Primitive Type: String
* Keywords: ```info, details, detailedinfo```

Returns the detailed info of the block. `DetailedInfo` contains the text on the right hand site of the terminal menu.

```
set myInfo to "Battery" details split "\n"

#Output stored power
Print myInfo[6]
```

### "Position" property
* Read-only
* Primitive Type: Vector
* Keywords: ```position, positions, location, locations```

Returns the current position of the given block, in World Coordinates.

```
#Gets the current position of the programmable block
Print "Block Position: " + my position

#Gets the current position of the remote control block
Print "Remote Control Position: " + "My Remote Control" position
```

### "Direction" property
* Read-only
* Primitive Type: Vector
* Keywords: ```direction, directions```

This directional property returns a normalized vector representing the requested "direction" with respect to the block, in World Coordinates.  In other words, if you add "my position" and "my forwards direction" together, you get a coordinate that is 1 meter in front of the block's position.

This property is really useful for creating things like target coordinates, which can then be sent to other grids.  Useful for "follow me" scripts or defining paths for other ships to follow.

Supported Directions:
* Up, Down, Left, Right, Forwards, Backwards

```
set myPosition to my position
set myForwardsDirection to my forwards direction
set myDownwardsDirection to my downwards direction

set targetLocation to myPosition + 10 * myForwardsDirection + 20 * myDownwardsDirection

Print "Target Location: " + targetLocation
```

### "Properties" Property
* Read-only
* Primitive Type: List
* Keywords: ```properties, attributes```
Returns a list of property names for the given block(s).  May contain duplicates if specified selector has more than 1 terminal block.  I recommend only using on a single block at a time. 

```
Print "My Properties: " + the "Test Piston" properties
```

### "Property" Property
* String attribute for the property name is required.
* Primitive type: Depends on the property.
* Keywords: ```property, attribute```

This property can get or set a given property for the block.  The attribute is expected to be a String representing the name of the property to be updated.  When resolving what property to get or set, the resolved value is first compared against known reserved keywords which represent other properties.  So, you can dynamically set properties using this feature:

```
#These two commands are equivalent
set the "Base Antenna" range to 2000
set the "Base Antenna" "range" property to 2000
```

If the given block type does not have a property for the given keywords, then the block attempts to look up the SpaceEngineers property for that block by name.  This enables you to get or set properties even if they do not have explicit property mappings.  See "Controlling Unsupported Block Types" for more information.

### "Actions" property
* Read-only
* Primitive type: List
* Keywords: ```actions```
Returns a list of action names for the given block(s).  May contain duplicates if specified selector has more than 1 terminal block.  I recommend only using on a single block at a time.

```
Print "My Actions: " + the "Test Piston" actions
```

### "Action" Attribute Property
* Update-only
* Primitive Type: String
* Keywords: ```action```

Applies the given action on all blocks in the given Selector.  Actions do not have return values, so this is an update-only property.

```
#All of the following will work
tell the "Gatling Turrets" to apply the "ShootOnce" action
apply the "ShootOnce" action on the "Gatling Turrets"
apply the "Gatling Turrets" "ShootOnce" action

set myAction to "ShootOnce"
tell the "Gatling Turrets" to apply myAction action
apply myAction action on the "Gatling Turrets"
apply the "Gatling Turrets" myAction action
```

### Controlling Unsupported Block Types
It's impossible to model all Block Types in Space Engineers, since you might have mods installed that add new block types that EasyCommands doesn't know about.  Luckily, since almost all blocks inherit from Terminal Block, you can control most of those block's even if they don't have an explicit handler of their own.  So, if you find there's a block type (which is a Terminal Block) without a block handler (including 3P block types from mods), you can get some control over it's properties with this Block Handler. 

Here's a script to print out properties & values for an unsupported block:

```
set unknownBlock to "Unknown Block"
#Print a set of supported properties for the given blcok
Print $unknownBlock terminal properties

#Print out current property names and values
set myProperties to $unknownBlock terminal properties

set myOutput to "Unknown Block Name: " + unknownBlock + "\n\n"

for each i in 0 .. count of myProperties[] - 1
   myOutput+= "Property: " + myProperties[i] + ", Value: " + $unknownBlock terminal myProperties[i] property + "\n\n"

print myOutput
```

And a similar script for printing out available actions for an unsupported Block:

```
set unknownBlock to "Unknown Block"
#Print a set of supported actions for the given blcok
Print $unknownBlock terminal actions

#Print out current property names and values
set myActions to $unknownBlock terminal actions

set myOutput to "Unknown Block Name: " + unknownBlock + "\n\n"

for each i in 0 .. count of myActions[] - 1
   myOutput+= "Action: " + myActions[i] + "\n\n"

print myOutput
```
