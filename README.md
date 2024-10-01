[![Language](https://img.shields.io/badge/.NET-6.0-blue)]()


# yubikey-removal-tool
This is code examples on how a YubiKey Removal behaviour can be implemented in Windows 10/11. This is C# code using the Yubico Desktop SDK. This tool is built for YubiKeys and then it can be run as a standard user in Windows, but to be able to get info from a SecurityKey or a YubiKey Bio Administrative rights is needed due to how Microsoft has designed FIDO access and how the Yubico Enterprise SDK is leverage that.

## Basic Usage ##

`Yubico Lock Tool` used as example to lock the computer if YubiKey is removed
`Yubico Logout Tool` used as example to log out the user if YubiKey is removed


## Pre-requisites: ##

This should be developed and run on Windows 10 or Windows 11.

* Download and Install Visual Studio
* Download or fetch this code from Github


## Build ##

1. After all pre-requisites are installed and downloaded, open Visual Studio.
2. Build the code and publish the exe-file
3. Find the exe fil in Explorer
4. Double click on the exe and test how it works.

## Example - How to start automatic on logon ##

To make this code little bit better and to make it start on startup you can add these things:
1. Make a shortcut and add that into the startup folder, which makes it run directly on startup. Here is how you do that:
  - Create a shortcut of the YubicoLockTool.exe or YubicoLogoutTool.exe
  - Move the shortcut to the desktop and rename as you like
  - Open the Run command (Windows Logo + R or search for Run)
  - Write shell:startup and press Enter
  - Drag the shortcut into the Startup folder

2. Change the YubicoLockTool.cs or YubicoLogoutTool.cs to have those lines (**Replace [ExampleClass] below with correct class name**):

        using System.Runtime.InteropServices;
        namespace Yubico;

        class [ExampleClass] {
          [DllImport("kernel32.dll")]
          static extern IntPtr GetConsoleWindow();

          [DllImport("user32.dll")]
          static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        public static void Main() {
          var handle = GetConsoleWindow();

          // Hide
          ShowWindow(handle, 0);

          ...

## News
 - Latest version has a better YubiKey Information class for accurancy of YubiKey models.
 - Latest version also has a log tool

## [License](#license)

Yubico Removal Tool is provided under the [Apache License 2.0](https://github.com/YubicoLabs/se-internal/tree/master/yubikey-removal-tools/LICENSE).
