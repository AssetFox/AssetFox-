using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using AppliedResearchAssociates.iAM.DataAccess;
using AppliedResearchAssociates.Validation;
using Microsoft.Data.SqlClient;

namespace AppliedResearchAssociates.iAM.Analysis.Testing
{
    internal static class Program
    {
        private static void LogProgressToConsole(TimeSpan elapsed, string label) => Console.WriteLine($"{elapsed} --- {label}");

        private static void Main()
        {
            Console.WriteLine("User Id:");
            var userId = Console.ReadLine();

            Console.WriteLine("Password:");
            var password = Console.ReadLine();

            Console.Clear();

            var connectionString = string.Format(ConnectionFormats.SmallBridgeDatasetLocal, userId, password);

            using var connection = new SqlConnection(connectionString);
            connection.Open();

            var accessor = new DataAccessor(connection, LogProgressToConsole);

            Console.WriteLine("Network/Simulation ID specification:");
            // E.g. "net1:sim1,sim2,sim3;net2" where sims 1, 2, and 3 of net1 are run, and all sims
            // of net2 are run. Blank input means run everything.
            var idSpec = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(idSpec))
            {
                accessor.RequestedSimulationsPerNetwork = new SortedDictionary<int, SortedSet<int>>();
                foreach (var networkSpec in idSpec.Split(";"))
                {
                    var netSims = networkSpec.Split(':');
                    var net = int.Parse(netSims[0]);
                    var sims = netSims[1].Split(',').Select(int.Parse).ToSortedSet();
                    accessor.RequestedSimulationsPerNetwork.Add(net, sims);
                }
            }

            var explorer = accessor.GetExplorer();

            var errorIsPresent = false;
            foreach (var result in explorer.GetAllValidationResults())
            {
                errorIsPresent |= result.Status == ValidationStatus.Error;
                Console.WriteLine($"[{result.Status}] {result.Message} --- {result.Target.Object}::{result.Target.Key}");
            }

            if (errorIsPresent)
            {
                Console.WriteLine("Analysis should not run when validation errors are present. Run will proceed anyway and will exclude simulations with validation errors.");
            }

            var connectionStringBuilder = new SqlConnectionStringBuilder(connectionString);
            var outputFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "iAM Analysis Testing Outputs", connectionStringBuilder.InitialCatalog, DateTime.Now.ToString("yyyyMMddHHmmss"));
            _ = Directory.CreateDirectory(outputFolder);

            foreach (var network in explorer.Networks)
            {
                var networkId = accessor.IdPerNetwork[network];

                foreach (var simulation in network.Simulations)
                {
                    var simulationId = accessor.IdPerSimulation[simulation];

                    Console.WriteLine();

                    if (simulation.GetAllValidationResults().Any(result => result.Status == ValidationStatus.Error))
                    {
                        Console.WriteLine($"Skipping {network.Name} ({networkId}) {simulation.Name} ({simulationId}) due to validation errors.");
                        continue;
                    }

                    Console.WriteLine($"Running {network.Name} ({networkId}) {simulation.Name} ({simulationId}) ...");

                    var runner = new SimulationRunner(simulation);
                    runner.Information += (sender, eventArgs) => Console.WriteLine(eventArgs.Message);
                    runner.Warning += (sender, eventArgs) => Console.WriteLine(eventArgs.Message);
                    runner.Failure += (sender, eventArgs) => Console.WriteLine(eventArgs.Message);

                    var timer = Stopwatch.StartNew();

                    try
                    {
                        runner.Run();
                    }
                    catch (SimulationException e)
                    {
                        Console.WriteLine("Simulation failed: " + e.Message);
                        continue;
                    }

                    LogProgressToConsole(timer.Elapsed, "simulation run");

                    Console.WriteLine("Final condition of network: " + simulation.Results.Years.Last().ConditionOfNetwork);

                    var outputFile = $"Network {networkId} - Simulation {simulationId}.json";
                    var outputPath = Path.Combine(outputFolder, outputFile);
                    using var outputStream = File.Create(outputPath);
                    using var outputWriter = new Utf8JsonWriter(outputStream, new JsonWriterOptions { Indented = true });
                    JsonSerializer.Serialize(outputWriter, simulation.Results, new JsonSerializerOptions { Converters = { new JsonStringEnumConverter() } });
                }
            }
        }

        private static class ConnectionFormats
        {
            public const string MainDataset = @"Data Source=52.177.117.86,56242\SQL2014;Initial Catalog=DbBackup;User Id={0};Password={1}";
            public const string MainDatasetLocal = @"Server=localhost;Database=DbBackup;User Id={0};Password={1}";
            public const string SmallBridgeDataset = @"Data Source=52.177.117.86,56242\SQL2014;Initial Catalog=PennDot_Light;User Id={0};Password={1}";
            public const string SmallBridgeDatasetLocal = @"Server=localhost;Database=PennDot_Light;User Id={0};Password={1}";
        }
    }
}
