# Menace's Shrinker Asset
 
Advanced Player Shrinking asset for VRChat World Creations.

This asset is made with Vket booth compatibility in mind. Coding practice are made to comply and to be compatible with VRChat ToS and Vket ToS. This means you can compile to both Vket booth version and non-Vket without breaking compatibility.

The asset includes example scene with a shrink volume.

![2024-09-23 12-02-58](https://github.com/user-attachments/assets/76201341-2dfc-4d41-92cd-48a4aecb0fd0)

Download: https://github.com/MenacingExiler/VRCMenacePlayerShrinkerAsset/releases/download/Alpha/MenacePlayerShrinkerAsset_Pre-release.0.1.0-alpha.unitypackage

## Features

Adjust player walking, jumping and physics.

![image](https://github.com/user-attachments/assets/3f22d6f8-0f82-4eed-8711-484163c3ff6a)


Shrinks or increases player avatar size when enter a trigger volume.
The script takes avatar changes into mind. If player changes avatar, their avatar will be immediately shrinked to determined rate adn will restore to size it was intended to.

### Shrink time
![ShrinkTime](https://github.com/user-attachments/assets/711348e0-e10e-4e78-bf08-ea97af4a2733)

A feature that would gradually shrink player to desired height in seconds. This feature have a shrink rate of every 0.1 seconds.

Example: Setting 5 seconds with 0.5 fixed height will take 5 seconds for player to shrink form current height to 50cm, gradual shrink happening every 0.1 seconds.

### Fixed mode toggle.
![FixedMode](https://github.com/user-attachments/assets/1f04267e-02e0-48ce-aa0c-2ac75e976647)

Toggle between Fixed mode or multiplier mode.
In fixed mode, all player will be set to specific height regardless to their origin height.
In multiplier mode, player will be scaled based on difference in percentage. This mode also supports maximum and minimum height.

### Fixed mode.
![FixedModify](https://github.com/user-attachments/assets/c7f07598-1e38-4c40-895b-33aaa66f491b)

A classic shrinking mode that simply shrink player to a certain height in metre.

Example: If you set it to 0.5, Player will shrink from current height to 50cm.

### Height mode.
![HeightMode](https://github.com/user-attachments/assets/8b4b1810-aa4c-433f-aee7-8518d2a120f7)

Scale player height based on percentage. Additionally, you can also set minimum height and maximum height player is allowed to srhink to and will cap when player height result crossing the limit.

Example: If you want player height to decrease by 50% but not lower than 0.5m or higher than 1.5m.

### Experimental

Enable Mesh: disable or enable mesh when game starts.
Shrink rate: How ofter player gets shrinked each interval in Height mode.

![image](https://github.com/user-attachments/assets/59b7cf38-2728-4a90-b1ac-d6a4096e3f68)


## How to use?

You can either use premade shrink cube or apply script to any object with collider and trigger enabled.

To turn any object into trigger like a cube, drag the shrink script to an object. Remmeber to have collider in the object. Any collider can do as long it has Trigger enabled.

# License

This asset is licensed under Apache License 2.0. Feel free to contribute or include in your project as long as you follow the license.
If you use it in your world or Vket booth, please credit me MenacingExiler with link to github.com/MenacingExiler/VRCMenacePlayerShrinkerAsset

# Support me!

If you like what I made and would like to support me, I would appreciate if you can! ko-fi.com/menacingexiler
