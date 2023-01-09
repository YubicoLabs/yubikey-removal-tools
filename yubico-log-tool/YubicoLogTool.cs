namespace Yubico;
/**
 * This is the main program starting the Yubico Log Tool.
 * 
 * This code is only an example how this feature can be implemented
 * using the Yubico Desktop SDK 1.4.0. it is not in any way ready
 * for release or production use.
 **/
class YubicoLogTool
{
    public static void Main()
    {       
        new YubiKeyLogWorkStation();
        while (true)
        {
            Thread.Sleep(2);
        }
    }

 }