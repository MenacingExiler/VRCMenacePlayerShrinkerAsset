# VRCMenacePlayerShrinkerAsset
 
Advanced Player Shrinking asset for VRChat World Creations.

This asset is made with Vket booth compatibility in mind. Coding practice are made to comply and to be compatible with VRChat ToS and Vket ToS.

The asset includes example scene with a shrink volume.

## Features

Shrinks or increases player avatar size when enter a trigger volume.
The script takes avatar changes into mind. If player changes avatar, their avatar will be immediately shrinked to determined rate adn will restore to size it was intended to.

### Shrink time
A feature that would gradually shrink player to desired height in seconds. This feature have a shrink rate of every 0.1 seconds.

Example: Setting 5 seconds with 0.5 fixed height will take 5 seconds for player to shrink form current height to 50cm, gradual shrink happening every 0.1 seconds.

### Fixed mode.
A classic shrinking mode that simply shrink player to a certain height in metre.

Example: If you set it to 0.5, Player will shrink from current height to 50cm.

### Height mode.
Scale player height based on percentage. Additionally, you can also set minimum height and maximum height player is allowed to srhink to and will cap when player height result crossing the limit.

Example: If you want player height to decrease by 50% but not lower than 30cm or higher than 70cm.

## How to use?

You can either use premade shrink cube or apply script to any object with collider and trigger enabled.

To turn any object into trigger like a cube, drag the shrink script to an object. Remmeber to have collider in the object. Any collider can do as long it has Trigger enabled.



This asset is licensed under Apache License 2.0. Feel free to contribute or include in your project as long as you follow the license.
