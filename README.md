# BodylogicalAR_iOS

BodylogicalAR_iOS is the repository for the BodylogicalAR derivative on iOS device using ARkit. It is compatible with iPhones and iPads that support AR. This readme provides a simple guide on how to deploy this project onto your target iOS device.

## Getting Started

Download the package, and open it in Unity (built on 2019.1.5f1). Find "Scenes" in the Asset folder and click "MainScene". This would be our starting game scene. Click "Play", you should see the welcome menu with language options. If there is no compile error, then it should be fine.

## Switch Platform

Click "File"->"Build Settings", check "Scene/MainScene", and switch platform to "iOS" if not already. The profile setting should be as following:
* Run in Xcode: Latest version
* Run in Xcode as: Release
* Check Development Build
* Leave others unchecked.

Now click "Player Settings", click "Other Settings" and make sure the profile matches the following:
* Scripting Runtime Version: .NET 4.0 Equivalent
* Scripting Backend: IL2CPP
* Automatically Sign: Checked.

* Accelerometer Frequency: 60 Hz
* Camera Usage Description: 'AR BABY'

* Target Device: iPhone + iPad
* Target SDK: 12.0
* Requires ARKit Support: Checked


## Deployment

Make sure the Xcode is the latest version, and then in Unity, select "Build and Run" (either in "Build Settings" or in "File") to build into a designated folder. The Xcode window should automatically pop out after Unity builds the project.

Now plug in the iPhone or iPad, and in Xcode, in TARGETS -> Unity-iPhone, find General tab and change the Bundle Identifier to a different string, and then check "Automatically manage signing" and select your team and provisioning file. Make sure the Device Orientation has "Landscape Left" and "Landscape right" checked, also check "Hide status bar" and "Requires full screen". If no error shows up, you can select your target device and click the Play button to build to the device.

## Built With

* [ARKit SDK](https://developer.apple.com/arkit/) - Apple's AR SDK
* [Unity](https://unity3d.com/) - Development Engine & Environment

## Runing the Game

If the build is successful, the game should start on the your target device. Simple instructions are available at the top and bottom parts of the screen. More detailed tutorials are also available at certain stages.

## Authors

* **Wanze (Russell) Xie** - (https://github.com/russellxie7)
* **Yue Wu** - (https://github.com/ALMSIVI)

## Acknowledgments

This project is a new iteration of the previous project [BodylogicalAR] (https://github.com/RussellXie7/BodylogicalAR) for HoloLens by Wanze and Yining. Great thanks to the help and contribution from Jurgen, Janet, Andrea, Colleen, Paul, Marcelo and people who gave productive feedback for the BodylogicalAR project.


