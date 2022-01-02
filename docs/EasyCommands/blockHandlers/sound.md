# Sound Block Handler
This Block Handler handles Sound Blocks.  With it you can play/stop, change the song, change the loop period, the volume, and the range.

* Block Type Keywords: ```siren, alarm, speaker```
* Block Type Group Keywords: ```sirens, alarms, speakers```

Default Primitive Properties:
* Bool - Power
* Numeric - Volume
* String - Media

Default Directional Properties
* Up - Volume 

## "Enabled" Property
* Primitive Type: Bool
* Keywords: ```enable, enabled```
* Inverse Keywords: ```disable, disabled```

Enables or Disables the given block

```
#Enable Block
enable "My Siren"
set "My Siren" to enabled

#Disable Block
disable "My Siren"
set "My Siren" to disabled
```

## "Volume" Property
* Primitive Type: Numeric
* Keywords: ```volume, volumes, output, outputs, intensity, intensities```

Gets/Sets the volume of the speaker. Values are between 0 - 1, with 1 = 100% volume.

```
Print "Speaker Volume: " + "My Siren" volume

#75% Volume
set "My Siren" volume to 0.75
```

## "Range" Property
* Primitive Type: Numeric
* Keywords: ```range, ranges, distance, distances, limit, limits, radius, radii```

Gets/Sets the range the speaker plays sound within, in meters.  

```
Print "Speaker Range: " + "My Speaker" range

#50 meters
set "My Speaker" range to 50
```

## "Length" Property
* Primitive Type: Numeric
* Keywords: ```length, lengths```

Gets/Sets the loop period for the speaker, in seconds.  Effectively this is how long the speaker will play before stopping, unless you stop it manually
```
Print "Loop Period: " + "My Speaker" length

#Play for 1 minute
set "My Speaker" length to 60
```

## "Media" Property
* Primitive Type: String
* Keywords: ```music, song```

Gets/Sets the currently selected sound, regardless of whether it is currently playing or not.

```
Print "Current Song: " + "My Speaker" song

set "My Speaker song to "SoundBlockLightsOn"
```

## "Power" Property
* Read-only
* Primitive Type:
* Keywords: ```power, powered```

Gets/Sets whether the speaker is currently playing.  Since this is the default property, it's easiest to just turn it on/off.

```
if "My Speaker" is on
  Print "Speaker is playing"

#Play sound
turn on "My Speaker"

#Turn off sound
turn off "My Speaker"
```