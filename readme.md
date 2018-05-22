Logging Fork
============

This fork takes the functionality of the original and modifies it:
1. The application is changed to a windowed application
2. The heart rate measurement service fields are completely recorded
3. Logging of the heart rate service data to a csv file occurs automatically upon connection.

This has been tested with a Polar H7 and a Polar H10

Heart rate monitor
==================

A lightweight program to display the heart rate reading from a 
[Bluetooth Low Energy device](https://www.bluetooth.com/specifications/gatt/viewer?attributeXmlFile=org.bluetooth.characteristic.heart_rate_measurement.xml).

This has only been tested on a [Polar H7](https://www.amazon.com/dp/B007S088F4) (non-referral link) but others should work.

Requires Windows 8.1 or newer.

The executable and code are released under MIT license.

Motivation
----------
I owned a Polar H7 I could not get to work with any iOS software for use in the
gym. I decided to repurpose it as a way to read my heartrate while I worked on
my computer. Having found no software, I decided to create my own.

The program's secondary purpose is for Twitch streamers, because having looked
into their setups, I found there was a mixture of excessive hardware and
software needed. I am not a Twitch streamer and am willing to add the
customizations needed by request.

Notice on deployment
--------
Some builds of Windows 10 have bug related to security which causes Desktop
apps to stop receiving notification callbacks from GATT characteristics after
receiving a few initial callbacks. One may find more details in this post:
https://social.msdn.microsoft.com/Forums/en-US/58da3fdb-a0e1-4161-8af3-778b6839f4e1/
or in similar posts on social.msdn.microsoft.com.

There are several workarounds, the simplest being creating an AppId for the app in
the Windows Registry and adding a security setting for the AppId, like below:

Windows Registry Editor Version 5.00

[HKEY_LOCAL_MACHINE\SOFTWARE\Classes\AppID\{C6BFD646-3DF0-4DE5-B7AF-5FFFACB844A5}]
"AccessPermission"=hex:01,00,04,80,9c,00,00,00,ac,00,00,00,00,00,00,00,14,00,\
  00,00,02,00,88,00,06,00,00,00,00,00,14,00,07,00,00,00,01,01,00,00,00,00,00,\
  05,0a,00,00,00,00,00,14,00,03,00,00,00,01,01,00,00,00,00,00,05,12,00,00,00,\
  00,00,18,00,07,00,00,00,01,02,00,00,00,00,00,05,20,00,00,00,20,02,00,00,00,\
  00,18,00,03,00,00,00,01,02,00,00,00,00,00,0f,02,00,00,00,01,00,00,00,00,00,\
  14,00,03,00,00,00,01,01,00,00,00,00,00,05,13,00,00,00,00,00,14,00,03,00,00,\
  00,01,01,00,00,00,00,00,05,14,00,00,00,01,02,00,00,00,00,00,05,20,00,00,00,\
  20,02,00,00,01,02,00,00,00,00,00,05,20,00,00,00,20,02,00,00

[HKEY_LOCAL_MACHINE\SOFTWARE\Classes\AppID\YOURAPP.exe]
"AppID"="{C6BFD646-3DF0-4DE5-B7AF-5FFFACB844A5}"


How to use
----------
1. Connect appropriate bluetooth device inside Window's bluetooth settings.
2. [Download prebuilt binaries](https://github.com/jlennox/HeartRate/releases) or build from source.
3. Run HeartRate.exe

I do not have the time to turn this into production grade software but I am
willing to expand it to your needs. At startup, the program searches for an
appropriate device and attempts to register a notification event for
`heart_rate_measurement`. If this fails, the error is likely vague and not overly
helpful. If you believe your device should work with this software but does
not, please open a github issue with specifics.

This will not work for any general USB/bluetooth heart rate monitor. It has to
be a Bluetooth Low Energy device supporting `heart_rate_measurement`.

By default, only an icon in the system tray is displayed. When the heart rate
goes over an alert threshold, a balloon notification shows.

Clicking the system tray icon reveals a window with scaling text. This is for
Twitch streamers to be able to region for broadcast.

Settings
--------
Right clicking the system tray icon gives the option to edit an XML settings
file. When the editor is closed, the settings will be reloaded automatically.
The file is `%appdata%\HeartRate\settings.xml`

`color` values are formatted as 32bit ARGB hex values. Leading 0's are optional.

| Setting    | Type | Default  | Description |
|------------|------|----------|-------------|
| `FontName` | text | Arial | The font name for the system tray icon. |
| `UIFontName` | text | Arial | The font name for the window display. |
| `AlertLevel` | number | 65 | The heart rate to display a system tray notification balloon at. 0 to disable. |
| `WarnLevel` | number | 70 | The heart rate to use `WarnColor` at. 0 to disable. |
| `AlertTimeout` | number | 120000 | The amount of milliseconds to cooldown for being able to show an alert after one was shown. |
| `DisconnectedTimeout` | number | 10000 | The amount of milliseconds after disconnecting to await for a valid device connection before displaying "X" |
| `Color` | color | FFADD8E6 | The default color for the system tray icon. |
| `WarnColor` | color | FFFF0000 | The system tray icon color once `WarnLevel` has been met. |
| `UIColor` | color | FF00008B | The default color for the window display. |
| `UIWarnColor` | color | FFFF0000 | The window display color once `WarnLevel` has been met. |
| `UIBackgroundColor` | color | 00FFFFFF | The background color for the window display. |
