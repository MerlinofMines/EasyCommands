# Display Block Handler
This handler is used to control diplays on any block containing one or more displays such as LCDs, Cockpits, Progammable Blocks, etc.  You can write text to the screen, change the colors, change the script/images, and more.

Note that this block handler does not extend from Terminal Block, so this Block Handler *does not* have properties defined in [Terminal Block Handler](https://spaceengineers.merlinofmines.com/EasyCommands/blockHandlers/terminal "Terminal Block Handler").

* Block Type Keywords: ```display, displays, screen, screens, lcd, lcds```

Default Primitive Properties:
* Boolean - Enable
* Numeric - Level
* String - Text
* List - Image List

Default Directional Properties
* Up - Text

Note that there are no Display Block Group keywords.  This is because individual blocks often have more than 1 display (Cockpits, Programmable Blocks).  If you want to get all displays from a group of blocks, use ```"My Displays" group displays```

## "Name" Property
* Read-only
* Primitive Type: Bool
* Keywords: ```name, names, label, labels```

Prints the Display Name for the given display.

This property also enables you to get back all Display Names for a given block by doing:

```
Print "Display names: " + "My Programmable Block" display names
```

You can update a given block's displays by named index using the display name, as follows:

```
#Keyboard
Print "My Program" display [1] name

set "My Program" display ["Keyboard"] text to "My Keyboard Text"
```

## "Enabled" Property
* Primitive Type: Bool
* Keywords: ```enable, enabled```
* Inverse Keywords: ```disable, disabled```

Enables or Disables the given Display by setting the display's content type to None (disabled) or TEXT_AND_IMAGES (enabled).

```
#Enable Block
enable "My Display"
set "My Display" to enabled
turn on "My Display"

#Disable Block
disable "My Display"
set "My Display" to disabled
turn off "My Display"
```

## "Text" Property
* Primitive Type: String
* Keywords: ```text, texts, message, messages```

Gets or Sets text on the display.  When setting the display text, this property will automatically set the content type of the display to TEXT_AND_IMAGES.

```
Print "Display Text: " + "My Display" text

set "My Display" text to "Hello World!"
set "My Display" to "Hello World!"
```

## "Image" Property
* Primitive Type: String
* Keywords: ```image```

Gets/Sets the currently shown image on the display, if it exists.  If there is no image currently being displayed, returns "".

When setting the display image, this property will automatically set the content type of the display to TEXT_AND_IMAGES.

```
Print "Image: " + "My Display" image

set "My Display" image to "Construction"
```

See below for available images.

## "Images" Property
* Primitive Type: List
* Keywords: ```images```

Gets/Sets the set of images that are being displayed on the screen.  Returns an empty list if there ar none.

When setting this property, each item in the list is expected to be a string representing one of the supported images.  See the Image property for supported images.

When setting the display images, this property will automatically set the content type of the display to TEXT_AND_IMAGES.

```
Print "Selected Images: " + "My Display" images

set "My Display" images to ["Danger", "No Entry"]
```

### Available Images:
* "Offline"
* "Online"
* "Arrow"
* "Cross"
* "Danger"
* "No Entry"
* "Construction"
* "White screen"
* "Grid"
* "Offline_wide"
* "Online_wide"
* "StoreBlock2"
* "LCD_Economy_Charts"
* "LCD_Economy_SC_Here"
* "LCD_Economy_Coins"
* "LCD_Economy_SingleCoin"
* "LCD_Economy_SC_Logo"
* "LCD_Economy_SC_Blueprint"
* "LCD_Economy_SC_Logo_2"
* "LCD_Economy_Faction_1"
* "LCD_Economy_Poster_1"
* "LCD_Economy_Trade"
* "LCD_Economy_Clear"
* "LCD_Economy_Graph"
* "LCD_Economy_Graph_2"
* "LCD_Economy_Graph_3"
* "LCD_Economy_Graph_4"
* "LCD_Economy_Graph_5"
* "LCD_Economy_SE_Logo_1"
* "LCD_Economy_SE_Logo_2"
* "LCD_Economy_Blueprint_2"
* "LCD_Economy_Blueprint_3"
* "LCD_Economy_Trinity"
* "LCD_Economy_KeenSWH"
* "LCD_Economy_Badge"
* "LCD_Frozen_Poster01"
* "LCD_Frozen_Poster02"
* "LCD_Frozen_Poster03"
* "LCD_Frozen_Poster04"
* "LCD_Frozen_Poster05"
* "LCD_Frozen_Poster06"
* "LCD_Frozen_Poster07"
* "LCD_HI_Poster1_Square"
* "LCD_HI_Poster1_Landscape"
* "LCD_HI_Poster1_Portrait"
* "LCD_HI_Poster2_Square"
* "LCD_HI_Poster2_Landscape"
* "LCD_HI_Poster2_Portrait"
* "LCD_HI_Poster3_Square"
* "LCD_HI_Poster3_Landscape"
* "LCD_HI_Poster3_Portrait"
* "LCD_SoF_CosmicTeam_Square"
* "LCD_SoF_CosmicTeam_Landscape"
* "LCD_SoF_CosmicTeam_Portrait"
* "LCD_SoF_Exploration_Square"
* "LCD_SoF_Exploration_Landscape"
* "LCD_SoF_Exploration_Portrait"
* "LCD_SoF_ThunderFleet_Square"
* "LCD_SoF_ThunderFleet_Landscape"
* "LCD_SoF_ThunderFleet_Portrait"
* "LCD_SoF_BrightFuture_Square"
* "LCD_SoF_BrightFuture_Landscape"
* "LCD_SoF_BrightFuture_Portrait"
* "LCD_SoF_SpaceTravel_Square"
* "LCD_SoF_SpaceTravel_Landscape"
* "LCD_SoF_SpaceTravel_Portrait"

## "Script" Property
* Primitive Type: String
* Keywords: ```script, run, running```

Gets/Sets the currently running script, if it exists.  Returns "" otherwise.  

When setting the display script, this property will automatically set the content type of the display to SCRIPT.

```
set "My Display" script to "TSS_ArtificialHorizon"
```

### Available Scripts:
* TSS_ClockAnalog
* TSS_ArtificialHorizon
* TSS_ClockDigital
* TSS_EnergyHydrogen
* TSS_FactionIcon
* TSS_Gravity
* TSS_Velocity
* TSS_VendingMachine
* TSS_Weather
* TSS_Jukebox

## "Padding" Property
* Primitive Type: Numeric
* Keywords: ```padding, paddings, offset, offsets```

Gets/Sets the Text Padding for the given display.

```
Print "Padding: " + "My Display" padding

set "My Display" padding to 3
```

## "Ratio" Property
* Primitive Type: Bool
* Keywords: ```ratio, ratios```

Gets/Sets whether to preserve aspect ratio for images on this display block

```
if "My Displays" ratio is true
  Print "Preserving Aspect Ratio"

set "My Display" ratio to true
```

## "Alignment" Property
* Primitive Type: String

Get/Sets the text alignment for the given display block.

When setting, if an unknown alignment is used, the alignment is set as "left". Note that the type is String, please wrap the values in quotes so they aren't incorrectly parsed.

```
set "My Display" alignement to "left"
set "My Display" alignment to "center"
set "My Display" alignment to "right"
```

### Supported Values
* "left"
* "center"
* "right"

## "Size" Property
* Primitive Type: Numeric
* Keywords ```size, height```

Gets/Sets the FontSize of the given display.

```
Print "Font Size: " + "My Display" size

set "My Display" size to 4
```

## "Color" Property
* Primitive Type: Color
* Keywords: ```color, foreground```

If the display is in Script mode, Gets/Sets the Script Foreground color.  Otherwise, Gets/Sets the Text Font Color.

```
set "My Display" color to green
```

## "Background" Property
* Primitive Type: Color
* Keywords: ```background```

If the display is in Script mode, Gets/Sets the Script Background color.  Otherwise, Gets/Sets the Text Background Color.

```
set "My Display" background to red
```

## "Font" Property
* Primitive Type: String/Numeric/Color
* Keywords: ```font```

When Getting this property, it returns the Font ("Debug", etc) for the given display.

When setting this property, if the value is a String, will update the Font.  If the value is numeric, will update the font size.  If the value is a color, will update the font color.
 
```
#Font to "Debug"
set "My Display" font to "Debug"

#Font Size to 2
set "My Display" font to 2

#Font Color to red
set "My Display" font to red
```

### Supported Fonts
* "Debug"
* "Red"
* "Green"
* "Blue"
* "White"
* "DarkBlue"
* "UrlNormal"
* "UrlHighlight"
* "ErrorMessageBoxCaption"
* "ErrorMessageBoxText"
* "InfoMessageBoxCaption"
* "InfoMessageBoxText"
* "ScreenCaption"
* "GameCredits"
* "LoadingScreen"
* "BuildInfo"
* "BuildInfoHighlight"
* "Monospace"
* "BI_Green"
* "BI_SkyBlue"
* "BI_Yellow"
* "BI_Gray"