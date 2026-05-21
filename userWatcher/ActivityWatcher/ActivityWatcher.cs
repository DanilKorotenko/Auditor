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
        if (IsAFK)
        {
            return new Activity.AFKActivity();
        }

        ProcessInfo? processInfo = ProcessInfo.GetCurrentProcessInfo();

        if (processInfo == null)
        {
            return new Activity.Activity();
        }

        Activity.Activity result = new Activity.Activity();

        result.ProcessName = processInfo.ProcessName;

        return result;
    }

    private void Shutdown()
    {
        logger.LogInformation($"Shutdown");
    }
}
