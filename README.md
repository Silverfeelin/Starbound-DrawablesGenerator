# Drawables Generator
Create multiplayer-compatible drawables for Starbound.

## Table of Contents
- [Installation](#installation)
- [Usage](#usage)
 - [Regular usage](#regular-usage)
 - [Quick drag and drop](#quick-drag-and-drop)
- [Generation methods](#generation-methods)
 - [Plain Text](#plain-text)
 - [Spawn Command](#spawn-command)
 - [StarCheat Export](#starcheat-export)
 - [Single Texture Directives](#single-texture-directives)
- [Planned](#planned)
- [Potential issues](#potential-issues)
- [Contributing](#contributing)

## Installation
* If you haven't already, [download](https://www.microsoft.com/en-us/download/details.aspx?id=30653) and install .NET Framework 4.5.
* [Download](https://github.com/Silverfeelin/Drawables-Generator/releases) the release for the current version of Starbound.
* Unpack the archive. Make sure the executable and included library files are unpacked to the same location.

## Usage

##### Regular usage
* Open the application.
* Select an image. You can do so by pressing 'Select Image' or dragging an image file into the window.
 * Images may contain up to `32678` pixels (width in pixels multiplied by height in pixels). This is to keep the performance losses low.
* Position the image. You can do so by dragging on the preview or manually adjusting the X and Y coordinate.
* Select a generation method.

#### Quick drag and drop
You can drag an image directly onto the executable. This will copy a `/spawnitem` command to your clipboard.  
The default starting position (0,0) is used in this method.

## Generation Methods

##### Plain Text
Creates and position drawables for your selected image, and applies the drawables to a basic active item template.  
The output is displayed in a text field.

##### Spawn Command
Creates and positions drawables for your selected image, and applies the drawables to a basic active item template.  
The output is displayed as a valid `/spawnitem` command, which can be pasted in game to spawn the item.
This requires admininstrative permissions (`/admin`), which means you might have to enter singleplayer to use the command.

Note that pasting a long command in chat might lower the performance on your game. This performance decrease will disappear once you spawn the item.

#### StarCheat Export
Creates and positions drawables for your selected image, and applies the drawables to a basic active item template.  
The applications prompts a location to save the output, and then creates a JSON file for the item at the specified location.

#### Single Texture Directives
Creates a directives string which can be applied to a single texture (that supports directives), to form the selected image.

If the image you apply these directives to is smaller than 64x64 (in any dimension), you must manually increase the scale found near the start of the output (`?scalenearest=value`).

## Planned
Nothing yet, feel free to suggest additions or changes!

## Potential issues
Of course, we can't ignore the amount of data requires to form these images.  
The main two things to worry about are performance losses and game updates 'patching' this feature (it is an exploit, after all).

## Contributing
I love suggestions! If you can think of anything to improve this application feel free to leave a suggestion on the discussion thread over at PlayStarbound (link pending).  
If you're really dedicated, you can also create a pull request and directly contribute to the mod!
