namespace Helpers;

public static class HW
{

    public static void VibrateDevice(int time = 50)
    {
        if (DeviceInfo.Platform == DevicePlatform.Android)
        {
            TimeSpan vibrationLength = TimeSpan.FromMilliseconds(time);
            Vibration.Default.Vibrate(vibrationLength);
        }
    }
}