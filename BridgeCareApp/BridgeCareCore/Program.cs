using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace BridgeCareCore
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            /*try
            {
                _backgroundTaskQueue.QueueBackgroundWorkItem(async (cancellationToken) =>
                {
                    await _hubContext
                        .Clients
                        .All
                        .SendAsync("BroadcastDataMigration", "Starting data migration...");

                    using (var scope = _host.Services.CreateScope())
                    {
                        var _legacySimulationSynchronizer = scope.ServiceProvider.GetRequiredService<LegacySimulationSynchronizer>();
                        _legacySimulationSynchronizer.SynchronizeLegacySimulation(legacySimulationId);
                    }

                    await _hubContext
                        .Clients
                        .All
                        .SendAsync("BroadcastDataMigration", "Finished data migration");
                });

                return Ok();
            }
            catch (Exception e)
            {
                _logger.Error($"{e.Message}::{e.StackTrace}");
                await _hubContext
                    .Clients
                    .All
                    .SendAsync("BroadcastDataMigration", $"{e.Message}::{e.StackTrace}");
                return StatusCode(500, e);
            }*/
            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostcontext, config) =>
                {
                    var env = hostcontext.HostingEnvironment;
                    config.AddJsonFile("esec.json", optional: true, reloadOnChange: true);
#if MsSqlDebug
                    config.AddJsonFile("coreConnection.Development.json", optional: true, reloadOnChange: true);
#else
                    config.AddJsonFile("coreConnection.json", optional: true, reloadOnChange: true);
#endif
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
