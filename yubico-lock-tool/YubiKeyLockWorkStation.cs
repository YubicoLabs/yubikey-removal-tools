namespace Yubico;

using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Yubico.YubiKey;


/**
 * Is used to track the behaviour of Insert or remove
 * of yubiKeys. If an YubiKey is removed then the computer locks.
 *
 * This code is only an example how this feature can be implemented
 * using the Yubico Desktop SDK 1.4.0. it is not in any way ready
 * for release or production use.
 **/
public class YubiKeyLockWorkStation
{
    // Define the use of LockWorkStation command in the user32.dll
    [DllImport("user32")]
    private static extern void LockWorkStation();

    private YubiKeyDeviceListener yubiKeyDeviceListener = YubiKeyDeviceListener.Instance; // Set the Device Listener from Yubico SDK.
    private YubiKeyInfo _yubikey = null; // YubiKey info
    private bool _yubiKeyInserted = false; //Boolean used to see if the YubiKey is inserted in the computer or not.

    public YubiKeyLockWorkStation()
    {
        // Subscribe to the remove and insert listener event and which methods to use in the case it happens.yubiKeyDeviceListener.Removed += YubiKeyRemoved;
        yubiKeyDeviceListener.Arrived += YubiKeyInserted;
        yubiKeyDeviceListener.Removed += YubiKeyRemoved;
    }

    /**
     * Called if the YubiKey is removed from the computer. This method fetches the
     * serial number if the YubiKey wasn't inserted before, logs the event depending
     * if it is an YubiKey or SecurityKey used and finally lock the user account.
     **/
    private void YubiKeyRemoved(object? sender, YubiKeyDeviceEventArgs eventArgs)
    {
        // Check if a YubiKey is plugged in and if so fetch the serial number for that YuibiKey
        if (!_yubiKeyInserted)
        {
            _yubikey = new YubiKeyInfo(chooseFirstYubiKey());
            _yubiKeyInserted = false;
        }

        // If the serial number is set then it is a YubiKey and if null then it is a SecurityKey. Then a log event is created
        if (_yubikey != null && _yubikey.Fido)
        {
            LogInfo(Environment.UserName + " locked the computer when removing the " + _yubikey.Name + ".", 4800);

            //Locks the computer
            LockWorkStation();
        }
    }

    /**
      * Called if the YubiKey is inserted in the computer. This method fetches the
      * serial number of the YubiKey and logs the insert event depending
      * if it is an YubiKey or SecurityKey used.
      **/
    private void YubiKeyInserted(object? sender, YubiKeyDeviceEventArgs eventArgs)
    {
        _yubikey = new YubiKeyInfo(chooseFirstYubiKey());

        if (_yubikey != null && _yubikey.Fido)
        {
            _yubiKeyInserted = true;
            LogInfo(_yubikey.Name + " was inserted.", 6416);
        }
    }

    /**
     * Chooses the first USB-based YubiKey that exposes any functionality.
     **/
    private IYubiKeyDevice? chooseFirstYubiKey()
    {
        IYubiKeyDevice? firstYubiKey = null;
        IEnumerable<IYubiKeyDevice> list = YubiKeyDevice.FindAll();

        if (list.Any())
        {
            firstYubiKey = list.First();
        }

        return firstYubiKey;
    }

    /**
        * Takes a string and eventID that should be added in the Application event log.
        **/
    private void LogInfo(string message, int eventID)
    {
        EventLog eventLog = new EventLog();
        eventLog.Source = "Application";
        eventLog.WriteEntry(message, EventLogEntryType.Information, eventID);
    }
}
