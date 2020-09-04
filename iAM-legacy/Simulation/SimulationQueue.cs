﻿using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.DataAccess;
using AppliedResearchAssociates.Validation;
using log4net;
using MongoDB.Driver;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using static Simulation.Simulation;

namespace Simulation
{
    public sealed class SimulationQueue : IDisposable
    {
        public SimulationQueue(int maximumConcurrency) => MaximumConcurrency = maximumConcurrency;

        private static readonly ILog log = LogManager.GetLogger(typeof(SimulationQueue));
        private static string MongoConnection;
        private static IMongoCollection<SimulationModel> Simulations;
        private static int SimulationId;
        private static void LogProgressToConsole(TimeSpan elapsed, string label)
        {
            log.Info($"NewAnalysis: {elapsed} --- {label}");
 
            var updateStatus = Builders<SimulationModel>.Update.Set(s => s.status, $"{elapsed} --- {label}");
            Simulations.UpdateOne(s => s.simulationId == SimulationId, updateStatus);
        }

        private static SimulationQueue mainSimulationQueueInstance = null;
        public static SimulationQueue MainSimulationQueue
        {
            get
            {
                lock (typeof(SimulationQueue))
                {
                    if (mainSimulationQueueInstance == null)
                    {
                        mainSimulationQueueInstance = new SimulationQueue(1);
                    }
                    return mainSimulationQueueInstance;
                }
            }
        }

        public int MaximumConcurrency
        {
            get => _MaximumConcurrency;
            set
            {
                lock (Lock_MaximumConcurrency)
                {
                    _MaximumConcurrency = Math.Max(1, value);
                    while (Consumers.Count < _MaximumConcurrency)
                    {
                        var consumer = new Task(Consume, Consumers.Count);
                        if (!Consumers.TryAdd(Consumers.Count, consumer))
                        {
                            throw new InvalidOperationException("Consumer already exists.");
                        }

                        consumer.Start();
                    }
                }
            }
        }

        public void Dispose() => Queue.CompleteAdding();

        public Task Enqueue(SimulationParameters simulationParameters, CancellationToken cancellationToken = default)
        {
            var task = new Task(Simulation, simulationParameters, cancellationToken);
            Queue.Add(task);
            return task;
        }

        // It is probably a temporary method to run simulation on the new refactored Analysis
        public Task EnqueueForNewAnalysis(SimulationParameters simulationParameters, CancellationToken cancellationToken = default)
        {
            var task = new Task(SimulationForNewAnalysis, simulationParameters, cancellationToken);
            Queue.Add(task);
            return task;
        }

        // Allow generic tasks to be enqueued, if it is unsafe to run a simulation while those tasks are running
        public Task Enqueue(Task nonSimulationTask)
        {
            Queue.Add(nonSimulationTask);
            return nonSimulationTask;
        }

        public Task Enqueue(Action nonSimulationAction, CancellationToken cancellationToken = default)
        {
            var task = new Task(nonSimulationAction, cancellationToken);
            Queue.Add(task);
            return task;
        }

        private readonly ConcurrentDictionary<int, Task> Consumers = new ConcurrentDictionary<int, Task>();

        private readonly object Lock_MaximumConcurrency = new object();

        private readonly BlockingCollection<Task> Queue = new BlockingCollection<Task>();

        private int _MaximumConcurrency;

        private static void Simulation(object state)
        {
            var parameters = (SimulationParameters)state;
            var simulation = new Simulation(parameters.SimulationName, parameters.NetworkName, parameters.SimulationId, parameters.NetworkId, parameters.ConnectionString);
            simulation.CompileSimulation(parameters.IsApiCall);
        }

        private static void SimulationForNewAnalysis(object state)
        {
            var parameters = (SimulationParameters)state;

            MongoConnection = parameters.ConnectionString;
            var mongoClient = new MongoClient(MongoConnection);
            var mongoDB = mongoClient.GetDatabase("BridgeCare");
            Simulations = mongoDB.GetCollection<SimulationModel>("scenarios");
            SimulationId = parameters.SimulationId;

            using var connection = new SqlConnection(parameters.SQLConnection);
            connection.Open();
            var newSimulation = new DataAccessor().GetStandAloneSimulation(
            connection,
            parameters.NetworkId,
            parameters.SimulationId,
            LogProgressToConsole);

            var errorIsPresent = false;
            foreach (var result in newSimulation.Network.Explorer.GetAllValidationResults())
            {
                errorIsPresent |= result.Status == ValidationStatus.Error;
                Console.WriteLine($"[{result.Status}] {result.Message} --- {result.Target.Object}::{result.Target.Key}");

                log.Info($"NewAnalysis: [{result.Status}] {result.Message} --- {result.Target.Object}::{result.Target.Key}");
                var updateStatus = Builders<SimulationModel>.Update.Set(s => s.status, $"[{result.Status}] {result.Message} - {result.Target.Key}");
                Simulations.UpdateOne(s => s.simulationId == SimulationId, updateStatus);
            }

            if (errorIsPresent)
            {
                Console.WriteLine("Analysis should not run when validation errors are present. Terminating execution...");
                log.Error("NewAnalysis: Analysis should not run when validation errors are present. Terminating execution...");
                //Environment.Exit(1);
            }

            var runner = new SimulationRunner(newSimulation);
            runner.Information += (sender, eventArgs) => { 
                log.Info(eventArgs.Message);
                var updateStatus = Builders<SimulationModel>.Update.Set(s => s.status, $"{eventArgs.Message}");
                Simulations.UpdateOne(s => s.simulationId == SimulationId, updateStatus);
            };
            runner.Warning += (sender, eventArgs) => { 
                log.Info(eventArgs.Message);
                var updateStatus = Builders<SimulationModel>.Update.Set(s => s.status, $"{eventArgs.Message}");
                Simulations.UpdateOne(s => s.simulationId == SimulationId, updateStatus);
            };

            var timer = Stopwatch.StartNew();

            runner.Run();
            LogProgressToConsole(timer.Elapsed, "simulation run");

            var folderPathForNewAnalysis = $"DownloadedReports\\{SimulationId}_NewAnalysis";
            var relativeFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, folderPathForNewAnalysis);
            Directory.CreateDirectory(relativeFolderPath);

            var outputFile = $"Network {parameters.NetworkId} - Simulation {parameters.SimulationId}.json";
            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, folderPathForNewAnalysis, outputFile);
            using var outputStream = File.Create(filePath);
            using var outputWriter = new Utf8JsonWriter(outputStream, new JsonWriterOptions { Indented = true });
            JsonSerializer.Serialize(outputWriter, newSimulation.Results, new JsonSerializerOptions { Converters = { new JsonStringEnumConverter() } });
        }

        private void Consume(object state)
        {
            var consumerIndex = (int)state;
            foreach (var task in Queue.GetConsumingEnumerable())
            {
                try
                {
                    if (!task.IsCanceled)
                    {
                        task.RunSynchronously();
                    }
                }
                catch (InvalidOperationException)
                {
                }

                if (consumerIndex >= MaximumConcurrency)
                {
                    if (!Consumers.TryRemove(consumerIndex, out var _))
                    {
                        throw new InvalidOperationException("Consumer does not exist.");
                    }

                    break;
                }
            }
        }
    }
}
