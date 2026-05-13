

using Microsoft.Extensions.Logging;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using userWatcher.ActivityWatcher;

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
            (_) => 
            {
                cts.Cancel();
                //Environment.Exit(0); 
            });

        ILogger<ActivityWatcher> activityWatcherLogger = factory.CreateLogger<ActivityWatcher>();

        ActivityWatcher watcher = new ActivityWatcher(activityWatcherLogger);

        await watcher.RunUpdateLoop(cts.Token);

        logger.LogInformation("UserWatcher stop");
    }
}
