using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs;
using BridgeCareCore.Hubs;
using BridgeCareCore.Interfaces.Simulation;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Data.SqlClient.Server;
using MoreLinq;

namespace BridgeCareCore.Services.SimulationAnalysis
{
    public class SimulationAnalysis : ISimulationAnalysis
    {
        private readonly IAttributeRepository _attributeRepo;
        private readonly INetworkRepository _networkRepo;
        private readonly IInvestmentPlanRepository _investmentPlanRepo;
        private readonly IAnalysisMethodRepository _analysisMethodRepo;
        private readonly IPerformanceCurveRepository _performanceCurveRepo;
        private readonly ISelectableTreatmentRepository _selectableTreatmentRepo;
        private readonly ISimulationRepository _simulationRepo;
        private readonly ISimulationOutputRepository _simulationOutputRepo;
        private readonly ISimulationAnalysisDetailRepository _simulationAnalysisDetailRepo;
        private readonly IHubContext<BridgeCareHub> HubContext;

        public SimulationAnalysis(IAttributeRepository attributeRepo, INetworkRepository networkRepo,
            IInvestmentPlanRepository investmentPlanRepo, IAnalysisMethodRepository analysisMethodRepo,
            IPerformanceCurveRepository performanceCurveRepo, ISelectableTreatmentRepository selectableTreatmentRepo,
            ISimulationRepository simulationRepo, ISimulationOutputRepository simulationOutputRepo,
            ISimulationAnalysisDetailRepository simulationAnalysisDetailRepo, IHubContext<BridgeCareHub> hub)
        {
            _attributeRepo = attributeRepo ?? throw new ArgumentNullException(nameof(attributeRepo));
            _networkRepo = networkRepo ?? throw new ArgumentNullException(nameof(networkRepo));
            _investmentPlanRepo = investmentPlanRepo ?? throw new ArgumentNullException(nameof(investmentPlanRepo));
            _analysisMethodRepo = analysisMethodRepo ?? throw new ArgumentNullException(nameof(analysisMethodRepo));
            _performanceCurveRepo = performanceCurveRepo ?? throw new ArgumentNullException(nameof(performanceCurveRepo));
            _selectableTreatmentRepo = selectableTreatmentRepo ?? throw new ArgumentNullException(nameof(selectableTreatmentRepo));
            _simulationRepo = simulationRepo ?? throw new ArgumentNullException(nameof(simulationRepo));
            _simulationOutputRepo = simulationOutputRepo ?? throw new ArgumentNullException(nameof(simulationOutputRepo));
            _simulationAnalysisDetailRepo = simulationAnalysisDetailRepo ?? throw new ArgumentNullException(nameof(simulationAnalysisDetailRepo));
            HubContext = hub ?? throw new ArgumentNullException(nameof(hub));
        }

        public void CreateAndRun(Guid networkId, Guid simulationId)
        {
            var explorer = _attributeRepo.GetExplorer();
            var network = _networkRepo.GetSimulationAnalysisNetwork(networkId, explorer);
            _simulationRepo.GetSimulationInNetwork(simulationId, network);

            var simulation = network.Simulations.First();
            _investmentPlanRepo.GetSimulationInvestmentPlan(simulation);
            _analysisMethodRepo.GetSimulationAnalysisMethod(simulation);
            _performanceCurveRepo.GetSimulationPerformanceCurves(simulation);
            _selectableTreatmentRepo.GetSimulationTreatments(simulation);

            var simulationAnalysisDetail = new SimulationAnalysisDetailDTO
            {
                SimulationId = simulation.Id,
                LastRun = DateTime.Now,
                Status = "Starting analysis..."
            };
            _simulationAnalysisDetailRepo.UpsertSimulationAnalysisDetail(simulationAnalysisDetail);

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
                    _simulationOutputRepo.CreateSimulationOutput(simulationId, simulation.Results);
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
        }

        public void CreateSimulation(string simulationName)
        {

        }

        public void GetAllSimulations(Guid networkId)
        {
            var explorer = _attributeRepo.GetExplorer();
            var network = _networkRepo.GetSimulationAnalysisNetwork(networkId, explorer);
            _simulationRepo.GetAllInNetwork(network);
        }

        private void UpdateSimulationAnalysisDetail(SimulationAnalysisDetailDTO simulationAnalysisDetail, DateTime? stopDateTime)
        {
            if (stopDateTime != null)
            {
                var interval = stopDateTime - simulationAnalysisDetail.LastRun;
                simulationAnalysisDetail.RunTime = interval.Value.ToString(@"hh\:mm\:ss");
            }
            _simulationAnalysisDetailRepo.UpsertSimulationAnalysisDetail(simulationAnalysisDetail);
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
