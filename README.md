# Drawables Generator
Create multiplayer-compatible drawables for Starbound.

## Table of Contents
- [Wiki](#wiki)
- [Generation methods](#generation-methods)
 - [Plain Text](#plain-text)
 - [Spawn Command](#spawn-command)
 - [StarCheat Export](#starcheat-export)
 - [Single Texture Directives](#single-texture-directives)
- [Planned](#planned)
- [Potential issues](#potential-issues)
- [Contributing](#contributing)

![Application](https://raw.githubusercontent.com/Silverfeelin/Drawables-Generator/master/readme/application.png "Application")  
*Overview of the application*

## Wiki

The Wiki covers about everything you need to know and do to set up and use the Drawables Generator.
https://github.com/Silverfeelin/Starbound-DrawablesGenerator/wiki

### Quick Reference

* [Installation](https://github.com/Silverfeelin/Starbound-DrawablesGenerator/wiki/Installation)
* [Usage](https://github.com/Silverfeelin/Starbound-DrawablesGenerator/wiki/Usage)

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

![Output](https://raw.githubusercontent.com/Silverfeelin/Drawables-Generator/master/readme/output.png "Output")  
*Sample output for the Spawn Command generation method*

## Planned
Nothing yet, feel free to suggest additions or changes!

## Potential issues
Of course, we can't ignore the amount of data requires to form these images.  
The main two things to worry about are performance losses and game updates 'patching' this feature (it is an exploit, after all).

## Contributing
I love suggestions! If you can think of anything to improve this application feel free to leave a suggestion on the discussion thread over at PlayStarbound (link pending).  
If you're really dedicated, you can also create a pull request and directly contribute to the mod!
