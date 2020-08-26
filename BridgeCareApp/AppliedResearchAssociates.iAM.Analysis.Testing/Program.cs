using System;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using AppliedResearchAssociates.iAM.DataAccess;
using AppliedResearchAssociates.Validation;
using Microsoft.Data.SqlClient;

namespace AppliedResearchAssociates.iAM.Analysis.Testing
{
    internal static class Program
    {
        private static readonly SimulationConnectionInfo LocalZerothDataset = new SimulationConnectionInfo
        {
            ConnectionFormat = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=iAMBridgeCare;Integrated Security=True",
            NetworkId = 13,
            SimulationId = 91, // "MASTER - no commitments"
        };

        private static readonly SimulationConnectionInfo MainDataset = new SimulationConnectionInfo
        {
            ConnectionFormat = @"Data Source=52.177.117.86,56242\SQL2014;Initial Catalog=DbBackup;User Id={0};Password={1}",
            NetworkId = 13,
            SimulationId = 1171, // "JML Run District 8"
        };

        private static readonly SimulationConnectionInfo MainDatasetLocal = new SimulationConnectionInfo
        {
            ConnectionFormat = @"Server=localhost;Database=DbBackup;User Id={0};Password={1}",
            NetworkId = 13,
            SimulationId = 1171, // "JML Run District 8"
        };

        private static readonly SimulationConnectionInfo SmallBridgeDataset = new SimulationConnectionInfo
        {
            ConnectionFormat = @"Data Source=52.177.117.86,56242\SQL2014;Initial Catalog=PennDot_Light;User Id={0};Password={1}",
            NetworkId = 13,
            SimulationId = 1181, // "District 2 Initial Run"
        };

        private static readonly SimulationConnectionInfo SmallBridgeDatasetLocal = new SimulationConnectionInfo
        {
            ConnectionFormat = @"Server=localhost;Database=PennDot_Light;User Id={0};Password={1}",
            NetworkId = 13,
            SimulationId = 1181, // "District 2 Initial Run"
        };

        private static void LogProgressToConsole(TimeSpan elapsed, string label) => Console.WriteLine($"{elapsed} --- {label}");

        private static void Main()
        {
            var simulationConnectionInfo = SmallBridgeDatasetLocal;

            Console.WriteLine("User Id:");
            var userId = Console.ReadLine();
            Console.WriteLine("Password:");
            var password = Console.ReadLine();
            Console.Clear();

            var connectionString = string.Format(simulationConnectionInfo.ConnectionFormat, userId, password);

            using var connection = new SqlConnection(connectionString);
            connection.Open();

            var simulation = new DataAccessor().GetStandAloneSimulation(
                connection,
                simulationConnectionInfo.NetworkId,
                simulationConnectionInfo.SimulationId,
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
            Console.WriteLine("Network condition: " + simulation.Results.FinalStatus.ConditionOfNetwork);

            var outputFolder = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            var outputFile = $"{DateTime.Now:yyyyMMddHHmmss} - Network {simulationConnectionInfo.NetworkId} - Simulation {simulationConnectionInfo.SimulationId}.json";
            var outputPath = Path.Combine(outputFolder, outputFile);
            using var outputStream = File.Create(outputPath);
            using var outputWriter = new Utf8JsonWriter(outputStream, new JsonWriterOptions { Indented = true });
            JsonSerializer.Serialize(outputWriter, simulation.Results, new JsonSerializerOptions { Converters = { new JsonStringEnumConverter() } });
        }

        private sealed class SimulationConnectionInfo
        {
            public string ConnectionFormat;

            public int NetworkId;

            public int SimulationId;
        }
    }
}
