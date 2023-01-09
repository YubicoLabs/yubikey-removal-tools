namespace Yubico;

using Yubico.YubiKey;

/**
* Class to hold all needed information about a YubiKey.
**/
class YubiKeyInfo : Exception
{
    private IYubiKeyDevice _yubiKey;
    private string _serialNumber = "";
    private string _version = "";
    private string _name = "";
    private string _type = "";
    private int _family = 0;
    private bool _fips = false;
    private bool _special = false;
    private bool _nfc = false;
    private bool _fido = false;
    private bool _sky = false;
    private bool _bio = false;
    private string[] _formFactorTranslation = { "Unknown", "A", "A Nano", "C", "C Nano", "Ci", "Bio USB-A", "Bio USB-C" };

    public YubiKeyInfo(IYubiKeyDevice yubiKey){
        setYubiKey(yubiKey);
        setSerialNumber();
        setVersion();
        setType();
        setFamily();
        setFido();
        setFIPS();
        setSKY();
        setNFC();
        setBio();
        setName();
    }

    /**
        * Set the YubiKey used. 
        **/
    private void setYubiKey(IYubiKeyDevice yubiKey)
    {
        _yubiKey = yubiKey;
    }

    /**
    * Set the name of the YubiKey used with extra info such as 
    * version, serial number and FIPS as well as which sort of
    * YubiKey used. 
    * To be able to read all info from a SecurityKey or YubiKey Bio (as using FIDO)
    * the program needs to have Administrator rights in Windows.
    * 
    * E.g. YubiKey 5C NFC FIPS (v5.4.3) with serial number 101112131
    *      SecurityKey USB-C NFC (v5.1.2)
    *      YubiKey Bio USB-A (v5.6.0)
    *      Enterprise SecurityKey USB-C NFC (v5.6.0) with serial number 101112131
    **/
    private void setName(){
        if (_family > 0)
        {
            // Set the name for a SecurityKey
            if (_sky)
            {
                if (!_serialNumber.Equals(""))
                {
                    _name = "Enterprise ";
                }

                _name += "SecurityKey";
                if (_type.Equals("Unknown"))
                {
                    _name += " USB-A";
                }
                else
                {
                    _name += " USB-"+_type;
                }

            }
            else
            {
                _name = "YubiKey";

                // Set the name for a YubiKey Bio
                if (_bio)
                {
                    _name += " "+_type;
                }
                else
                // Set the name for a normal YubiKey
                {
                    _name += " " + _family;
                    if (_family > 4) { 
                        _name += _type;
                    }
                }
            }

            if (_nfc)
            {
                _name += " NFC";
            }

            if (_fips)
            {
                _name += " FIPS";
            }

            _name += " (v" + _version + ")";

            if (!_serialNumber.Equals(""))
            {
                _name += " with serial number " + _serialNumber;
            }
        } else
        {
            _name = "YubiKey";
        }
    }

    /**
    * Set the serial number of this YubiKey if it exists depending
    * on which YubiKey is used. For the time being YubiKey Bio don't 
    * provide a serial number.
    **/
    private void setSerialNumber()
    {
        if (_yubiKey.SerialNumber != null){
            _serialNumber = ((int)_yubiKey.SerialNumber).ToString();
        }
    }

    /**
    * Set if this is a FIPS YubiKey. 
    **/
    private void setFIPS()
    {
        if (_yubiKey.IsFipsSeries){
            _fips = true;
        }
    }

    /**
    * Set if this is a NFC enabled YubiKey. 
    **/
    private void setNFC()
    {
        if (_yubiKey.AvailableNfcCapabilities != 0)
        {
            _nfc = true;
        }
    }

    /**
    * Set if this is a SecurityKey. 
    **/
    private void setSKY()
    {
        if (_yubiKey.IsSkySeries)
        {
            _sky = true;
        }
    }

    /**
    * Set the version of this YubiKey. This is only the first part 
    * of the firmware version e.g. 5.4.3 => 5 
    **/
    private void setVersion() => _version = _yubiKey.FirmwareVersion.ToString();

    /**
    * Set the family of this YubiKey. This is only the first part 
    * of the firmware version e.g. 5.4.3 => 5 
    **/
    private void setFamily() => _family = int.Parse(_version.Split('.')[0]);

    /**
    * Define which YubiKey is used and set the name of that
    * to YubiKey, SecurityKey by Yubico or YubiKey Bio. 
    **/
    private void setType() => _type = _formFactorTranslation[(int)_yubiKey.FormFactor];

    /**
    * Set if this YubiKey is a Biometric YubiKey.
    **/
    private void setBio()
    {
        if (_yubiKey.FormFactor == FormFactor.UsbABiometricKeychain || _yubiKey.FormFactor == FormFactor.UsbCBiometricKeychain)
        {
            _bio = true;
        }
    }

    /**
     * Check if FIDO is enabled on the YubiKey and as good as possible
     * on SecurityKey or YubiKey Bio. 
     **/
    private void setFido()
    {
        if (_special && _yubiKey.AvailableUsbCapabilities.HasFlag(YubiKeyCapabilities.FidoU2f)){
            _fido = true;
        }
        else if (_yubiKey.EnabledUsbCapabilities.HasFlag(YubiKeyCapabilities.Fido2)){
            _fido = true;
        }
    }

    public string Name => _name;
    public bool Fido => _fido;
}