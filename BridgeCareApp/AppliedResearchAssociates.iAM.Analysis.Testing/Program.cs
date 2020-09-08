using System;
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
            const int networkId = 13;

            Console.WriteLine("User Id:");
            var userId = Console.ReadLine();
            Console.WriteLine("Password:");
            var password = Console.ReadLine();
            Console.WriteLine("Network ID:");
            Console.WriteLine(networkId);
            Console.WriteLine("Simulation ID:");
            var simulationId = int.Parse(Console.ReadLine());
            Console.Clear();

            var connectionString = string.Format(ConnectionFormats.SmallBridgeDatasetLocal, userId, password);

            using var connection = new SqlConnection(connectionString);
            connection.Open();

            var simulation = new DataAccessor().GetStandAloneSimulation(
                connection,
                networkId,
                simulationId,
                LogProgressToConsole);

            var errorIsPresent = false;
            foreach (var result in simulation.Network.Explorer.GetAllValidationResults())
            {
                errorIsPresent |= result.Status == ValidationStatus.Error;
                Console.WriteLine($"[{result.Status}] {result.Message} --- {result.Target.Object}::{result.Target.Key}");
            }

            if (errorIsPresent)
            {
                Console.WriteLine("Analysis should not run when validation errors are present. Terminating execution...");
                Environment.Exit(1);
            }

            var runner = new SimulationRunner(simulation);
            runner.Information += (sender, eventArgs) => Console.WriteLine(eventArgs.Message);
            runner.Warning += (sender, eventArgs) => Console.WriteLine(eventArgs.Message);

            var timer = Stopwatch.StartNew();
            runner.Run();
            LogProgressToConsole(timer.Elapsed, "simulation run");

            Console.WriteLine();
            Console.WriteLine("Network condition: " + simulation.Results.Years.Last().ConditionOfNetwork);

            var outputFolder = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            var outputFile = $"{DateTime.Now:yyyyMMddHHmmss} - Network {networkId} - Simulation {simulationId}.json";
            var outputPath = Path.Combine(outputFolder, outputFile);
            using var outputStream = File.Create(outputPath);
            using var outputWriter = new Utf8JsonWriter(outputStream, new JsonWriterOptions { Indented = true });
            JsonSerializer.Serialize(outputWriter, simulation.Results, new JsonSerializerOptions { Converters = { new JsonStringEnumConverter() } });
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
