# Save My Eyes - Protect from PWM Flicker

Some screens use pulse-width modulation (PWM) flicker for low-brightness.

This tiny application ensures that at low brightness levels, your screen still uses direct current (DC) dimming to reduce flicker, potentially providing a more comfortable visual experience. 

The app functions as a substitute for the default brightness adjustment icon in the system tray.

## Features

- **PWM Threshold Control**: Set a brightness percentage for PWM to avoid that your screen switches to PWM dimming below a specific brightness level.
- **Overlay Darkness Adjustment**: Tune the darkness of the overlay that simulates lower brightness without using PWM.
- **System Tray Integration**: Access and adjust settings conveniently from the system tray icon, through a simple slider interface.

![图片](https://github.com/puff-dayo/Save-My-Eyes/assets/84665734/a84a8066-4ecb-4e8a-aaf9-4ab3d651bee7)


## Getting Started

### Release

Precompiled binary .exe files are provided.

### Compile from source

- Microsoft .NET Framework 4.8
- Microsoft Visual Studio 2022

### Usage

- Left-click the tray icon to open the brightness control slider.
- Right-click the tray icon to access the settings menu where you can set PWM threshold and overlay darkness.
- The application will remember your settings between sessions.

## License

This project is licensed under the MIT License.



---


> [!IMPORTANT]
> Remember to look away from your screen regularly!

