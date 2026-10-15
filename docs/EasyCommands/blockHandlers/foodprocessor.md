# Food Processor

Keywords: `foodprocessor`, `foodprocessors`.

Extends the [Assembler handler](https://spaceengineers.merlinofmines.com/EasyCommands/blockHandlers/assembler), narrowing selection to the Food Processor definition. All assembler properties, queue behavior, modes, and defaults are inherited. Broad `assembler` selectors still include Food Processors and Survival Kits.

```
tell "Kitchen" foodprocessor to create "kelp crisp"
print "Kitchen" foodprocessor is running
print "Kitchen" foodprocessor progress
```

`running` reads `IsProducing`; `progress`/`ratio` reads `CurrentProgress`. Both are read-only and are also available on ordinary assemblers. A queued recipe waiting for ingredients is not necessarily running. Progress is for the current job, not the entire queue. `producing` retains its existing recipe-qualified CREATE meaning.

Food names map to verified blueprint IDs; see [Items](https://spaceengineers.merlinofmines.com/EasyCommands/items). Producers still accept only their supported recipes. Existing enqueue/error behavior is preserved; this change does not introduce broad recipe prevalidation or automatic ingredient management. Survival Kits can be addressed as assemblers and have a smaller recipe set.
