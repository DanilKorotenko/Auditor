using auditorAgent;

[System.Runtime.Versioning.SupportedOSPlatform("windows")]

class Program
{
    static void Main(string[] args)
    {
        HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
        builder.Services.AddWindowsService(
            options =>
            {
                options.ServiceName = "eniAgent";
            });

        //LoggerProviderOptions.RegisterProviderOptions<EventLogSettings, EventLogLoggerProvider>(services: builder.Services);

        builder.Services.AddLogging();

        // Register Services
        builder.Services.AddSingleton<AuditorService>();

        builder.Services.AddHostedService<WindowsBackgroundService>();

        IHost host = builder.Build();
        host.Run();
    }
}
