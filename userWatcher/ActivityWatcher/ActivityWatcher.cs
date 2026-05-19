using common;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace userWatcher.ActivityWatcher;

[System.Runtime.Versioning.SupportedOSPlatform("windows")]

public class ActivityWatcher
{
    [DllImport("user32.dll")]
    private static extern IntPtr GetForegroundWindow();

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

    [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    private static extern int GetWindowTextLength(IntPtr hWnd);

    [DllImport("user32.dll")]
    private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

    [StructLayout(LayoutKind.Sequential)]
    struct LASTINPUTINFO
    {
        public uint cbSize; // The size of the structure in bytes
        public uint dwTime; // The tick count when the last input was received
    }

    [DllImport("user32.dll")]
    static extern bool GetLastInputInfo(ref LASTINPUTINFO plii);


    private readonly int DEFAULT_UPDATE_INTERVAL = 1;

    private readonly ILogger<ActivityWatcher> logger;

    public ActivityWatcher(ILogger<ActivityWatcher> aLogger)
    {
        logger = aLogger;
        UpdateInterval = DEFAULT_UPDATE_INTERVAL;
    }

    private int UpdateInterval { get; set; }

    public async Task RunUpdateLoop(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                Update();
            }
            catch (Exception ex)
            {
                string error = $"Error in update loop: {ex.Message}";
                logger.LogError(error);
            }
            try
            {
                await Task.Delay(TimeSpan.FromSeconds(UpdateInterval), cancellationToken);
            }
            catch { }
        }
        Shutdown();
    }

    void Update()
    {

        Activity.Activity activity = GetCurrentActivity();
        logger.LogInformation($"{activity}");
    }

    public Activity.Activity GetCurrentActivity()
    {
        if (IsAFK)
        {
            return new Activity.AFKActivity();
        }
        Activity.Activity result = new Activity.Activity();

        return result;
    }

    private bool IsAFK
    {
        get
        {
            uint idleTime = GetIdleTime();
            logger.LogInformation($"Idle time {idleTime}");
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


    private void Shutdown()
    {
        logger.LogInformation($"Shutdown");
    }
}
