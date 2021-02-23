using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using BridgeCareCore.Hubs;
using BridgeCareCore.Interfaces.Simulation;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Data.SqlClient.Server;
using MoreLinq;

namespace BridgeCareCore.Services.SimulationAnalysis
{
    public class SimulationAnalysis : ISimulationAnalysis
    {
        private readonly UnitOfDataPersistenceWork _unitOfDataPersistenceWork;
        private readonly IHubContext<BridgeCareHub> HubContext;

        public SimulationAnalysis(UnitOfDataPersistenceWork unitOfDataPersistenceWork, IHubContext<BridgeCareHub> hub)
        {
            _unitOfDataPersistenceWork = unitOfDataPersistenceWork ?? throw new ArgumentNullException(nameof(unitOfDataPersistenceWork));
            HubContext = hub ?? throw new ArgumentNullException(nameof(hub));
        }

        public Task CreateAndRun(Guid networkId, Guid simulationId)
        {
            var simulationAnalysisDetail = new SimulationAnalysisDetailDTO
            {
                SimulationId = simulationId,
                LastRun = DateTime.Now,
                Status = "Starting analysis..."
            };
            _unitOfDataPersistenceWork.SimulationAnalysisDetailRepo.UpsertSimulationAnalysisDetail(simulationAnalysisDetail);
            sendSimulationAnalysisDetail(simulationAnalysisDetail);

            var explorer = _unitOfDataPersistenceWork.AttributeRepo.GetExplorer();
            var network = _unitOfDataPersistenceWork.NetworkRepo.GetSimulationAnalysisNetwork(networkId, explorer);
            _unitOfDataPersistenceWork.SimulationRepo.GetSimulationInNetwork(simulationId, network);

            var simulation = network.Simulations.First();
            _unitOfDataPersistenceWork.InvestmentPlanRepo.GetSimulationInvestmentPlan(simulation);
            _unitOfDataPersistenceWork.AnalysisMethodRepo.GetSimulationAnalysisMethod(simulation);
            _unitOfDataPersistenceWork.PerformanceCurveRepo.SimulationPerformanceCurves(simulation);
            _unitOfDataPersistenceWork.SelectableTreatmentRepo.GetSimulationTreatments(simulation);

            var runner = new SimulationRunner(simulation);

            runner.Failure += (sender, eventArgs) =>
            {
                simulationAnalysisDetail.Status = eventArgs.Message;
                UpdateSimulationAnalysisDetail(simulationAnalysisDetail, DateTime.Now);
                sendRealTimeMessage(eventArgs.Message, simulationId);
                sendSimulationAnalysisDetail(simulationAnalysisDetail);
            };
            runner.Information += (sender, eventArgs) =>
            {
                if (eventArgs.Message == "Simulation complete.")
                {
                    simulationAnalysisDetail.Status = eventArgs.Message;
                    UpdateSimulationAnalysisDetail(simulationAnalysisDetail, DateTime.Now);
                    _unitOfDataPersistenceWork.SimulationOutputRepo.CreateSimulationOutput(simulationId, simulation.Results);
                }
                else
                {
                    simulationAnalysisDetail.Status = eventArgs.Message;
                    UpdateSimulationAnalysisDetail(simulationAnalysisDetail, null);
                }

                sendRealTimeMessage(eventArgs.Message, simulationId);
                sendSimulationAnalysisDetail(simulationAnalysisDetail);
            };
            runner.Warning += (sender, eventArgs) =>
            {
                sendRealTimeMessage(eventArgs.Message, simulationId);
            };

            runner.Run();

            return Task.CompletedTask;
        }

        private void UpdateSimulationAnalysisDetail(SimulationAnalysisDetailDTO simulationAnalysisDetail, DateTime? stopDateTime)
        {
            if (stopDateTime != null)
            {
                var interval = stopDateTime - simulationAnalysisDetail.LastRun;
                simulationAnalysisDetail.RunTime = interval.Value.ToString(@"hh\:mm\:ss");
            }
            _unitOfDataPersistenceWork.SimulationAnalysisDetailRepo.UpsertSimulationAnalysisDetail(simulationAnalysisDetail);
        }

        private void sendRealTimeMessage(string message, Guid simulationId)
        {
            HubContext
                .Clients
                .All
                .SendAsync("BroadcastScenarioStatusUpdate", message, simulationId);
        }

        private void sendSimulationAnalysisDetail(SimulationAnalysisDetailDTO simulationAnalysisDetail)
        {
            HubContext
                .Clients
                .All
                .SendAsync("BroadcastSimulationAnalysisDetail", simulationAnalysisDetail);
        }
    }
}
