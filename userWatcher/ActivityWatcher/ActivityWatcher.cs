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

public partial class ActivityWatcher
{
    [DllImport("user32.dll")]
    private static extern IntPtr GetForegroundWindow();

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

    [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    private static extern int GetWindowTextLength(IntPtr hWnd);

    [DllImport("user32.dll")]
    private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);


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

    private void Shutdown()
    {
        logger.LogInformation($"Shutdown");
    }
}
