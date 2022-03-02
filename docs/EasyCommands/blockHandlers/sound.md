# Sound Block Handler
This Block Handler handles Sound Blocks.  With it you can play/stop, change the song, change the loop period, the volume, and the range.

* Block Type Keywords: ```siren, alarm, speaker```
* Block Type Group Keywords: ```sirens, alarms, speakers```

Default Primitive Properties:
* Bool - Enabled
* Numeric - Volume
* String - Media

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

## "Power" Property
* Primitive Type: Bool/String.
* Keywords: ```power, powered```

Same as Enabled. Gets/Sets whether the speaker is currently enabled.
```
if "My Speaker" is powered
  Print "Speaker is enabled"

#Enable speaker
power on "My Speaker"

#Disable speaker
power off "My Speaker"
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

## "Radius" Property
* Primitive Type: Numeric
* Keywords: ```radius, radii```

Gets/Sets the radius the speaker plays sound within, in meters.  

```
Print "Speaker Range: " + "My Speaker" radius

#50 meters
set "My Speaker" radius to 50
```

## "Range" Property
* Primitive Type: Numeric
* Keywords: ```range, ranges, distance, distances, limit, limits```

Same as Radius. Gets/Sets the range the speaker plays sound within, in meters.  

```
Print "Speaker Range: " + "My Speaker" range

#50 meters
set "My Speaker" range to 50
```

## "Period" Property
* Primitive Type: Numeric
* Keywords: ```period, periods, length, lengths```

Gets/Sets the loop period for the speaker, in seconds.  Effectively this is how long the speaker will play before stopping, unless you stop it manually
```
Print "Loop Period: " + "My Speaker" period

#Play for 1 minute
set "My Speaker" period to 60
```

## "Sound" Property
* Primitive Type: Bool / String
* Keywords: ```music, sound, song, track, play, playing```
* Inverse Keywords: ```silent, silence, quiet```

For Booleans, Gets/Sets whether the speaker is currently playing. Retrieving will return true if and only if the following conditions are met:
* The currently selected sound is loopable
* The currently selected sound is playing and the loop period is not over.

When setting, will either play or stop the music based on the boolean value.

For Strings, Gets/Sets the currently selected sound, regardless of whether it is currently playing or not.  Setting the sound will *not* immediately cause the speaker to start playing.  You'll need to ask it to "play" (using Bool) after setting the sound.

When retrieving this property without indicating Bool or String, the currently selected song is returned.

```
#Check if playing
if "My Speaker" is playing
  Print "Rock on"

#Play currently selected sound.
play "My Speaker"

#Stop playing
tell "My Speaker" to stop playing

#Retrieve current sound
Print "Current Sound: " + "My Speaker" sound

#Check if currently selected song is the specified one, regardless if it is currently playing or not.
if "My Speaker" is playing "My Music"
  print "I love my playlist.."

#Set Speaker sound.  Does not cause speaker to play
set "My Speaker" sound to "SoundBlockLightsOn"
```

## "Sounds" Property
* Read-only
* Primitive Type: List
* Keywords: ```sounds, songs```

Returns a list of currently available sounds which you can set the speaker to play using the Sound property.

```
Print "Available Sounds: " + "My Speaker" sounds
```

Here's a fun little script to sample the available music.  You can imagine extending this to create your own custom playlist to provide background music.

```
for each mySong in "Test Speaker" songs
  set the "Test Speaker" song to mySong
  play the "Test Speaker"
  set i to 0
  until i > 120
    print "Current Song: " + mySong
    i++
```