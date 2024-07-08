# Controller Slide Presenter

A small tool to emulate a Slide Presenter using a Wiimote or a Joy-Con.

## Controls

### Wiimote

- A or Right - Next Slide
- B or Left - Previous Slide

### Joy-Con

- A or ZL/ZR - Next Slide
- B or L/R - Previous Slide

## Platform Support

### Windows

For the Joy-Con you just need to connect it via Bluetooth and then run the program.

For the Wiimote you may need to connect it using Dolphin Emulator and then just run the program.

**WARNING:** It's possible that Steam tries reading the Joy-Con in the background and keeps it from working.

### Linux

**Prerequisite:** You need [xdotool](https://github.com/jordansissel/xdotool) installed in order to redirect the input.

At the moment there's only Joy-Con support, so connect it via Bluetooth and run the program.

### MacOS

At the moment there's no MacOS support as I don't have a computer to test, but feel free to submit a Pull Request adding that feature, the Joy-Con reading should work just fine, only the input redirection is needed.

## Packages used

**JoyCon.NET** - ([GitHub](https://github.com/ClusterM/joycon)) ([NuGet](https://www.nuget.org/packages/JoyCon.NET))

**WiimoteLib.NetCore** - ([GitHub](https://github.com/BrianPeek/WiimoteLib)) ([NuGet](https://www.nuget.org/packages/WiimoteLib.NetCore))
