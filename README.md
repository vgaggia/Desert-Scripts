# SunCycle for VRChat

![VRChat SDK3 Compatible](https://img.shields.io/badge/VRChat%20SDK3-Compatible-brightgreen)
![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)

SunCycle is a dynamic day/night cycle system for VRChat worlds, built with UdonSharp. It allows players to interact with a designated lever to control the time of day, as well as being a decent time cycle script for a directional light.

![SunCycle Demo](Example.gif)

## Features

- 🌞 Smooth day/night cycle with dynamic lighting
- 🕹️ Interactive lever for manual time control
- 🔄 Bi-directional time flow (forward and backward)
- 🎛️ Customizable cycle length and sun intensity
- 🔊 Audio feedback for lever interaction
- 🔄 Network-synced across all players
- 📱 Compatible with both VR and desktop users

## Installation

1. Ensure you have [VRChat SDK3](https://vrchat.com/home/download) and [UdonSharp](https://github.com/vrchat-community/UdonSharp) installed in your Unity project.
2. Download the latest release from the [Releases](https://github.com/yourusername/SunCycle/releases) page.
3. Import the package into your Unity project.

## Usage

1. Drag the `SunLever` prefab into your scene.
2. Assign your directional light to the `Sun Light` field in the inspector.
3. Customize the `Cycle Length` and `Sun Intensity` as desired.
4. (Optional) Assign a UI Slider to the `Time Slider` field for visual time representation.
5. Build and test your world!

## Customization

You can easily customize the SunCycle behavior:

- **Cycle Length**: Adjust the duration of a full day/night cycle in seconds.
- **Sun Intensity**: Set the maximum intensity of the sun light.
- **Rotation Speed**: Control how quickly the lever rotates when interacted with.
- **Toggle Objects**: Assign GameObjects to be toggled on/off when the lever is activated.

## Included Assets

- `SunCycleLever` prefab: A ready-to-use lever model for easy integration.
- `LeverSound.wav`: Default audio file for lever interaction feedback.

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Acknowledgments

- Thanks to VRChat and UdonSharp for their invaluable resources and support.

## Support

If you encounter any issues or have questions, please [open an issue](https://github.com/yourusername/SunCycle/issues) on this repository.

---

Made with ❤️ for the VRChat community
