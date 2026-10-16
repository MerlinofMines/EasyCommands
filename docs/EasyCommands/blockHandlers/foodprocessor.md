# Food Processor

Keywords: `foodprocessor`, `food processor` (single); `foodprocessors`, `food processors` (group).

Extends the [Assembler handler](https://spaceengineers.merlinofmines.com/EasyCommands/blockHandlers/assembler), narrowing selection to the Food Processor definition. All assembler properties, queue behavior, modes, and defaults are inherited. Broad `assembler` selectors still include Food Processors and Survival Kits.

```
tell "Kitchen" foodprocessor to create "kelp crisp"
print "Kitchen" foodprocessor is running
print "Kitchen" foodprocessor progress
```

`running` indicates active production; `progress`/`ratio` gives current job completion from 0 to 1. Both are read-only and are also available on ordinary assemblers. A queued recipe waiting for ingredients is not necessarily running. Progress is for the current job, not the entire queue. Use `producing` with a recipe name to check its queued amount.

Food names map to blueprint IDs; see [Items](https://spaceengineers.merlinofmines.com/EasyCommands/items). Producers still accept only their supported recipes. Supply ingredients separately. Survival Kits can be addressed as assemblers and have a smaller recipe set.
