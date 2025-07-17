# TermBar
TermBar is a window manager with a terminal, inspired heavily by [catppuccin for rainmeter](https://github.com/modkavartini/catppuccin). It's a bar that goes on the top or bottom of your screen and can easily replace the taskbar. TermBar is extremely customizable and uses the [Catppuccin palette](https://catppuccin.com/palette/) throughout.

TermBar is a C# WinUI 3 application for .NET 8.0 and is available as pre-built x64 and ARM64 packaged app releases, signed with a self-signed certificate. It runs on Windows 11 and should support Windows 10 as well, though I haven't tested it.

![A screenshot of TermBar on the desktop.](screenshots/TermBar.png)

## Features
TermBar implements a modular approach to customization: you control which modules are displayed, how many times they're displayed, how they're displayed, and in which order they're displayed.

Right clicking TermBar presents a menu that can open the settings window or Task Manager.

### Modules
Each of these modules is described in its own section below.

| Module          | Description                                                                   |
| --------------- | ----------------------------------------------------------------------------- |
| Clock           | A clock to show the time, plus a calendar that's displayed when you click it. |
| Cpu             | A CPU usage monitor.                                                          |
| Gpu             | A GPU usage monitor.                                                          |
| Launcher        | A set of launcher buttons, a lot like items pinned to the Windows 11 taskbar. |
| Memory          | A memory usage monitor.                                                       |
| StaticText      | A simple static text display.                                                 |
| SystemDropdown  | A drop-down menu providing access to system commands.                         |
| Terminal        | A fully functional terminal emulator.                                         |
| Volume          | A system volume level monitor that can adjust volume and mute.                |
| WindowBar       | A taskbar-like list of all open windows.                                      |
| WindowDropdown  | Like WindowBar, but shows open windows as a drop-down menu.                   |

### Appearance
Deeply integrating the Catppuccin palette, TermBar supports any of the four Catppuccin flavors to adjust its entire look and feel: Latte, Frappé, Macchiato, and Mocha. TermBar is designed to work with your favorite [Nerd Font](https://www.nerdfonts.com/) and can take advantage of its 10,000+ glyphs. It comes configured beautifully out of the box with the 0xProto Nerd Font and the Mocha theme, ready to be customized as much as you like.

TermBar does not use any icons; it uses Nerd Font glyphs instead. It's designed to fit in with the look and feel of your favorite terminal emulator or minimalistic desktop environment. By default, it runs on a single monitor, but can be customized to run on as many monitors as you'd like, with a separate configuration for each one. TermBar intentionally does not change its appearance based on the system Light/Dark setting.

### Window Tracking
Window tracking is highly customizable, including color and icon customization, group sorting, and window prioritization. It supports regular expressions to match window titles and select an icon based on matches, letting you easily tell apart multiple windows with the same process.

## License
TermBar is released under the MIT License.

## Configuration
TermBar uses a JSON configuration file. When first starting, it creates a default configuration file in the packaged app's `LocalState` directory. Right click TermBar to open its settings and see the location of this file, plus the current runtime configuration. TermBar never writes to the configuration file unless it doesn't exist, so you own it and manage it.

![A screenshot of the TermBar settings window.](screenshots/SettingsWindow.png)

In the [schema](schema/) directory, the TermBar configuration schema is available, including descriptions, defaults, and allowed ranges. A markdown file containing a human-readable description of the schema is also available.

### The WindowList
The WindowList configures three important things: window prioritization, group sorting, and the ProcessIconMap.

Window prioritization can be configured using HighPriorityWindows and LowPriorityWindows, each of which is an array of process names. Note that these process names cannot end with an extension. Windows with process names in HighPriorityWindows will be pinned to the beginning of any modules that list windows in the order they are listed. The same applies to LowPriorityWindows, but at the end of the list. Windows with process names in neither list are displayed after the high-priority section and before the low-priority section in the order that Windows tracks them.

Within each group of windows associated with the same process, SortGroupsAlphabetically can be used to sort them alphabetically; otherwise, they appear in the same order that Windows tracks them.

The ProcessIconMap is an array used to map window process names to icons and icon colors. It can optionally, for a process name, apply a regular expression against the window title to select icons and icon colors using each entry's WindowNameIconMap. As with HighPriorityWindows and LowPriorityWindows, process names cannot end with `.exe`.

## Modules
This section describes each of the TermBar modules.

### Clock Module
The Clock module displays the current time in a configurable format and optionally can pop out a calendar when clicked. The time is refreshed at a configurable interval and the calendar's date formats are customizable. The calendar uses the system's settings to determine the first day of the week.

![A screenshot of the TermBar Clock module.](screenshots/Clock.png)

![A screenshot of the TermBar Clock module's calendar.](screenshots/Clock-Calendar.png)

### Cpu Module
The Cpu module displays the current CPU load, refreshing at a configurable interval. The percentage can be formatted. This value is obtained via [LibreHardwareMonitorLib](https://github.com/LibreHardwareMonitor/LibreHardwareMonitor/).

![A screenshot of the TermBar Cpu module.](screenshots/Cpu.png)

### Gpu Module
The Cpu module displays the current CPU load, refreshing at a configurable interval. The percentage can be formatted. This value is obtained via [LibreHardwareMonitorLib](https://github.com/LibreHardwareMonitor/LibreHardwareMonitor/). This currently only works for NVIDIA graphics cards, though LibreHardwareMonitorLib supports AMD and Intel graphics cards as well; I simply have neither to test against.

![A screenshot of the TermBar Gpu module.](screenshots/Gpu.png)

### Launcher Module
The Launcher module presents a set of buttons that can be used to shell execute configurable items. Both the command and arguments are configurable and have environment variables replaced with their values. Anything that is launchable via Explorer should be launchable here.

Launcher entry icons can be defined specifically in the Launcher module configuration, then are checked against the WindowList's ProcessIconMap. If a match is found, the icon and color defined there are used. If not, the default icon and color defined in the Launcher module configuration are used.

![A screenshot of the TermBar Launcher module.](screenshots/Launcher.png)

### Memory Module
The Cpu module displays the current memory load, refreshing at a configurable interval. The percentage can be formatted. This value is obtained via [LibreHardwareMonitorLib](https://github.com/LibreHardwareMonitor/LibreHardwareMonitor/).

![A screenshot of the TermBar Memory module.](screenshots/Memory.png)

### StaticText Module
The StaticText module displays a static text string, with an optional icon.

![A screenshot of the TermBar StaticText module.](screenshots/StaticText.png)

### SystemDropdown Module
The SystemDropdown module provides a menu providing system commands, like shutting down, rebooting, and opening the Window settings. It can be configured to customize which options are displayed.

TODO: screenshots

### Terminal Module
The Terminal module embeds an instance of w6t (link), a fully functional terminal emulator, in TermBar. The Terminal module is presented with three rows and 60 columns by default, but can be configured to any size you like. w6t is extensively customizable; see its GitHub page for details.

By default, the Terminal module uses the same font family and font size used by TermBar, but this can be overridden if desired. The Terminal module implements its own context menu from w6t.

![A screenshot of the TermBar Terminal module.](screenshots/Terminal.png)

### Volume Module
The Volume module displays and optionally allows controlling the system default device's volume level. The volume level can be formatted.

![A screenshot of the TermBar Volume module.](screenshots/Volume.png)

### WindowBar Module
The WindowBar module displays and allows changing the active window on the desktop. Its appearance is customizable, including the size of each window in the window bar. The icons and icon colors of each window are configured via the WindowList.

![A screenshot of the TermBar WindowBar module.](screenshots/WindowBar.png)

### WindowDropdown Module
The WindowDropdown module is far more compact than the WindowBar module, but also displays and allows changing the active window on the desktop via a drop-down menu. The icons of each window are configured via the WindowList.

![A screenshot of the TermBar WindowDropdown module.](screenshots/WindowDropdown.png)

![A screenshot of the TermBar WindowDropdown module, with the menu displayed.](screenshots/WindowDropdown-Menu.png)
