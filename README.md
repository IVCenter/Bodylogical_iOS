# Bodylogical iOS

Bodylogical iOS is the repository for the Bodylogical AR application for iOS devices. It is compatible with iPhones and iPads that support AR. Below is a simple guide on deploying this project onto your target iOS device.

## Getting Started

Download the package, and open it in Unity (built on 2019.3.3f1). Find "Scenes" in the Asset folder and click "MainScene". Click "Play", you should see the welcome menu with language options. 

You should be able to interact with the app in the editor. The AR component of this app, such as plane tracking, will be removed, but all the visualization should be available. The in-editor camera is a little hard to control, but it should be enough for testing and debugging purposes.

## Switch Platform

Click "File"->"Build Settings", check "Scene/MainScene", and switch platform to "iOS". The profile setting should be as following:

* Run in Xcode: Latest version
* Run in Xcode as: Release
* Check Development Build
* Leave others unchecked.

Now click "Player Settings", click "Other Settings" and make sure the profile matches the following:

* Scripting Runtime Version: .NET 4.0 Equivalent

* Scripting Backend: IL2CPP

* Automatically Sign: Checked.

* Accelerometer Frequency: 60 Hz

* Camera Usage Description: "Bodylogical AR" <-- You can change this to a clearer description.

* Target Device: iPhone + iPad

* Target SDK: 12.0

* Requires ARKit Support: Checked

## Deployment

Make sure the Xcode is the latest version, and then in Unity, select "Build and Run" (either in "Build Settings" or in "File") to build into a designated folder. The Xcode window should automatically pop out after Unity builds the project.

Now plug in the iPhone or iPad, and in Xcode, in TARGETS -> Unity-iPhone. Check "Automatically manage signing" and select your team and provisioning file. Make sure the Device Orientation has "Landscape Left" and "Landscape right" checked, also check "Hide status bar" and "Requires full screen". If no error shows up, you can select your target device and click the Play button to build to the device.

## Built With

* [Unity](https://unity3d.com/) - Development Engine & Environment
* [ARFoundation](https://docs.unity3d.com/Packages/com.unity.xr.arfoundation@3.1/manual/index.html) - Unity's AR library that supports multiple systems including iOS (ARKit) and Android (ARCore).

## Runing the App

If the build is successful, the application should start on the your target device. Simple instructions are available at the top and bottom parts of the screen. More detailed tutorials are also available at certain stages.

## Authors

* **Wanze (Russell) Xie** - (https://github.com/russellxie7)
* **Yue Wu** - (https://github.com/ALMSIVI)
* **Helen Cheng**
* **Jiaming Li**
* **Sophia Boss**

## Acknowledgments

This project is a new iteration of the previous project [BodylogicalAR] (https://github.com/RussellXie7/BodylogicalAR) for HoloLens by Wanze and Yining. Great thanks to the help and contribution from Jurgen, Janet, Andrea, Colleen, Paul, Marcelo and people who gave productive feedback for the BodylogicalAR project.
