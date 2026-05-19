

using Microsoft.Extensions.Logging;
using System.Runtime.InteropServices;
using userWatcher.ActivityWatcher;

[System.Runtime.Versioning.SupportedOSPlatform("windows")]

class Program
{

    public static async Task Main()
    {
        using ILoggerFactory factory = LoggerFactory.Create(builder => builder.AddConsole());

        ILogger logger = factory.CreateLogger<Program>();

        logger.LogInformation("App started at {Time}", DateTime.Now);

        CancellationTokenSource cts = new CancellationTokenSource();

        PosixSignalRegistration.Create(
            PosixSignal.SIGTERM,
            (context) => 
            {
                context.Cancel = true;
                cts.Cancel();
            });

        PosixSignalRegistration.Create(
            PosixSignal.SIGHUP,
            (context) =>
            {
                context.Cancel = true;
                cts.Cancel();
            });

        PosixSignalRegistration.Create(
            PosixSignal.SIGINT,
            (context) =>
            {
                context.Cancel = true;
                cts.Cancel();
            });

        ILogger<ActivityWatcher> activityWatcherLogger = factory.CreateLogger<ActivityWatcher>();

        ActivityWatcher watcher = new ActivityWatcher(activityWatcherLogger);

        await watcher.RunUpdateLoop(cts.Token);

        logger.LogInformation("UserWatcher stop");
    }
}
