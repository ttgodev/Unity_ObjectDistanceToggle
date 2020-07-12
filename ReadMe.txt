Project Requirements

- Unity 2019.1 or higher
- Configure the following project settings to use this asset:
 • Open the Player Settings (Edit -> Project Settings -> Player), set API Compatibility Level .Net 4.x
 • Open the Package Manager (Window -> Package Manager), install Burst 1.2.3
 • To use the burst compiler in standalone builds, you need to install the Windows SDK and VC++ toolkit from the Visual Studio Installer.

Note: the demo scene example uses the builtin render pipeline with Rendering Path set to Deferred.


Usage

1. Import the ObjectDistanceToggle.unitypackage into your project.
2. Add the ObjectDistanceToggleManager prefab to a scene.
3. Assign the 'Origin Trafnsform' in the inspector field that will be used for distance check; this is most likely your camera or player object.
4. Add the ObjectDistanceToggle script to any objects that should be toggled on/off based on distance.


Tutorial

https://youtu.be/wCxP5lUk6L8