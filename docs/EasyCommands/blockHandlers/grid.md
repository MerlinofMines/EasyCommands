# Grid Block Handler
This Block Handler handles Grids, giving you access to a few of the properties on the grid which your program and blocks reside upon.  Note that there are no block group keywords for grids.

Note that this block handler does not extend from Terminal Block, so this Block Handler *does not* have properties defined in the Reference: [Terminal Block Handler](https://spaceengineers.merlinofmines.com/EasyCommands/blockHandlers/terminal "Terminal Block Handler").

* Block Type Keywords: ```grid, grids```
* Block Type Group Keywords: ```grid, grids```
* Note: All keywords for grids are ambiguous, so they work for either a block or a block group

## "Name" Property
* Primitive Type: String
* Keywords: ```name```

Gets/Sets the name of the grid for on which this block resides.  Note that if a selector is used which spans multiple grids, all grid names will be retrieved or set.

```
Print "Grid Name: " + my grid name

set my grid name to "My New Name"

Print "All Grids: " + all grid names

set the name of all grids to "My New Name"
```

## "Static" Property
* Read-only
* Primitive Type: Bool
* Keywords: ```locked, permanent, static```
* Inverse Keywords: ```unlocked```

Gets whether the current grid is static 
```
Print "Grid is Static: " + my grid is static
```

## "Size" Property
* Read-only
* Primitive Type: String
* Keywords: ```size```

Gets whether the current grid size is large or small.  If large, will return "large", otherwise returns "small"
```
Print "Grid Size: " + my grid size
```