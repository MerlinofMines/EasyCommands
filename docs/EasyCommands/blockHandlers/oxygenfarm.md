# Oxygen Farm

Keywords: `oxygenfarm`, `oxygenfarms`.

Selects blocks implementing the Ingame oxygen-farm interface, including Half Oxygen Farm. Inherits functional/terminal properties.

`ready` / `able` is a read-only Boolean indicating `CanProduce`.

```
turn on "Oxygen Farm" oxygenfarm
print "Oxygen Farm" oxygenfarm ready
```

No output ratio or flow-rate alias is introduced yet: `GetOutput()` units still need verification in a running game. This handler does not treat oxygen farms as electrical power producers.
