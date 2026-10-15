# Algae Farm

Keywords: `algaefarm`, `algaefarms`. Includes Algae Farm and Half Algae Farm.

Inherits functional/terminal controls. The solar-food component supplies these read-only values:

| Property words | Type | Meaning |
| --- | --- | --- |
| `output`, `volume`, `rate`, `speed` | Numeric | Items per minute reported by the game |
| `countdown` | Numeric | Seconds until the next production batch |

The numeric default is output. These values are not inventory stock or guaranteed delivered items. Use the existing inventory selector to count/transfer algae:

```
print "Algae Farm" algaefarm output
print "Algae Farm" inventory amount "algae"
```

Missing solar-food components produce an error. Read-only writes have the existing no-op behavior. See [Apex support](https://spaceengineers.merlinofmines.com/EasyCommands/apex-survival) for validation limitations.
