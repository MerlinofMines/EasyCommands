# Oxygen Farm

Keywords: `oxygenfarm`, `oxygen farm` (single); `oxygenfarms`, `oxygen farms` (group).

Supports Oxygen Farms and Half Oxygen Farms, with functional/terminal properties such as power and name.

`ready` / `able` is a read-only Boolean indicating whether the farm can produce oxygen.

```
turn on "Oxygen Farm" oxygenfarm
print "Oxygen Farm" oxygenfarm ready
```

`ratio`, `percentage`, and `progress` report a read-only output factor from 0 to 1. This is a sunlight/output fraction, not liters per minute or measured gas delivered. A non-working farm reports zero. `ready` remains a separate production-readiness flag.

```
print "Oxygen Farm" oxygen farm percentage
```
