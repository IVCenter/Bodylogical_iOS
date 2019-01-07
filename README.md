# BodylogicalAR_iOS

BodylogicalAR_iOS is the repository for the BodylogicalAR derivative on iOS device using ARkit, it is compatible with iPhone and iPad that support the AR feature. This readme provides a simple guide of how to deploy this project onto your target iOS device.

## Getting Started

Download the package, and open it in Unity (tested in 2018.2.16 and 2018.2.19). Find "MainScene" in the Asset folder and click "MainTest". This would be our starting game scene. Click "Play", you should see a green screen with "Please find a flat surface". If no compile error, then it should be fine.

## Switch Platform

Click Build Settings, check the "MainScene/MainTest",and switch platform to "iOS" if not already. The profile setting should be as following:
* Run in Xcode: Latest version
* Run in Xcode as: Release
* Check Development Build
* Leave others unchecked.

Now click "Player Settings", click "Other Settings" and make sure the profile matches the following:
* Scripting Runtime Version: .NET 3.5 Equivalent
* Scripting Backend: IL2CPP
* Automatically Sign: Checked.

* Accelerometer Frequency: 60 Hz
* Camera Usage Description: 'AR BABY'

* Target Device: iPhone + iPad
* Target SDK: 12.0
* Requires ARKit Support: Checked


## Deployment

Make sure the Xcode is the latest version, and then in Unity, select "Build and Run" to build into a designated folder. The Xcode window should automatically pops out after finished.

Now plug in the iPhone or iPad, and in Xcode, in TARGETS -> Unity-iPhone, find General tab and change the Bundle Identifier to a different string, and then check "Automatically manage signing" and select your team and provisioning file. Make sure the Device Orientation has "Landscape Left" and "Landscape right" checked, also check "Hide status bar" and "Requires full screen". If no error shows up, you can select your target device and click the Play button to build to the device.

## Built With

* [ARKit SDK](https://developer.apple.com/arkit/) - The SDK used
* [Unity](https://unity3d.com/) - Development Engine & Environment

## Run the Game

If build success, the game should start on the your target device. For the cross-platform compatibility concerns, the main interaction is Gaze interaction, physically move the device and use the central cursor and tap screen to make selections in the game. The tutorial in the current version should guide user smoothly through the complete experience.

## Authors

* **Wanze (Russell) Xie** - (https://github.com/russellxie7)
* **Yue Wu**

## Acknowledgments

This project is a new iteration of the previous project BodylogicalAR for HoloLens by Wanze and Yining. Great thanks to the help and contribution from Jurgen, Janet, Andrea, Colleen, Paul, Marcelo and people who gave productive feedback for the BodylogicalAR project.


