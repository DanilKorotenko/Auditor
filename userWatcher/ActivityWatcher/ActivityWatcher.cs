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
        Activity.Activity result = new Activity.Activity();

        do
        {
            if (IsAFK)
            {
                result = new Activity.AFKActivity();
                break;
            }

            ProcessInfo? processInfo = ProcessInfo.GetCurrentProcessInfo();
            if (processInfo == null)
            {
                break;
            }

            if (processInfo.IsBrowser)
            {
                result = new Activity.BrowserActivity();
            }
            else
            {
                result = new Activity.AppActivity();
            }

            result.SetProcessInfo(processInfo);
        }
        while (false);

        return result;
    }

    private void Shutdown()
    {
        logger.LogInformation($"Shutdown");
    }
}
