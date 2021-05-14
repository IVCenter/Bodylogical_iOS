# Bodylogical iOS

Bodylogical is a body health simulator by PwC. This AR app is meant to demonstrate its usage and how we can utilize the data to predict and prevent future health problems.

This app is designed to run on iOS devices with ARKit support. However, it uses Unity's AR Foundation as its infrastructure so it should be easy to port to other platforms, such as Android.

## Built With

- [Unity](https://unity3d.com/) - Development Engine & Environment
- [ARFoundation](https://docs.unity3d.com/Packages/com.unity.xr.arfoundation@3.1/manual/index.html) - Unity's AR library that supports multiple systems including iOS (ARKit) and Android (ARCore).

Some Asset store libraries used:

- [Wireframe shader](https://assetstore.unity.com/packages/vfx/shaders/wireframe-shader-the-amazing-wireframe-shader-18794)
- [Cross Section](https://assetstore.unity.com/packages/vfx/shaders/cross-section-66300)

## Documentation

Check out the project's wiki page: https://github.com/IVCenter/Bodylogical_iOS/wiki for an overview of different modules and implementation details.

## Deployment

### Cloning the project

Some of the 3D files are large, and are managed by Git Large File Storage (git-lfs). Before you clone the project, visit https://git-lfs.github.com to install git-lfs and properly set up. Otherwise, you will not be able to fetch the model files correctly.

### Testing in Unity

The project is built on 2019.4.17f1, but should work on newer versions as well. After you open the project, find "Scenes" in the Asset folder and click "MainScene". Click "Play", you should see the welcome menu with language options. 

You should be able to interact with the app right in the editor. The AR component of this app, such as plane tracking, are not available, but all the visualizations modules should work well. The in-editor camera is a little hard to control, but it should be enough for testing and debugging purposes.

### Switching Platform

Click "File"->"Build Settings", check "Scene/MainScene", and switch platform to "iOS". Depending on your hardware, it might take a while to switch build target. The profile setting should be as following:

* Run in Xcode: Latest version
* Run in Xcode as: Release
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

### Deploying to device

Make sure the Xcode is the latest version, and then in Unity, select "Build and Run" (either in "Build Settings" or in "File") to build into a designated folder. The Xcode window should automatically pop out after Unity builds the project.

Now plug in the iPhone or iPad, and in Xcode, in TARGETS -> Unity-iPhone. Check "Automatically manage signing" and select your team and provisioning file. Make sure the Device Orientation has "Landscape Left" and "Landscape right" checked, also check "Hide status bar" and "Requires full screen". If no error shows up, you can select your target device and click the Play button to build to the device.

### Running the App

If the build is successful, the application should start on the your iOS device. Select your language and tutorial options, and follow the tutorial to discover what the simulator can do for you!

## Authors

* **Wanze (Russell) Xie** - (https://github.com/russellxie7)
* **Yue Wu** - (https://github.com/ALMSIVI)
* **Helen Cheng**
* **Jiaming Li**
* **Sophia Boss**

## Acknowledgments

This project is a new iteration of the previous project [BodylogicalAR](https://github.com/RussellXie7/BodylogicalAR) for HoloLens by Wanze and Yining. Great thanks to the help and contribution from Jurgen, Janet, Andrea, Colleen, Paul, Marcelo and people who gave productive feedback for the BodylogicalAR project.