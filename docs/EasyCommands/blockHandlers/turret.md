# Turret Block Handler

This Block Handler handles turrets.  It extends from the [Gun Handler](https://spaceengineers.merlinofmines.com/EasyCommands/blockHandlers/gun "Gun Handler") so it has all of those properties in addition to those below.

* Block Type Keywords: ```turret```
* Block Type Group Keywords: ```turrets```

Default Primitive Properties:
* Boolean - Enabled
* Numeric - Range
* Vector - Target

Default Directional Properties
* Up - Range

## "Auto" Property
* Primitive Type: Bool
* Keywords: ```auto```

Gets/Sets whether the turret has idle movement enabled

```
if "My Turret" is on auto
  Print "Moving about aimlessly.."

#Turn off Idle Movement
"My Turret" auto off
```

## "Locked" Property
* Primitive Type: Bool
* Keywords: ```lock, locked```
* Inverse Keywords: ```unlock, unlocked```

Gets/Sets whether the turret is currently aiming at a target.  

Note that this property will return true if the turret target is set manually.

When setting this property, the turret's target will be reset if the value is false.

```
if "My Turret" is locked
  Print "Locked onto target!"

#Reset turret's target
unlock "My Turret"
```

## "Target" Property
* Primitive Type: Vector
* Keywords: ```target, targets```

Gets/Sets whether the turret's current target.  When getting, will return either the manually set target, or the position of the targeted entity.  If the turret has no target, it will return 0:0:0.

When setting this property, the turret's target will be set to the static coordinates.  So if you set this to a moving target, it *will not auto track*

This property makes use of custom data on the Turret itself (to keep track of manual target).

```
Print "Target Location: " + "My Turret" target

#Maybe not a good idea...
set "My Turret" target to my position
```

## "Target Velocity" Property
* Primitive Type: Vector
* Keywords: ```targetVelocity```

Gets/Sets the velocity of the turret's current target, in world coordinates.  If the turret has no target, it will return 0:0:0.

When setting this property, the turret will be set to track the target assuming it maintains the velocity vector you've specified.  This is intended to be used in combination with the target property for manually tracking targets.

```
Print "Target Velocity: " + "My Turret" targetVelocity

#Definitely not a good idea!
set "My Turret" target to my position
set "My Turret" targetVelocity to my ship velocity
```