# Algae Farm

Keywords: `algaefarm`, `algae farm` (single); `algaefarms`, `algae farms` (group).

Supports standard and Half Algae Farms, with functional/terminal controls and these read-only values:

| Property words | Type | Meaning |
| --- | --- | --- |
| `output`, `volume`, `rate`, `speed` | Numeric | Items per minute reported by the game |
| `countdown` | Numeric | Seconds until the next production batch; `-1` when the game cannot supply a finite estimate |

The numeric default is output. These values are not inventory stock or guaranteed delivered items. Use the existing inventory selector to count/transfer algae:

```
print "Algae Farm" algaefarm output
print "Algae Farm" inventory amount "algae"
```

Assignments to read-only properties have no effect.

A countdown of `-1` means no finite estimate is available, for example when production is blocked. Check `output` separately; the countdown is not a guaranteed delivery time.
