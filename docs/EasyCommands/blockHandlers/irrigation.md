# Irrigation System

Keywords: `irrigation`, `irrigations` (group selector).

Extends the [Gas Generator handler](https://spaceengineers.merlinofmines.com/EasyCommands/blockHandlers/generator), narrowing selection to Irrigation System. Broad `generator` selectors still include Irrigation Systems. Enabled/power, terminal properties, defaults, and refill behavior are inherited unchanged.

`running` is a new read-only Boolean on all gas generators, reading `IsProducing`.

```
turn on "Water Supply" irrigation
print "Water Supply" irrigation is running
print "Water Supply" inventory amount "ice"
```

The inherited `auto`/`refill` property is bottle refill; it is **not automatic watering**. The vanilla irrigation definition cannot refill bottles. Stored water level/capacity belongs to Farm Plots, not this handler. Conveyor controls remain available through discovered terminal properties where exposed.
