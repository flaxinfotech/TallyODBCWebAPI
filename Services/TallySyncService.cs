using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using TallyIntegrationAPI.Services;

public class TallySyncService : BackgroundService
{
    private readonly OdbcService _odbcService;

    public TallySyncService(OdbcService odbcService)
    {
        _odbcService = odbcService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                // Example: Fetch all ledgers and sync with the database
                var ledgers = await _odbcService.GetFilteredLedgersAsync();
                // Perform sync logic (e.g., update your database)
                Console.WriteLine($"Synced {ledgers.Count} ledgers at {DateTime.Now}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during sync: {ex.Message}");
            }

            // Wait for a specified interval before the next sync
            await Task.Delay(TimeSpan.FromMinutes(10), stoppingToken);
        }
    }
}
