using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace auditorAgent;

[System.Runtime.Versioning.SupportedOSPlatform("windows")]

public class AuditorService
{
    private readonly int DEFAULT_HEARTBEAT_INTERVAL = 5;

    private readonly ILogger<AuditorService> logger;


    public AuditorService(ILogger<AuditorService> aLogger)
    {
        logger = aLogger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Start watching USB drives");

        // Run update loop
        await RunUpdateLoop(cancellationToken);
    }


    private async Task RunUpdateLoop(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {

            try
            {
                await Update();
            }
            catch (Exception ex)
            {
                string error = $"Error in update loop: {ex.Message}";
                logger.LogError(error);

            }

            await Task.Delay(TimeSpan.FromSeconds(DEFAULT_HEARTBEAT_INTERVAL), cancellationToken);
        }
    }

    async Task Update()
    {
        await Heartbeat();
    }

    async Task Heartbeat()
    {
        logger.LogInformation($"Heartbeat");

    }




}
