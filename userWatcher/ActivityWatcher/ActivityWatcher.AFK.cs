using common;
using System.Runtime.InteropServices;

namespace userWatcher.ActivityWatcher;

[System.Runtime.Versioning.SupportedOSPlatform("windows")]

public partial class ActivityWatcher
{
    [StructLayout(LayoutKind.Sequential)]
    struct LASTINPUTINFO
    {
        public uint cbSize; // The size of the structure in bytes
        public uint dwTime; // The tick count when the last input was received
    }

    [DllImport("user32.dll")]
    static extern bool GetLastInputInfo(ref LASTINPUTINFO plii);

    private bool IsAFK
    {
        get
        {
            uint idleTime = GetIdleTime();
            if (idleTime > PreferencesManager.AfkTimeout)
            {
                return true;
            }
            return false;
        }
    }

    public static uint GetIdleTime()
    {
        LASTINPUTINFO lastInput = new LASTINPUTINFO();
        lastInput.cbSize = (uint)Marshal.SizeOf(lastInput); // This must be set manually

        if (GetLastInputInfo(ref lastInput))
        {
            return (uint)Environment.TickCount - lastInput.dwTime;
        }
        return 0;
    }
}
