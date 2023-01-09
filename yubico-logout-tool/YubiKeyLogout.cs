namespace Yubico;

using System.Diagnostics;
using System.Runtime.InteropServices;
using Yubico.YubiKey;

/**
 * Is used to track the behaviour of Insert or remove
 * of yubiKeys. If an YubiKey is removed then the user is signed out.
 * 
 * This code is only an example how this feature can be implemented
 * using the Yubico Desktop SDK 1.4.0. it is not in any way ready
 * for release or production use. 
 **/
public class YubiKeyLogout {
    // Define the use of ExitWindowsEx command in the user32.dll
    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool ExitWindowsEx(uint uFlags, uint dwReason);

    private YubiKeyDeviceListener yubiKeyDeviceListener = YubiKeyDeviceListener.Instance; // Set the Device Listener from Yubico SDK.
    private YubiKeyInfo _yubikey = null; // YubiKey info

    public YubiKeyLogout() {
        // Subscribe to the remove listener event and which methods to use in the case it happens.
        yubiKeyDeviceListener.Removed += YubiKeyRemoved;
    }

    /**
     * Called if the YubiKey is removed from the computer. This method fetches
     * YubiKey info, logs the event depending of YubiKey type and finally log 
     * out the user.
     **/
    private void YubiKeyRemoved(object? sender, YubiKeyDeviceEventArgs eventArgs) {
        // Get the YubiKey info the YubiKey plugged into the computer.
        _yubikey = new YubiKeyInfo(chooseFirstYubiKey());

        // If it is a FIDO Key containing name, version and type information then a log event is created
        if (_yubikey != null && _yubikey.Fido) { 
            LogInfo(Environment.UserName + " logged out removing " + _yubikey.Name + ".", 4647);

            //Log out the user from Windows
            WindowsLogOff();
        }
    }

    /**
     * Chooses the first USB-based YubiKey that exposes any functionality and have the FIDO2 interface enabled.
     **/
    private IYubiKeyDevice? chooseFirstYubiKey() {
        IYubiKeyDevice? firstYubiKey = null;
        IEnumerable<IYubiKeyDevice> list = YubiKeyDevice.FindAll();

        if (list.Any()){
            firstYubiKey = list.First();
        }

        return firstYubiKey;
    }

    /**
     * Log out the current user.
     **/
    private static bool WindowsLogOff() {
        return ExitWindowsEx(0 | 0x00000004, 0);
    }

    /**
     * Takes a string and eventID that should be added in the Application event log.
     **/
    private void LogInfo(string message, int eventID) {
        EventLog eventLog = new EventLog();
        eventLog.Source = "Application";
        eventLog.WriteEntry(message, EventLogEntryType.Information, eventID);

    }
}
